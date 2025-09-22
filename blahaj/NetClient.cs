using System.Net;
using System.Net.Sockets;
using blahaj.Events;

namespace blahaj;

public class NetClient
{
    public TcpClient TcpClient { get; }
    public EndPoint? RemoteEndPoint { get; }
    
    public EventHandler<ConnectionClosedArgs>? OnConnectionClosed;
    public EventHandler<PacketReceivedArgs>? OnPacketReceived;
}