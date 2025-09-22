using System.Net;
using blahaj.Network;

namespace blahaj.Network.Events;

public class ConnectionClosedArgs
{
    public ConnectionClosedArgs(NetClient connection)
    {
        Connection = connection;
    }

    public NetClient Connection { get; }
}