using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using blahaj.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace blahaj;

public class NetServer : IDisposable
{
    private readonly ILogger<NetServer> _logger;
    private IConfigurationRoot _config;
    private TcpListener Listener;
    private ConcurrentDictionary<EndPoint, NetClient> Connections;

    public NetServer()
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build.AddConsole());
        _logger = factory.CreateLogger<NetServer>();
        
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        Connections = new ConcurrentDictionary<EndPoint, NetClient>();
    }

    private void Run()
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

        var netClient = new NetClient();
        var endpoint = client.Client.RemoteEndPoint;
        if (endpoint == null) return;
        if (!Connections.TryAdd(endpoint, netClient)) return;
        _logger.LogInformation($"Added client: {endpoint}");
        netClient.OnConnectionClosed += (sender, args) => OnClientDisconnect(args);
    }

    private void OnClientDisconnect(ConnectionClosedArgs args)
    {
        var endpoint = args.Connection.RemoteEndPoint;
        if (endpoint == null) return;
        if (Connections.TryRemove(endpoint, out var cl))
        {
            _logger.LogInformation($"Client Disconnected: {cl.RemoteEndPoint}");
        }
    }

    public void Dispose()
    {
        Listener.Dispose();
    }
}