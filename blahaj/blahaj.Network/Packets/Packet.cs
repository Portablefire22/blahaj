using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets;

public abstract class Packet
{
    public int Id;
    public Packet(int id)
    {
        Id = id;
    }

    public Packet()
    {
        Id = -1;
    }

    public abstract void Read(MinecraftStream stream);

    public abstract void Write(MinecraftStream stream);
}