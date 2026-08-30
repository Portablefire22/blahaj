using blahaj.blahaj.Entities;

namespace blahaj.blahaj.Network.Packets.Interfaces;

public interface IInvokableWithPlayer : IInvokable
{
    public MinecraftPlayer Player { get; set; }
}