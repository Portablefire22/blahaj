using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using blahaj.Network.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace blahaj.Network;

public class NetServer : IDisposable
{
    private ILogger<NetServer> Logger { get; }
    private IConfigurationRoot _config;
    private TcpListener Listener;
    private ConcurrentDictionary<EndPoint, NetClient> Connections;

    public NetServer()
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build.AddConsole());
        Logger = factory.CreateLogger<NetServer>();
        
        _config = new ConfigurationBuilder()
            .AddJsonFile("blahaj.Network/appsettings.json")
            .Build();
        Connections = new ConcurrentDictionary<EndPoint, NetClient>();
    }
    public void Run()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Parse(_config["ip"]), int.Parse(_config["port"]));
        Listener = new TcpListener(ipEndPoint);
        Listener.Start();
        Listener.BeginAcceptTcpClient(ConnectionCallback, null);
    }

    private void ConnectionCallback(IAsyncResult ar)
    {

        var client = Listener.EndAcceptTcpClient(ar);
        // Get other clients
        Listener.BeginAcceptTcpClient(ConnectionCallback, null);
        
        if (!client.Connected) return;

        var netClient = new NetClient(client);
        var endpoint = netClient.RemoteEndPoint;
        if (endpoint == null) return;
        if (!Connections.TryAdd(endpoint, netClient)) return;
        Logger.LogInformation($"Added client: {endpoint}");
        netClient.OnConnectionClosed += (sender, args) => OnClientDisconnect(args);
        netClient.OnPacketReceived += (sender, args) => OnPacketReceived(args);
        
        netClient.Initialise();
    }

    private void OnPacketReceived(PacketReceivedArgs args)
    {
        
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