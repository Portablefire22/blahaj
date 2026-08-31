using System.Numerics;

namespace blahaj.blahaj.World.Chunks.Generation;

public class OverworldChunkGenerator : ChunkGenerator
{
    public OverworldChunkGenerator(long seed) : base(seed)
    {
    }

    public override Chunk Generate(KeyValuePair<int, int> pos)
    {

        var blocks = new byte[16, 16, 384];
        var random = new Random();

        for (int y = 0; y < 384; y++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (y == 125)
                    {
                        blocks[x, z, y] = (byte)(9);
                    }
                    else
                    {
                        blocks[x, z, y] = (byte)(0);
                    }
                }
            }
        }

        /*
        for (int i = 0; i < 16 * 16 * 384; i++)
        {
            if (i / 256f < 1f)
            {
                blocks[i / 16, i % 16, 0] = 1;
            }
            else
            {
                blocks[(i / 15) % 15, i % 15, 1] = 0;
            }
        }*/
        
        var chunk = new Chunk(new Vector2(pos.Key, pos.Value), 384)
        {
           Blocks =  blocks,
        };
        return chunk;
    }
}