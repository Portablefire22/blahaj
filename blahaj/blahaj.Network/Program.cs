namespace blahaj.Network;

class Program
{
    static void Main(string[] args)
    {
        var server = new NetServer();
        server.Run();
        while(true) {Thread.Sleep(5000);}
    }
}