using blahaj.Network.Packets;

namespace blahaj.Network.Events;

public class PacketReceivedArgs
{
    public PacketReceivedArgs(Packet packet)
    {
        Packet = packet;
    }

    public Packet Packet { get; set; }
    
}