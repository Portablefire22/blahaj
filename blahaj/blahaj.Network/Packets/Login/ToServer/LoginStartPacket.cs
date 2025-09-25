using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Login;

public class LoginStartPacket : Packet
{
    public string Name { get; private set; }
    public Guid Uuid { get; private set; }
    
    public LoginStartPacket() : base(0)
    {
    }

    public override void Read(MinecraftStream stream)
    {
        Name = stream.ReadString();
        Uuid = stream.ReadUuid();
    }

    public override void Write(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }
}