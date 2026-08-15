namespace blahaj.blahaj.Network.Packets.Interfaces;

public interface IInvokableWithServer : IInvokable
{
    public NetServer Server { get; set; }
}