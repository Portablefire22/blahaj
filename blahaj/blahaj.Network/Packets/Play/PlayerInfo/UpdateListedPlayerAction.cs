using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.PlayerInfo;

public class UpdateListedPlayerAction : IPlayerInfoAction
{
    public bool Show { get; set; } = true;
    public void Write(MinecraftStream stream)
    {
        stream.WriteBool(Show);
    }
}