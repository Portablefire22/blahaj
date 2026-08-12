using System.Text.Json;
using blahaj.blahaj.Registry.Data.DamageType;
using blahaj.blahaj.Registry.Data.SoundVariants;
using blahaj.Network;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace blahaj.blahaj.Registry;

public static class RegistryController
{
    private static ILogger Logger { get; }
    static RegistryController()
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build
            #if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
            #endif
            .AddConsole());
        Logger = factory.CreateLogger<NetClient>();
    }

    public static DamageType[] DamageTypes { get; private set; } = [];
    
    
    public static RegistryData DamageTypeRegistry { get; private set; }
    
    public static List<RegistryData> VariantsRegistry { get; private set; } = [];
    
    
    public static string RegistryPath { get; } = "Minecraft/registry/";

    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
    
    public static void Initialise()
    {
        
        Logger.LogInformation("Initialising registry data..."); 
        InitDamageTypes(); 
        InitVariants();
        Logger.LogInformation("Finished Initialising registry data");
    }

    private static void InitDamageTypes()
    {
        Logger.LogDebug("Initialising damage types...");
        var files = Directory.GetFiles(Path.Combine(RegistryPath, "damage_type/"), "*.json");
        var temp = new List<DamageType>();
        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var damageType = JsonSerializer.Deserialize<DamageType>(json, _jsonSerializerOptions);
            if (damageType == null) continue;
            var name = file.Substring(file.LastIndexOf('/') + 1).Replace(".json", "");
            damageType.Identifier = $"minecraft:{name}";
            Logger.LogDebug($"Registered damage type {damageType.Identifier}");
            temp.Add(damageType);
        }
        DamageTypes = [.. temp];
        DamageTypeRegistry = new RegistryData("minecraft:damage_type", DamageTypes);
        Logger.LogDebug("Finished Initialising damage types");
    }

    private static void InitVariants()
    {
        Logger.LogDebug("Initialising variants...");
        InitVariantSounds();
        Logger.LogDebug("Finished Initialising variants");
    }

    private static void InitVariantSounds()
    {
        var directories = Directory.GetDirectories(RegistryPath, "*_sound_variant");
        foreach (var directory in directories)
        {
            var directoryName = directory.Substring(directory.LastIndexOf('/') + 1);
            var mob = directoryName.Replace("_sound_variant", "");
            Logger.LogDebug($"Registering Sounds for {mob} variants...");
            var files = Directory.GetFiles(directory, "*.json");
            var tmp = new List<SoundVariant>();
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);

                var variant =  JsonSerializer.Deserialize<SoundVariant>(json, _jsonSerializerOptions);
                if (variant == null) continue;

                
                // Cows are weird
                if (variant.BabySounds == null && variant.AdultSounds == null && variant.Sounds == null)
                {
                    var sound =  JsonSerializer.Deserialize<SoundEntry>(json, _jsonSerializerOptions);
                    variant.Sounds = sound;
                }
                
                var name = file.Substring(file.LastIndexOf('/') + 1).Replace(".json", "");
                variant.Identifier = $"minecraft:{name}";

                tmp.Add(variant);
                Logger.LogDebug($"Registered {mob} sound soundVariant {variant.Identifier}");
            }
            VariantsRegistry.Add(new RegistryData($"minecraft:{directoryName}", [.. tmp]));
        }
    }
}