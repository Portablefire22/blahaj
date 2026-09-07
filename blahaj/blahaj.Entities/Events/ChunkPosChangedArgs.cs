using System.Numerics;
using blahaj.blahaj.World.Chunks;

namespace blahaj.blahaj.Entities.Events;

public class ChunkPosChangedArgs
{
    public ChunkPosChangedArgs(ChunkPos chunkPos, ChunkPos oldChunkPos)
    {
        ChunkPos = chunkPos;
        OldChunkPos = oldChunkPos;
    }

    public ChunkPos OldChunkPos { get; protected set; }
    public ChunkPos ChunkPos { get; protected set; }
}