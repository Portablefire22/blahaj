using System.Numerics;
using blahaj.blahaj.Network.Packets.Play.ToClient;

namespace blahaj.blahaj.World.Chunks;

/// <summary>
/// A 16x16xWorldHeight 
/// </summary>
public class Chunk : IDisposable
{
    public int[,,] Blocks { get; set; }

    /// <summary>
    /// Number of entities that are loading this chunk
    /// </summary>
    public int LoaderEntities { get; set; } = 0;
    
    public int[,] HeightMap { get; set; } = new int[16,16];
    
    public ChunkPos ChunkPosition { get; set; }
    
    public int WorldHeight { get; private set; }

    public Chunk(ChunkPos chunkPosition, int worldHeight)
    {
        Blocks = new int[16, 16, worldHeight];
        ChunkPosition = chunkPosition;
        WorldHeight = worldHeight;
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public void Tick()
    {
        
    }

    public ChunkSection[] Split()
    {
        var list = new List<ChunkSection>();

        for (int section = 0; section < WorldHeight / 16; section++)
        {
            var chunkSection = new ChunkSection();
            chunkSection.BlockStates.BitsPerEntry = 4;
            chunkSection.BlockStates.Data = new byte[2048];
            var palette = new List<int>();
            var left = true;

            var i = 0; 
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        var block = Blocks[x, z, y + section * 16];
                        if (!palette.Contains(block)) palette.Add(block);
                       
                        var index = (byte)palette.IndexOf(block);

                        if (left)
                        {
                            chunkSection.BlockStates.Data[i] = (byte)(index << 4);
                        }
                        else
                        {
                            chunkSection.BlockStates.Data[i] |= index;
                            i++;
                        }
                        left = !left;
                        
                    }
                }
            }
            chunkSection.BlockStates.Palette = palette.ToArray();

            if (palette.Count == 1)
            {
                chunkSection.BlockStates.BitsPerEntry = 0;
                chunkSection.BlockStates.Data = null;
                chunkSection.BlockStates.Palette = [palette.Last()];
            }
            
            list.Add(chunkSection);
        }
        return [.. list];
    }
    
}