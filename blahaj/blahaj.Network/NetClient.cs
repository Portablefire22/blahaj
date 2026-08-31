using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Events;
using blahaj.blahaj.Network.Packets;
using blahaj.blahaj.Network.Packets.Configuration;
using blahaj.blahaj.Network.Packets.Configuration.ToClient;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Network.Packets.Play.ToClient;
using blahaj.blahaj.Registry;
using blahaj.blahaj.Registry.Packs;
using blahaj.blahaj.Stream;
using blahaj.blahaj.World;
using Microsoft.Extensions.Logging;

namespace blahaj.blahaj.Network;

public class NetClient : IDisposable
{
    private ILogger<NetClient> Logger {get;}
    public TcpClient TcpClient { get; }

    public NetServer Server { get; }
   
    public EndPoint? RemoteEndPoint { get; protected set;  }

    public MinecraftPlayer? Player { get; set; }

    public MinecraftWorld MinecraftWorld => Server.World;
    
    
    public bool UseCompression = false;

    private ConnectionState _conState = ConnectionState.Handshake;
    
    public ConnectionState ConnectionState
    {
        get => _conState;
        set
        {
            Logger.LogInformation($"Changing ConnectionState to {value}");
            _conState = value;
        }
    }

    public EventHandler<ConnectionClosedArgs>? OnConnectionClosed { get; set; }
    public EventHandler<PacketReceivedArgs>? OnPacketReceived { get; set; }

    private Task NetworkReading { get; set; }
    private Task NetworkWriting { get; set; }

    private BlockingCollection<Packet> WriteQueue { get; }

    public byte[]? RandomToken { get; set; }
    
    protected bool ShouldStop { get; set; }
    
    public MinecraftStream ReaderStream { get; protected set; }
    public MinecraftStream WriterStream { get; protected set; }

    public Pack[] KnownPacks { get; set; }
    
    public NetClient(TcpClient tcpClient, NetServer server)
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build
            #if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
            #endif
            .AddConsole());
        Logger = factory.CreateLogger<NetClient>();

        Server = server;
        
        TcpClient = tcpClient;
        RemoteEndPoint = tcpClient.Client.RemoteEndPoint;

        OnPacketReceived += (sender, args) => OnPacket(args) ;
        
        WriteQueue = new BlockingCollection<Packet>();
        ShouldStop = false;
    }

    public void InitEncryption(byte[] sharedSecret)
    {
        ReaderStream.InitEncryption(sharedSecret);
        WriterStream.InitEncryption(sharedSecret);
    }
    
    public void Stop()
    {
        ShouldStop = true;
    }

    public void Initialise()
    {
        NetworkReading = new Task(ReadStream);
        NetworkReading.Start();
        NetworkWriting = new Task(WriteStream);
        NetworkWriting.Start();
    }

    public void Disconnect()
    {
        ShouldStop = true;
        OnConnectionClosed?.Invoke(this, new ConnectionClosedArgs(this));
        TcpClient.Close();
    }
    
    private void ReadStream()
    {
        try
        {
            using NetworkStream ns = TcpClient.GetStream();
            using (MinecraftStream ms = new MinecraftStream(ns))
            {
                ReaderStream = ms;
                while (!ShouldStop)
                {
                    var length = ms.ReadVarInt();
                    var packetId = ms.ReadVarInt();
                    byte[] data;
                    if (UseCompression)
                    {
                        data = new byte[length];
                    }
                    else
                    {
                        data = ms.ReadByteArray(length - 1);
                    }

                    var packet = MinecraftPacketFactory.GetPacket(ConnectionState, packetId);
                    if (packet == null)
                    {
                        Logger.LogCritical($"Unknown Packet ID: {packetId}, State: {ConnectionState}");
                        continue;
                    }
                    
                    packet.Read(new MinecraftStream(new MemoryStream(data)));
                    var args = new PacketReceivedArgs(packet);
                    if (packet.ShouldLog)
                    {
                        Logger.LogDebug($"Received: {packet.GetType()}");
                    }

                    OnPacketReceived?.Invoke(this, args);
                    Task.Delay(1).Wait();
                }
            }
        }
        catch (EndOfStreamException e) {}
        catch (Exception e)
        {
            Logger.LogCritical($"Client disconnected due to exception: {e}");
        }
        finally
        {
            Disconnect();
        }
    }

    
    private void OnPacket(PacketReceivedArgs args)
    {

        if (args.Packet is not IInvokable invokable) return;

        if (invokable is IInvokableWithClient client)
        {
            client.Client = this;
        }
        if (invokable is IInvokableWithServer server)
        {
            server.Server = Server;
        }
        if (invokable is IInvokableWithPlayer player)
        {
            player.Player = Player;
        }
        
        var resp = invokable.Invoke();
        
        if (resp != null) WriteQueue.Add(resp);
    }

    public void QueuePacket(Packet packet)
    {
        WriteQueue.Add(packet);
    }

    public void SendKnownPacks()
    {
        var packs = new Pack[] { new MinecraftCorePack(Server.Config["version"]) };
        WriteQueue.Add(new KnownPacksPacket(packs));
    }
    
    public void SendRegistryData()
    {
        WriteQueue.Add(new RegistryDataPacket(RegistryController.DamageTypeRegistry));
        foreach (var variant in RegistryController.VariantsRegistry)
        {
            WriteQueue.Add(new RegistryDataPacket(variant));
        }

        WriteQueue.Add(new UpdateTagsPacket([.. RegistryController.TaggedRegistries]));
        
        SendFinishConfig();
    }

    private void SendFinishConfig()
    {
       WriteQueue.Add(new FinishConfigurationPacket()); 
    }

    private void WriteStream()
    {
        using NetworkStream ns = TcpClient.GetStream();
        using (MinecraftStream ms = new MinecraftStream(ns))
        {
            WriterStream = ms;
            while (!ShouldStop)
            {
                if (!WriteQueue.TryTake(out var packet))
                {
                    Task.Delay(10).Wait();
                    continue;
                }
                var stream = new MemoryStream();
                using (var st = new MinecraftStream(stream))
                {
                    st.WriteVarInt(packet.WriteId);
                    packet.Write(st);
                }

                var arr = stream.ToArray();
                #if DEBUG
                var str = "";
                foreach (var by in arr)
                {
                    str += $"{by} ";
                }
                Logger.LogDebug(str);
                #endif
                ms.WriteVarInt(arr.Length);
                ms.WriteByteArray(arr);
                stream.Dispose();
            }
        }
    }

    public void LoadPlayer()
    {
        if (Player == null)
        {
            Disconnect();
            return;
        }

        Player.Join(MinecraftWorld, MinecraftWorld.Dimensions[0]);

        /*
        // Fake chunk data

        var playerChunkX = (int) Player.Position.X / 16;
        var playerChunkZ = (int) Player.Position.Z / 16;
        
        
        var settings = WorldSettings.FromConfig(Server.Config);
        var width = 7 + (settings.ViewDistance * 2);
        for (int x = -(int) Math.Ceiling(width / 2f); x < (int) Math.Floor(width / 2f); x++)
        {
            for (int z = -(int) Math.Ceiling(width / 2f); z < (int) Math.Floor(width / 2f); z++)
            {
                QueuePacket(new ChunkDataWithLight(playerChunkX + x, playerChunkZ + z) );
            }
        }
        */
        
    }

    public void Dispose()
    {
       TcpClient.Dispose();
    }
}