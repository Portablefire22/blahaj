using System.Text.Json;
using blahaj.blahaj.Registry.Data.DamageType;

namespace blahaj.blahaj.Registry;

public static class RegistryController
{
    public static DamageType[] DamageTypes { get; private set; } = [];
    
    
    public static RegistryData DamageTypeRegistry { get; private set; }
    
    
    public static string RegistryPath { get; } = "Minecraft/registry/";

    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
    
    public static void Initialise()
    {
       InitDamageTypes(); 
    }

    private static void InitDamageTypes()
    {
        var files = Directory.GetFiles(Path.Combine(RegistryPath, "damage_type/"), "*.json");
        var temp = new List<DamageType>();
        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var damageType = JsonSerializer.Deserialize<DamageType>(json, _jsonSerializerOptions);
            if (damageType == null) continue;
            var name = file.Substring(file.LastIndexOf('/') + 1).Replace(".json", "");
            damageType.Identifier = $"minecraft:{name}"; 
            temp.Add(damageType);
        }
        DamageTypes = [.. temp];
        DamageTypeRegistry = new RegistryData("minecraft:damage_type", DamageTypes);
    }
}