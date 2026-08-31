using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.PlayerInfo;

public class AddPlayerInfoAction : IPlayerInfoAction
{
    
    public string Name { get; set; }
    public GameProfileProperties[] Properties { get; set; } = [];
    
    public void Write(MinecraftStream stream)
    {
        stream.WriteString(Name);
        stream.WriteVarInt(Properties.Length);
        foreach (var property in Properties)
        {
            stream.WriteString(property.Name);
            stream.WriteString(property.Value);
            stream.WritePrefixedOptionalString(property.Signature);
        }
    }
}