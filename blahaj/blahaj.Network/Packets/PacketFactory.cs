using blahaj.Network.Packets.Handshake;

namespace blahaj.Network.Packets;

public class PacketFactory<T, S, P> where P : Packet
{

    private Dictionary<int, Func<P>> Packets;

    public PacketFactory()
    {
        Packets = new Dictionary<int, Func<P>>();
    }
    
    public bool TryGet(int packetId, out P? packet)
    {
        if (!Packets.TryGetValue(packetId, out var p))
        {
            packet = null;
            return false;
        }
        packet = p();
        return true;
    }

    public void Register(Func<P> newPacketFunc)
    {
        var pack = newPacketFunc();
        Packets.Add(pack.Id, newPacketFunc);
    }
}