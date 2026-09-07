using System.Numerics;
using blahaj.blahaj.Entities.Events;
using blahaj.blahaj.Network;
using blahaj.blahaj.Network.Packets.Play.ToClient;
using blahaj.blahaj.Network.Packets.Play.ToServer;
using blahaj.blahaj.World;
using blahaj.blahaj.World.Chunks;

namespace blahaj.blahaj.Entities;

public class Entity
{
    public Entity()
    {
    }

    public Entity(Vector3 position, int type)
    {
        Position = position;
        Type = type;
    }

    public Vector3 Position { get; protected set; } = new Vector3(float.NegativeInfinity);
    
    public ChunkPos ChunkPosition { get; protected set; }
    
    public Vector2 Rotation { get; protected set; } = new Vector2(0, 90f);
    public Vector3 Velocity { get; protected set; }

    public NetServer Server { get; set; }
    
    public Guid EntityGuid { get; protected set; } =  Guid.NewGuid();
    public int Type { get; protected set; }

    public MinecraftDimension? Dimension { get; protected set; }
    public EntityMetadata Metadata { get; protected set; } = new ();
    
    public EventHandler<ChunkPosChangedArgs> ChunkPosChanged;
    
    // Has a tick occured since being created?
    private bool _afterFirstTick = false;

    public Chunk GetChunk()
    {
        return Dimension!.GetChunk(ChunkPosition);
    }

    protected Chunk GetChunk(Vector2 position)
    {
        var chunkPos = new ChunkPos((int)position.X, (int)position.Y);
        return Dimension!.GetChunk(chunkPos);
    }
    
    public void UpdateMetadata()
    {
        Dimension?.UpdateEntityMetadata(this);
    }

    public virtual void Tick()
    {
        if (!_afterFirstTick) OnSpawn();
    }

    public virtual void OnSpawn()
    {
        _afterFirstTick = true;
    }
    
    public virtual void SetPosition(Vector3? position = null, Vector3? velocity = null, Vector2? rotation = null)
    {
        position ??= Position;
        velocity ??= Velocity;
        rotation ??= Rotation;
        
        var lastChunkX = Math.Floor(Position.X / 16);
        var lastChunkZ = Math.Floor(Position.Z / 16);
        var currentChunkX = Math.Floor(position.Value.X / 16);
        var currentChunkZ = Math.Floor(position.Value.Z / 16);

        var lastPos = Position;

        Position = (Vector3)position;
        Velocity = (Vector3)velocity;
        Rotation = (Vector2)rotation;
       
        if (Math.Abs(lastChunkX - currentChunkX) > 0.1 || Math.Abs(lastChunkZ - currentChunkZ) > 0.1)
        {
            var currentChunk = new ChunkPos((int)currentChunkX, (int)currentChunkZ);
            
            var oldChunk = new ChunkPos((int)lastChunkX, (int)lastChunkZ);
            
            ChunkPosChanged.Invoke(this, new ChunkPosChangedArgs(currentChunk, oldChunk));
        }
    }
    
    public int Id
    {
        get;
        set
        {
            if (field == 0)
            {
                field = value; 
            }
            else
            {
                throw new InvalidOperationException("Attempted to set Entity ID after " +
                                                    "assignment!");
            }
        }
    }
}