using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using blahaj.blahaj.Stream;
using blahaj.Network.Packets;
using blahaj.Network.Events;
using Microsoft.Extensions.Logging;

namespace blahaj.Network;

public class NetClient : IDisposable
{
    private ILogger<NetClient> Logger {get;}
    public TcpClient TcpClient { get; }
    public EndPoint? RemoteEndPoint { get; }
    
    public EventHandler<ConnectionClosedArgs>? OnConnectionClosed { get; set; }
    public EventHandler<PacketReceivedArgs>? OnPacketReceived { get; set; }

    private Task NetworkReading { get; set; }
    private Task NetworkWriting { get; set; }

    private BlockingCollection<Packet> WriteQueue { get; }
    
    private bool ShouldStop { get; set; }

    public NetClient(TcpClient tcpClient)
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build.AddConsole());
        Logger = factory.CreateLogger<NetClient>();
        
        TcpClient = tcpClient;
        RemoteEndPoint = tcpClient.Client.RemoteEndPoint;

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
                    
                }
            };
        }
        finally
        {
            Disconnect();
        }
    }

    private void WriteStream()
    {
        while (!ShouldStop)
        {
            var packet = WriteQueue.Take();
        }
    }

    public void Dispose()
    {
       TcpClient.Dispose();
    }
}