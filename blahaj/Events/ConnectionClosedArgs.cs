using System.Net;

namespace blahaj.Events;

public class ConnectionClosedArgs
{
    public ConnectionClosedArgs(NetClient connection)
    {
        Connection = connection;
    }

    public NetClient Connection { get; }
}