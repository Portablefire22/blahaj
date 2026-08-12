using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using blahaj.blahaj.Crypto;
using blahaj.blahaj.Player;
using blahaj.blahaj.Registry;
using blahaj.blahaj.Registry.Packs;
using blahaj.blahaj.Stream;
using blahaj.blahaj.World;
using blahaj.Network.Packets;
using blahaj.Network.Events;
using blahaj.Network.Packets.Configuration;
using blahaj.Network.Packets.Configuration.ToClient;
using blahaj.Network.Packets.Configuration.ToServer;
using blahaj.Network.Packets.Handshake;
using blahaj.Network.Packets.Login;
using blahaj.Network.Packets.Login.Json;
using blahaj.Network.Packets.Play.ToClient;
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

    public MinecraftPlayer Player { get; private set; }
    
    public bool UseCompression = false;

    private ConnectionState _conState = ConnectionState.Handshake;
    
    private ConnectionState ConnectionState
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

    private byte[]? RandomToken;
    
    private bool ShouldStop { get; set; }
    
    private MinecraftStream ReaderStream { get; set; }
    private MinecraftStream WriterStream { get; set; }

    private Pack[] KnownPacks { get; set; }
    
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
        switch (ConnectionState)
        {
            case ConnectionState.Handshake:
                HandleHandshake(args.Packet);
                break;
            case ConnectionState.Status:
                HandleStatus(args.Packet);
                break;
            case ConnectionState.Login:
                HandleLogin(args.Packet);
                break;
            case ConnectionState.Configuration:
                HandleConfiguration(args.Packet);
                break;
            case ConnectionState.Play:
                HandlePlay(args.Packet);
                break;
            default:
                Logger.LogCritical($"Invalid State: {ConnectionState}");
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
                Logger.LogCritical($"Invalid Handshake packet: {packet.Id}");
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
            case EncryptionResponsePacket encryptionResponsePacket:
                HandleEncryptionResponse(encryptionResponsePacket);
                break;
            default:
                Logger.LogCritical($"Invalid Status packet: {packet.Id}");
                break;
        }
    }
    
    private void HandleLogin(Packet packet)
    {
        switch (packet)
        {
            case LoginStartPacket loginStartPacket:
                HandleLoginStart(loginStartPacket);
                break;
            case EncryptionResponsePacket encryptionResponsePacket:
                HandleEncryptionResponse(encryptionResponsePacket);
                break;
            case LoginAcknowledgedPacket:
                ConnectionState = ConnectionState.Configuration;
                SendKnownPacks();
                break;
            default:
                Logger.LogCritical($"Invalid Login packet: {packet.Id}");
                break;
        }
    }
    private void HandleLoginStart(LoginStartPacket packet)
    {
        Player = new MinecraftPlayer(packet.Name, packet.Uuid);
        Logger.LogInformation($"Connecting Player: {packet.Name} ({packet.Uuid})");
        SendEncryptionRequest();
    }

    private void SendEncryptionRequest()
    {
        var packet = new EncryptionRequestPacket("BlahajCSharpMeowPurr", true);
        RandomToken = packet.VerifyToken;
        WriteQueue.Add(packet);
    }


    private void HandleEncryptionResponse(EncryptionResponsePacket packet)
    {
        string serverHash;
        LoginSuccessJson? json;
        using (var ms = new MemoryStream())
        {
            var ascii = Encoding.ASCII.GetBytes("BlahajCSharpMeowPurr");
            ms.Write(ascii, 0, ascii.Length);
            ms.Write(packet.SharedSecret, 0, 16);
            var publicKey = Encryption.ExportKeyAsDer();
            ms.Write(publicKey, 0, publicKey.Length);
            serverHash = MinecraftShaDigest.Sha(ms.ToArray());
        }
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://sessionserver.mojang.com/");
            Logger.LogDebug($"{client.BaseAddress}session/minecraft/hasJoined?username={Player.Name}&serverId={serverHash}");
            var res = client.GetStringAsync($"session/minecraft/hasJoined?username={Player.Name}&serverId={serverHash}").Result;
            if (res.Length == 0) {
                Disconnect();
                return;
            }

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            json = JsonSerializer.Deserialize<LoginSuccessJson>(res, options);  
            if (json == null) {
                Disconnect();
                return;
            }
            Logger.LogInformation("Authentication Successful");
        }
        ReaderStream.InitEncryption(packet.SharedSecret);
        WriterStream.InitEncryption(packet.SharedSecret);
        SendLoginSuccess(json);
    }

    private void HandlePing(PingPacket packet)
    {
        WriteQueue.Add(packet);
    }

    private void SendLoginSuccess(LoginSuccessJson json)
    {
        var packet = new LoginSuccessPacket(json, Player.SessionId);
        WriteQueue.Add(packet);
    }

    private void HandleConfiguration(Packet packet)
    {
        switch (packet)
        {
            case BrandPacket brandPacket:
                HandleBrand(brandPacket);
                break;
            case ClientInformationPacket clientInformationPacket:
                HandleClientInformation(clientInformationPacket);
                break;
            case KnownPacksPacket knownPacksPacket:
                HandleKnownPacks(knownPacksPacket);
                break;
            case AcknowledgeFinishConfiguration:
                HandleFinishConfiguration();
                break;
            default:
                Logger.LogCritical($"Invalid Configuration packet: {packet.Id}");
                break;
        }
    }

    private void HandleClientInformation(ClientInformationPacket packet)
    {
        Logger.LogDebug($"Client Information: {packet.Locale} {packet.ViewDistance}");   
    }

    private void HandleBrand(BrandPacket packet)
    {
        var brand = new BrandPacket("Blahaj");
        WriteQueue.Add(brand);
    }

    private void SendKnownPacks()
    {
        var packs = new Pack[] { new MinecraftCorePack(Server.Config["version"]) };
        WriteQueue.Add(new KnownPacksPacket(packs));
    }

    private void HandleKnownPacks(KnownPacksPacket packet)
    {
        KnownPacks = packet.KnownPacks;
        SendRegistryData();
    }

    private void SendRegistryData()
    {
        WriteQueue.Add(new RegistryDataPacket(RegistryController.DamageTypeRegistry));
        
        SendFinishConfig();
    }

    private void SendFinishConfig()
    {
       WriteQueue.Add(new FinishConfigurationPacket()); 
    }

    private void HandleFinishConfiguration()
    {
        ConnectionState = ConnectionState.Play;
        SendLoginPlay();
    }

    private void HandlePlay(Packet packet)
    {
        switch (packet)
        {
            default:
                Logger.LogCritical($"Invalid Play packet: {packet.Id}");
                break;
        }
    }

    private void SendLoginPlay()
    {
        // Ewwwwwwwwwwwwww
        var set = WorldSettings.FromConfig(Server.Config);
        var packet = new LoginPacket(set, 1, "overworld", false, null, 
            null, 0, 0);
        WriteQueue.Add(packet);
    }
    
    private void HandleStatusResponse(StatusRequestPacket packet)
    {
        // Maybe I should check for nulls, or maybe the user should just set up configs correctly
        var version = new StatusVersion(Server.Config["version"], short.Parse(Server.Config["protocol"]));
        // Get all connections that contain a valid player
        var validPlayers = Server.Players.Where(x => x.IsValid()).ToArray();
        // Get all players that want to show in the listing
        var temp = validPlayers.Where(x => x.ClientInformation.AllowServerListings).Select(x => 
                x.ToStatus()).ToArray();
        var players = new StatusPlayers(int.Parse(Server.Config["maxPlayers"]), validPlayers.Length,
            temp.Length > 0 ? temp : []);
        var desc = new StatusDescription(Server.Config["motd"]);
        
        var resp = new StatusResponse(version, players, desc, $"data:image/png;base64,{Server.Favicon}", 
            bool.Parse(Server.Config["enforcesSecureChat"]));
        WriteQueue.Add(new StatusResponsePacket(resp));
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