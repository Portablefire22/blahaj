namespace blahaj.blahaj.Network.Packets.Interfaces;

public interface IInvokableWithClient :  IInvokable
{
    public NetClient Client { get; set; }
}