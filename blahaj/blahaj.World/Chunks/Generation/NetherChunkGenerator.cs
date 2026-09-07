using System.Numerics;

namespace blahaj.blahaj.World.Chunks.Generation;

public class NetherChunkGenerator : ChunkGenerator
{
    public NetherChunkGenerator(int seed) : base(seed)
    {
    }

    public override Chunk Generate(ChunkPos pos)
    {
        var blocks = new int[16, 16, 384];

        FastNoiseLite netherNoise = new FastNoiseLite(Seed);
        netherNoise.SetFractalOctaves(8);
        netherNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        //var blocks = new float[16, 16, 384];
        netherNoise.SetFractalGain(5f);

        
        FastNoiseLite noise = new FastNoiseLite(Seed);
        noise.SetFractalOctaves(8);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        var heightmap = new float[16, 16];
        
        noise.SetFractalGain(0.0005f);
        
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {
                var noiseValue = noise.GetNoise(x + (pos.X * 16), z  + (pos.Z * 16f));
                noiseValue *= 30;
                
                noiseValue = Math.Clamp(noiseValue, -30, 258);

                var height = 126f + noiseValue;
                
                height = MathF.Floor(height);
                heightmap[x, z] = height;
                
                var intHeight = (int)height;

                blocks[15 - x,z, intHeight] = 9;
                
                for (int y = 0; y < intHeight; y++)
                {
                    if (intHeight - y< 3)
                    {
                        blocks[15 - x, z, y] = 10;
                    } else blocks[15- x,z,y] = 1;
                }
                
            }
        }
        
        
        
        
        
        for (int y = 0; y < 384; y++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (heightmap[x, z] < y) continue;
                    var noiseValue = netherNoise.GetNoise(x + (pos.X * 16), y, z + (pos.Z * 16f));

                    noiseValue *= 0.5f;
                    
                    if (noiseValue > 0.25f)
                    {
                        blocks[15 - x, z, y] = 0;
                    }
                }
            }
        }

        var chunk = new Chunk(pos, 384)
        {
            Blocks =  blocks,
        };
        return chunk;
    }
}