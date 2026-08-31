using System.Numerics;

namespace blahaj.blahaj.Entities.Events;

public class ChunkPosChangedArgs
{
    public ChunkPosChangedArgs(Vector2 chunkPos)
    {
        ChunkPos = chunkPos;
    }

    public Vector2 ChunkPos { get; protected set; }
}