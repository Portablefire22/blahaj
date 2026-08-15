namespace blahaj.blahaj.Network.Events;

public class ConnectionClosedArgs
{
    public ConnectionClosedArgs(NetClient connection)
    {
        Connection = connection;
    }

    public NetClient Connection { get; }
}