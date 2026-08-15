using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using blahaj.blahaj.Crypto;
using blahaj.blahaj.Network.Events;
using blahaj.blahaj.Network.Packets;
using blahaj.blahaj.Network.Packets.Configuration;
using blahaj.blahaj.Network.Packets.Configuration.ToClient;
using blahaj.blahaj.Network.Packets.Configuration.ToServer;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Network.Packets.Login.ToClient;
using blahaj.blahaj.Network.Packets.Login.ToClient.Json;
using blahaj.blahaj.Network.Packets.Login.ToServer;
using blahaj.blahaj.Network.Packets.Play.ToClient;
using blahaj.blahaj.Network.Packets.Status;
using blahaj.blahaj.Network.Packets.Status.ToClient;
using blahaj.blahaj.Network.Packets.Status.ToClient.Json;
using blahaj.blahaj.Network.Packets.Status.ToServer;
using blahaj.blahaj.Player;
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

    private NetServer Server { get; }
   
    public EndPoint? RemoteEndPoint { get; }

    public MinecraftPlayer Player { get; set; }
    
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
    
    private bool ShouldStop { get; set; }
    
    private MinecraftStream ReaderStream { get; set; }
    private MinecraftStream WriterStream { get; set; }

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
        Player = new MinecraftPlayer();
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
                while (true)
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
                    Logger.LogDebug($"Received: {packet.GetType()}");
                    OnPacketReceived?.Invoke(this, args);
                    Thread.Sleep(1);
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
                var packet = WriteQueue.Take();
                var stream = new MemoryStream();
                using (var st = new MinecraftStream(stream))
                {
                    st.WriteVarInt(packet.WriteId);
                    packet.Write(st);
                }

                #if DEBUG
                var str = "";
                foreach (var by in stream.GetBuffer())
                {
                    str += $"{by} ";
                }
                Logger.LogDebug(str);
                #endif
                
                var arr = stream.ToArray();
                ms.WriteVarInt(arr.Length);
                ms.WriteByteArray(arr);
                stream.Dispose();
            }
        }
    }

    public void Dispose()
    {
       TcpClient.Dispose();
    }
}