using System.Numerics;

namespace blahaj.blahaj.Entities;

public class Entity
{
    public Vector3 Position { get; set; }
    public Vector2 Rotation { get; set; } = new Vector2(0, 90f);
    public Vector3 Velocity { get; set; }

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