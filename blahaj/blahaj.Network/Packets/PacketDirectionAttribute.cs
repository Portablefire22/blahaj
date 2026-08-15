namespace blahaj.blahaj.Network.Packets;

public class PacketDirectionAttribute : Attribute
{
    
    public PacketDirection Direction1 { get; set; }
    public PacketDirection? Direction2 { get; set; }

    public PacketDirection[] Directions
    {
        get
        {
            var length = Direction2 == null ? 1 : 2;
            var a = new PacketDirection[length];

            a[0] = Direction1;
            if (length == 1) return a;
            a[1] = (PacketDirection)Direction2!;
            return a;
        }
    }

    public PacketDirectionAttribute(PacketDirection direction)
    {
        Direction1 = direction;
    }

    public PacketDirectionAttribute(PacketDirection dir1, PacketDirection dir2)
    {
        Direction1 = dir1;
        Direction2 = dir2;
    }
}

public enum PacketDirection
{
    ClientToServer,
    ServerToClient,
}