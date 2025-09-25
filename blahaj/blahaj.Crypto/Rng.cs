namespace blahaj.blahaj.Crypto;

public class Rng
{
    public static Random Random
    {
        get;
        private set;
    }

    public Rng()
    {
        Random = new Random(DateTime.Now.Nanosecond);
    }
}