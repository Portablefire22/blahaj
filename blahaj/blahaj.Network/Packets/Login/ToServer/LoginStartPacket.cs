using System.Security;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Network.Packets.Login.ToClient;
using blahaj.blahaj.Player;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Login.ToServer;

[PacketState(ConnectionState.Login)]
[PacketDirection(PacketDirection.ClientToServer)]
public class LoginStartPacket : Packet, IInvokableWithClient
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

    public Packet? Invoke()
    {
       Client.Player = new MinecraftPlayer(Name, Uuid);
       var packet = new EncryptionRequestPacket("BlahajCSharpMeowPurr", true);
       Client.RandomToken = packet.VerifyToken;
       return packet;
    }

    public NetClient Client { get; set; }
}