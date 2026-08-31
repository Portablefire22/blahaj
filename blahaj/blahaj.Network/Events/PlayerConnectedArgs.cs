using blahaj.blahaj.Entities;

namespace blahaj.blahaj.Network.Events;

public class PlayerConnectedArgs
{
    public PlayerConnectedArgs(MinecraftPlayer player)
    {
        Player = player;
    }

    public MinecraftPlayer Player { get; }
}