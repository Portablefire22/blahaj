using System.Numerics;

namespace blahaj.blahaj.World.Chunks.Generation;

public class OverworldChunkGenerator : ChunkGenerator
{
    public OverworldChunkGenerator(int seed) : base(seed)
    {
    }

    public int SeaLevel { get; set; } = 124;
    
    public override Chunk Generate(ChunkPos pos)
    {
        var blocks = new int[16, 16, 384];

        FastNoiseLite noise = new FastNoiseLite(Seed);
        noise.SetFractalOctaves(4);
        //noise.SetDomainWarpAmp(150);
        noise.SetFrequency(0.01f);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        var heightmap = new float[16, 16];
    
        var blockHeights = new int[16, 16];

        var scale = 1f;// 0.001f;
        
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {

                var scaledX = x + (pos.X * 16);
                var scaledZ = z + (pos.Z * 16);
                
                var noiseValue = noise.GetNoise(scaledX * scale,  scaledZ * scale);
                noiseValue *= 20;
                
                //var height = 126f + noiseValue;

                var height = GetContinental(scaledX, scaledZ) + noiseValue;
                
                height = MathF.Floor(height);
                heightmap[x, z] = height;
                
                var intHeight = (int)height;

                blocks[15 - x,z, intHeight] = 9;

                blockHeights[15 - x, z] = Math.Max(intHeight, SeaLevel);
                
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
        var chunk = new Chunk(pos, 384)
        {
            Blocks =  blocks,
            HeightMap = blockHeights
        };
        return chunk;
    }

    private float GetCave(int x, int y, int z)
    {
        FastNoiseLite noise = new FastNoiseLite(Seed);
        /*noise.SetFractalOctaves(3);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);*/
        noise.SetFrequency(0.01f);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        
        /*
        noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        */
        /*
        noise.SetDomainWarpAmp(5);
        */

        float scale = 1f;// * 0.001f;
        
        var value = noise.GetNoise(x * scale, y * scale, z * scale);

        var lerpValue = 0.5f;

        var spline = new Spline(
        [
            /*new SplinePoint(-0.2f, 0),
            new SplinePoint(-0.25f, 1),
            /*new SplinePoint(-0.3f, 0),
            
            new SplinePoint(0.2f, 0),#1#
            new SplinePoint(0.25f, 1),
            new SplinePoint(0.3f, 0),*/
            
            
            new SplinePoint(-0.25f, 0),
            new SplinePoint(-0.3f, 1),

            new SplinePoint(0.2f, 1),
            new SplinePoint(0.25f, 0),
        ]);
        
        lerpValue = spline.GetValue(value);
        return lerpValue;
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

        float scale = 0.5f;// * 0.001f;
        
        var value = noise.GetNoise(x * scale, z * scale);

        var lerpFactor = MathF.Abs((value + 1) / 2f);
        var lerpValue = 0.5f;
        var normalSea = SeaLevel - 63;

        var spline = new Spline(
            [
                new SplinePoint(-1, -10),
                new SplinePoint(-0.5f, normalSea / 2),
            new SplinePoint(-0.25f, normalSea),
            new SplinePoint(0.4f, 80),
            new SplinePoint(0.8f, 100),
            new SplinePoint(1f, 275),
            
        ]);
        
        /*if (value <= -.25)
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
        }*/

        lerpValue = spline.GetValue(value);
        
        lerpValue += 63;
        return lerpValue;
    }
    
    
    

    
}

public class Spline
{
    public Spline(SplinePoint[] points)
    {
        Points = points;
    }

    public SplinePoint[] Points { get; set; }

    public float GetValue(float value)
    {
        // Figure out which bounds of the spline we're in

        SplinePoint start = new SplinePoint(-1,0);
        SplinePoint end = new SplinePoint(1,0); 

        if (Math.Abs(Points[0].Position + 1f) < 0.01f)
        {
            start = new SplinePoint(-1, Points[0].Value);    
        }

        if (Math.Abs(Points[^1].Position - 1f) < 0.01f)
        {
            end = new SplinePoint(1, Points[^1].Value);
        }

        bool foundEnd = false;
        
        for (int i = 0; i < Points.Length; i++)
        {
            if (Points[i].Position <=  value)
            {
                start = Points[i];  
            }

            if (i + 1 >= Points.Length) continue;
            
            if (!foundEnd && Points[i + 1].Position >= value)
            {
                end = Points[i + 1];
                foundEnd = true;
            }
        }
        
        return GetPoint(start.Position, end.Position, start.Value, end.Value, value); 
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

public class SplinePoint
{
    public float Position;
    public float Value;

    public SplinePoint(float position, float value)
    {
        Position = position;
        Value = value;
    }
}
