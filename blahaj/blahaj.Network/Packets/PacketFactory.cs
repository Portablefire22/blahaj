namespace blahaj.blahaj.Network.Packets;

public class PacketFactory
{

    private Dictionary<int, Func<Packet>> Packets { get; } = [];

    public PacketFactory() {}
    
    public bool TryGet(int packetId, out Packet? packet)
    {
        if (!Packets.TryGetValue(packetId, out var p))
        {
            packet = null;
            return false;
        }
        packet = p();
        return true;
    }

    public void Register(Func<Packet> newPacketFunc)
    {
        var pack = newPacketFunc();
        Packets.Add(pack.Id, newPacketFunc);
    }
}