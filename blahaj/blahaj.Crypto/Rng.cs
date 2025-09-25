namespace blahaj.blahaj.Crypto;

public static class Rng
{
    public static Random Random
    {
        get;
        private set;
    }

    static Rng()
    {
        Random = new Random(DateTime.Now.Nanosecond);
    }
}