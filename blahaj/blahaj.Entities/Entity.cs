using System.Numerics;
using blahaj.blahaj.Network;
using blahaj.blahaj.Network.Packets.Play.ToClient;

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

    public Vector3 Position { get; protected set; }
    public Vector2 Rotation { get; protected set; } = new Vector2(0, 90f);
    public Vector3 Velocity { get; protected set; }

    public NetServer Server { get; set; }
    
    public Guid EntityGuid { get; protected set; } =  Guid.NewGuid();
    public int Type { get; protected set; }

    public EntityMetadata Metadata { get; protected set; } = new ();
    
    public void UpdateMetadata()
    {
        Server.UpdateEntityMetadata(this);
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