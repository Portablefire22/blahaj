using System.Collections.Concurrent;
using System.Drawing;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using blahaj.blahaj.Stream;
using blahaj.Network.Packets;
using blahaj.Network.Events;
using blahaj.Network.Packets.Handshake;
using blahaj.Network.Packets.Status;
using blahaj.Network.Packets.Status.Json;
using Microsoft.Extensions.Logging;

namespace blahaj.Network;

public class NetClient : IDisposable
{
    private ILogger<NetClient> Logger {get;}
    public TcpClient TcpClient { get; }

    private NetServer Server { get; }
   
    public EndPoint? RemoteEndPoint { get; }

    public bool UseCompression = false;
    private ConnectionState ConnectionState { get; set; } = ConnectionState.Handshake;
    
    public EventHandler<ConnectionClosedArgs>? OnConnectionClosed { get; set; }
    public EventHandler<PacketReceivedArgs>? OnPacketReceived { get; set; }

    private Task NetworkReading { get; set; }
    private Task NetworkWriting { get; set; }

    private BlockingCollection<Packet> WriteQueue { get; }
    
    private bool ShouldStop { get; set; }

    public NetClient(TcpClient tcpClient, NetServer server)
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build.AddConsole());
        Logger = factory.CreateLogger<NetClient>();

        Server = server;
        
        TcpClient = tcpClient;
        RemoteEndPoint = tcpClient.Client.RemoteEndPoint;

        OnPacketReceived += (sender, args) => OnPacket(args) ;
        
        WriteQueue = new BlockingCollection<Packet>();
        ShouldStop = false;

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
        var bf = new byte[4096];
        try
        {
            using NetworkStream ns = TcpClient.GetStream();
            using (MinecraftStream ms = new MinecraftStream(ns))
            {
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
                    OnPacketReceived?.Invoke(this, args);
                }
            };
        }
        finally
        {
            Disconnect();
        }
    }

    
    private void OnPacket(PacketReceivedArgs args)
    {
        switch (ConnectionState)
        {
            case ConnectionState.Handshake:
                HandleHandshake(args.Packet);
                break;
            case ConnectionState.Status:
                HandleStatus(args.Packet);
                break;
            default:
                Logger.LogCritical($"Invalid packet: {args.Packet.Id}");
                break;
        } 
    }

    private void HandleHandshake(Packet packet)
    {
        switch (packet)
        {
            case HandshakePacket handshakePacket:
                ConnectionState = handshakePacket.Intent;
                break;
            default:
                Logger.LogCritical($"Invalid packet: {packet.Id}");
                break;
        }
    }

    private void HandleStatus(Packet packet)
    {
        switch (packet)
        {
            case StatusRequestPacket statusRequestPacket:
                HandleStatusResponse(statusRequestPacket);
                break;
            case PingPacket pingPacket:
                HandlePing(pingPacket);
                break;
            default:
                Logger.LogCritical($"Invalid packet: {packet.Id}");
                break;
        }
    }

    private void HandlePing(PingPacket packet)
    {
        WriteQueue.Add(packet);
    }

    private void HandleStatusResponse(StatusRequestPacket packet)
    {
        // Maybe I should check for nulls, or maybe the user should just set up configs correctly
        var version = new StatusVersion(Server.Config["version"], short.Parse(Server.Config["protocol"]));
        var players = new StatusPlayers(int.Parse(Server.Config["maxPlayers"]), 0, new StatusPlayer[0]);
        var desc = new StatusDescription(Server.Config["motd"]);
        
        var resp = new StatusResponse(version, players, desc, $"data:image/png;base64,{Server.Favicon}", 
            bool.Parse(Server.Config["enforcesSecureChat"]));
        WriteQueue.Add(new StatusResponsePacket(resp));
    }

    private void WriteStream()
    {
        using NetworkStream ns = TcpClient.GetStream();
        using (MinecraftStream ms = new MinecraftStream(ns)) {
            while (!ShouldStop)
            {
                var packet = WriteQueue.Take();
                var stream = new MemoryStream();
                using (var st = new MinecraftStream(stream))
                {
                    st.WriteVarInt(packet.Id);
                    packet.Write(st);
                }

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