namespace blahaj.blahaj.Network;

public class PacketStateAttribute : Attribute
{
    
    public ConnectionState State { get; }
    
    public PacketStateAttribute(ConnectionState state)
    {
        State = state;
    }
}