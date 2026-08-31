namespace blahaj.blahaj.World.Actions;

public class WorldAction
{
    public WorldAction(ActionType type, object? data)
    {
        Type = type;
        Data = data;
    }

    public ActionType Type { get; set; }
    public object? Data { get; set; }
}