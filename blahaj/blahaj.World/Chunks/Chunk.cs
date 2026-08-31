using System.Numerics;

namespace blahaj.blahaj.World.Chunks;

/// <summary>
/// A 16x16xWorldHeight 
/// </summary>
public class Chunk : IDisposable
{
    public byte[,,] Blocks { private get; set; }
    
    public Vector2 ChunkPosition { get; set; }

    public Chunk(Vector2 chunkPosition, int worldHeight)
    {
        Blocks = new byte[16, 16, worldHeight];
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public void Tick()
    {
        
    }
}