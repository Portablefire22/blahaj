using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.PlayerInfo;

public interface IPlayerInfoAction
{
    public void Write(MinecraftStream stream);
}