using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text.Json;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Events;
using blahaj.blahaj.Network.Packets.Play.PlayerInfo;
using blahaj.blahaj.Network.Packets.Play.ToClient;
using blahaj.blahaj.Registry;
using blahaj.blahaj.World;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace blahaj.blahaj.Network;

public class NetServer : IDisposable
{
    private ILogger<NetServer> Logger { get; }
    public IConfigurationRoot Config { get; }
    private TcpListener Listener;
    private ConcurrentDictionary<EndPoint, NetClient> Connections;
    
    private List<MinecraftPlayer> ConnectedPlayers = [];
    
    public EventHandler<PlayerConnectedArgs> OnPlayerConnected;
   
    public MinecraftWorld World { get; }
    
    
    public string Favicon { get; }

    public MinecraftPlayer[] Players
    {
        get => [.. Connections.Where(x => x.Value.Player != null)
            .Select(x => x.Value.Player)!]!;
    }

    public NetServer()
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build.AddConsole());
        Logger = factory.CreateLogger<NetServer>();
        
        Config = new ConfigurationBuilder()
            .AddJsonFile("blahaj.Network/appsettings.json")
            .Build();
        Connections = new ConcurrentDictionary<EndPoint, NetClient>();

        var favicon = Config["favicon"];
        var path = favicon != null ? favicon : "placeholder.png";
        using (var image = Image.Load(path))
        {
            using (var m = new MemoryStream())
            {
                if (image.Width != 64 || image.Height != 64)
                {
                    image.Mutate(x => x.Resize(64,64, KnownResamplers.Lanczos8));
                }
                image.SaveAsPng(m);
                var imageBytes = m.ToArray();
                Favicon = Convert.ToBase64String(imageBytes);
            }
        }

        World = new MinecraftWorld(this, WorldSettings.FromConfig(Config));
        
        OnPlayerConnected = OnPlayerConnection;
    }
    public void Run()
    {
        RegistryController.Initialise();
       
        World.Start();
        
        var ipEndPoint = new IPEndPoint(IPAddress.Parse(Config["ip"]), int.Parse(Config["port"]));
        Listener = new TcpListener(ipEndPoint);
        Listener.Start();
        Listener.BeginAcceptTcpClient(ConnectionCallback, null);
    }

    private async void OnPlayerConnection(object? sender, PlayerConnectedArgs args)
    {
        ConnectedPlayers.Add(args.Player);
        if (args.Player.GameProfile == null) return;
        
        // Inform everyone of the new player
        var info = new AddPlayerInfoAction()
        {
            Name = args.Player.Name,
            Properties    = args.Player.GameProfile.Properties,
        };

        var packet = new PlayerInfoUpdate()
        {
            Players = [new KeyValuePair<Guid, IPlayerInfoAction[]>((Guid)args.Player.Uuid, [
                info, 
                        new UpdateListedPlayerAction()
                ])
            ],
            Action = 0x01 | 0x08
        };
        foreach (var player in ConnectedPlayers)
        {
            player.QueuePacket(packet);
        }
    
        // Inform connecting player of everyone on the server
        foreach (var player in ConnectedPlayers)
        {
            if (player == args.Player) continue;
            
            if (args.Player.GameProfile == null) return;
            info = new AddPlayerInfoAction()
            {
                Name = args.Player.Name,
                Properties    = args.Player.GameProfile.Properties,
            };

            packet = new PlayerInfoUpdate()
            {
                Players = [new KeyValuePair<Guid, IPlayerInfoAction[]>((Guid)args.Player.Uuid, [
                        info, 
                        new UpdateListedPlayerAction()
                    ])
                ],
                Action = 0x01 | 0x08
            };
            args.Player.QueuePacket(packet);
        }
    }
    
    private void ConnectionCallback(IAsyncResult ar)
    {

        var client = Listener.EndAcceptTcpClient(ar);
        // Get other clients
        Listener.BeginAcceptTcpClient(ConnectionCallback, null);
        
        if (!client.Connected) return;

        var netClient = new NetClient(client, this);
        var endpoint = netClient.RemoteEndPoint;
        if (endpoint == null) return;
        if (!Connections.TryAdd(endpoint, netClient)) return;
        Logger.LogInformation($"Added client: {endpoint}");
        netClient.OnConnectionClosed += (sender, args) => OnClientDisconnect(args);
        
        netClient.Initialise();
    }

    private void OnClientDisconnect(ConnectionClosedArgs args)
    {
        var endpoint = args.Connection.RemoteEndPoint;
        if (endpoint == null) return;
        if (Connections.TryRemove(endpoint, out var cl))
        {
            Logger.LogInformation($"Client Disconnected: {cl.RemoteEndPoint}");
        }
    }

    public void Dispose()
    {
        Listener.Dispose();
    }
}