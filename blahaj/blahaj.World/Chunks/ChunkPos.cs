namespace blahaj.blahaj.World.Chunks;

public class ChunkPos : IEquatable<ChunkPos>
{
    public ChunkPos(int x, int z)
    {
        X = x;
        Z = z;
    }

    public int X { get; set; }
    public int Z { get; set; }

    public bool Equals(ChunkPos? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return X == other.X && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ChunkPos)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }
}