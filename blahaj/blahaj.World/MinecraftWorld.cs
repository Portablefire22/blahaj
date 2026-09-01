using blahaj.blahaj.Network;

namespace blahaj.blahaj.World;


/// <summary>
/// Class for everything in a Minecraft World, including dimensions
/// </summary>
public class MinecraftWorld
{
    public List<MinecraftDimension> Dimensions { get; set; }
    
    public WorldSettings WorldSettings { get; set; }

    private NetServer _server;
    
    public MinecraftWorld(NetServer server, WorldSettings worldSettings)
    {
        WorldSettings = worldSettings;
        _server = server;
        
        Dimensions =  [new MinecraftDimension(_server, this, "minecraft:overworld", 0)
        {
            IsDefault = true
        }];
    }

    public void Start()
    {
        foreach (var dimension in Dimensions)
        {
            dimension.Start();
        }
    }
}