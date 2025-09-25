using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets;

public abstract class Packet
{
    public int Id;

    public int WriteId; 
    
    public Packet(int id)
    {
        Id = id;
        WriteId = id;
    }

    public Packet(int r, int w)
    {
        Id = r;
        WriteId = w;
    }

    public Packet()
    {
        Id = -1;
    }

    public abstract void Read(MinecraftStream stream);

    public abstract void Write(MinecraftStream stream);
}