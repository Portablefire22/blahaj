using blahaj.blahaj.World.Chunks.Generation;

namespace blahaj.blahaj.World.Chunks;

public abstract class ChunkGenerator
{
    protected ChunkGenerator(int seed)
    {
        Seed = seed;
    }

    public int Seed { get; }

    public Chunk Generate(int x, int z) => Generate(new (x, z));
    public abstract Chunk Generate(ChunkPos pos);
    
    
    public static ChunkGenerator FromIdentifier(string identifer, int seed)
    {
        switch (identifer)
        {
            case "minecraft:nether":
                return new NetherChunkGenerator(seed);
            case "minecraft:overworld":
                return new OverworldChunkGenerator(seed);
            default:
                return new DebugChunkGenerator(seed);
        }
    }
}