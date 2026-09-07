using System.Numerics;

namespace blahaj.blahaj.World.Chunks.Generation;

public class DebugChunkGenerator : ChunkGenerator
{
    public DebugChunkGenerator(int seed) : base(seed)
    {
    }

    public override Chunk Generate(ChunkPos pos)
    {

        var blocks = new int[16, 16, 384];
        var random = new Random();

        for (int y = 0; y < 384; y++)
        {
            for (int z = 0; z < 16; z++)
            {
                if (z < 1 || z > 15) continue;
                for (int x = 0; x < 16; x++)
                {
                    if (y == 125)
                    {
                        if (x < 2 || x > 13)
                        {
                            blocks[x, z, y] = 0;
                        }
                        else if (x < 4 || x > 11)
                        {
                            blocks[x, z, y] = 15033;
                        } else if (x < 6 || x > 9)
                        {
                            blocks[x, z, y] = 15036;
                        }
                        else
                        {
                            blocks[x, z, y] = 15030;
                        }
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
        
        var chunk = new Chunk(pos, 384)
        {
           Blocks =  blocks,
        };
        return chunk;
    }
}