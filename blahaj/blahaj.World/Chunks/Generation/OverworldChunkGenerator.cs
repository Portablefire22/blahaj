using System.Numerics;

namespace blahaj.blahaj.World.Chunks.Generation;

public class OverworldChunkGenerator : ChunkGenerator
{
    public OverworldChunkGenerator(int seed) : base(seed)
    {
    }

    public override Chunk Generate(KeyValuePair<int, int> pos)
    {
        var blocks = new int[16, 16, 384];

        FastNoiseLite noise = new FastNoiseLite(Seed);
        noise.SetFractalOctaves(8);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        var heightmap = new float[16, 16];
        
        noise.SetFractalGain(0.0005f);
        
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {
                var noiseValue = noise.GetNoise(x + (pos.Key * 16), z  + (pos.Value * 16f));
                noiseValue *= 30;
                
                noiseValue = Math.Clamp(noiseValue, -30, 258);

                var height = 126f + noiseValue;
                
                height = MathF.Floor(height);
                heightmap[x, z] = height;
                
                var intHeight = (int)height;

                blocks[15 - x,z, intHeight] = 9;
                
                for (int y = 0; y < intHeight; y++)
                {
                    blocks[15- x,z,y] = 1;
                }
                
            }
        }

        var chunk = new Chunk(new Vector2(pos.Key, pos.Value), 384)
        {
            Blocks =  blocks,
        };
        return chunk;
    }
}