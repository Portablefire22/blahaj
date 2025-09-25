using blahaj.blahaj.Stream;
using blahaj.Network.Packets.Login.Json;

namespace blahaj.Network.Packets.Login;

public class LoginSuccessPacket : Packet
{
    public Guid Uuid { get; private set; }
    public string Username { get; private set; }
    public LoginSuccessProperties[] Properties { get; private set; }
    
    public LoginSuccessPacket(LoginSuccessJson json) : base(0x02)
    {
        Uuid = Guid.Parse(json.Id);
        Username = json.Name;
        Properties = json.Properties;
    }

    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteUuid(Uuid);
        stream.WriteString(Username);
        using var ms = new MemoryStream();
        using var mine = new MinecraftStream(ms);
        Properties = new LoginSuccessProperties[] { };
        foreach (var property in Properties)
        {
            mine.WriteString(property.Name);
            mine.WriteString(property.Value);
            mine.WritePrefixedOptionalString(property.Signature);
        }
        var dat = ms.ToArray();
        stream.WriteVarInt(dat.Length);
        stream.WriteByteArray(dat);
    }
}

public class LoginSuccessProperties
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string? Signature { get; set; }
}