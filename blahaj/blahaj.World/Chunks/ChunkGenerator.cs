using blahaj.blahaj.World.Chunks.Generation;

namespace blahaj.blahaj.World.Chunks;

public abstract class ChunkGenerator
{
    protected ChunkGenerator(long seed)
    {
        Seed = seed;
    }

    public long Seed { get; }

    public Chunk Generate(int x, int z) => Generate(new KeyValuePair<int, int>(x, z));
    public abstract Chunk Generate(KeyValuePair<int, int> pos);
    
    
    public static ChunkGenerator FromIdentifier(string identifer, long seed)
    {
        switch (identifer)
        {
            default:
                return new OverworldChunkGenerator(seed);
        }
    }
}