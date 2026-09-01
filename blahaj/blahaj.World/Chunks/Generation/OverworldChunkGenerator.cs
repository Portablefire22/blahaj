using System.Numerics;

namespace blahaj.blahaj.World.Chunks.Generation;

public class OverworldChunkGenerator : ChunkGenerator
{
    public OverworldChunkGenerator(int seed) : base(seed)
    {
    }

    public int SeaLevel { get; set; } = 124;
    
    public override Chunk Generate(KeyValuePair<int, int> pos)
    {
        var blocks = new int[16, 16, 384];

        FastNoiseLite noise = new FastNoiseLite(Seed);
        noise.SetFractalOctaves(4);
        //noise.SetDomainWarpAmp(150);
        noise.SetFrequency(0.01f);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        var heightmap = new float[16, 16];


        var scale = 1f;
        
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {

                var scaledX = x + (pos.Key * 16);
                var scaledZ = z + (pos.Value * 16);
                
                var noiseValue = noise.GetNoise(scaledX * scale,  scaledZ * scale);
                noiseValue *= 20;
                
                //var height = 126f + noiseValue;

                var height = GetContinental(scaledX, scaledZ) + noiseValue;
                
                height = MathF.Floor(height);
                heightmap[x, z] = height;
                
                var intHeight = (int)height;

                blocks[15 - x,z, intHeight] = 9;
                
                for (int y = 0; y < Math.Max(intHeight, SeaLevel); y++)
                {

                    if (y > intHeight)
                    {
                        blocks[15 - x, z, y] = 86;
                    } else if (intHeight - y< 3)
                    {
                        blocks[15 - x, z, y] = 10;
                    } else if (y <= intHeight)
                    {
                        blocks[15 - x, z, y] = 1;
                    }
                }
            }
        }
        var chunk = new Chunk(new Vector2(pos.Key, pos.Value), 384)
        {
            Blocks =  blocks,
        };
        return chunk;
    }

    private float GetContinental(int x, int z)
    {
        FastNoiseLite noise = new FastNoiseLite(Seed);
        noise.SetFractalOctaves(3);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFrequency(0.01f);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        
        noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        noise.SetDomainWarpAmp(5);

        float scale = 0.05f;
        
        var value = noise.GetNoise(x * scale, z * scale);

        var lerpFactor = MathF.Abs((value + 1) / 2f);
        var lerpValue = 0.5f;
        var normalSea = SeaLevel - 63;
        if (value <= -.25)
        {
            //lerpValue = float.Lerp(50, 100, lerpFactor);
            lerpValue = GetPoint(-1, -.25f, 0, normalSea, value);
        } else if (value <= 0.4)
        {
            //lerpValue = float.Lerp(100, 150, lerpFactor);
            lerpValue = GetPoint(-.25f, 0.4f, normalSea, 80, value);
        } else if (value <= 0.8)
        {
            lerpValue = GetPoint(0.4f, 0.8f, 80, 230, value);
        }
        else
        {
            //lerpValue = float.Lerp(150, 200,  lerpFactor);
            lerpValue = GetPoint(0.8f, 1f, 230, 275, value);
        }

        lerpValue += 63;
        return lerpValue;
    }


    private float GetPoint(float start, float stop, float min, float max, float value)
    {
        var width = stop - start;
        var diff = max - min;
        var dx = diff / width;
        var y = max - (dx * stop);
        return y + (value * dx);
    }
}