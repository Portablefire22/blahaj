using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace blahaj.blahaj.World;

public class WorldSettings
{
   // Todo stick this in some static class that loads on start
   public static WorldSettings FromConfig(IConfigurationRoot config)
   {
      var root = config.GetSection("worldSettings");
      return new WorldSettings(
         bool.Parse(root["hardcore"]),
         root.GetSection("dimensionNames").Get<string[]>(),
         int.Parse(config["maxPlayers"]),
         int.Parse(root["viewDistance"]),
         int.Parse(root["simulationDistance"]),
         bool.Parse(root["reducedDebugInfo"]),
         bool.Parse(root["enableRespawnScreen"]),
         bool.Parse(root["doLimitedCrafting"]),
         byte.Parse(root["defaultGameMode"]),
            1,
               long.Parse(root["seed"]),
         bool.Parse(root["isDebug"]),
         bool.Parse(root["isFlat"]),
         int.Parse(root["portalCooldown"]),
         int.Parse(root["seaLevel"]),
         bool.Parse(config["enforcesSecureChat"])
         );
   }
   public WorldSettings(bool isHardcore, string[] dimensionNames, int maxPlayers, int viewDistance, int simulationDistance, bool reducedDebugInfo, bool enableRespawnScreen, bool doLimitedCrafting, byte defaultGameMode, int dimensionType, long seed, bool isDebug, bool isFlat, int portalCooldown, int seaLevel, bool enforcesSecureChat)
   {
      IsHardcore = isHardcore;
      DimensionNames = dimensionNames;
      MaxPlayers = maxPlayers;
      ViewDistance = viewDistance;
      SimulationDistance = simulationDistance;
      ReducedDebugInfo = reducedDebugInfo;
      EnableRespawnScreen = enableRespawnScreen;
      DoLimitedCrafting = doLimitedCrafting;
      DefaultGameMode = defaultGameMode;
      DimensionType = dimensionType;
      Seed = seed;
      IsDebug = isDebug;
      IsFlat = isFlat;
      PortalCooldown = portalCooldown;
      SeaLevel = seaLevel;
      EnforcesSecureChat = enforcesSecureChat;
   }

   public bool IsHardcore { get; private set; } 
   // All dimension identifiers
   public string[] DimensionNames { get; private set; }
   public int MaxPlayers { get; private set; }
   public int ViewDistance { get; private set; }
   public int SimulationDistance { get; private set; }
   public bool ReducedDebugInfo { get; private set; }
   public bool EnableRespawnScreen { get; private set; }
   public bool DoLimitedCrafting { get; private set; }
   public byte DefaultGameMode { get; private set; }
   public int DimensionType { get; private set; }
   public long Seed { get; private set; }

   public long HashedSeed
   {
      get
      {
         var sha = MinecraftShaDigest.Sha(BitConverter.GetBytes(Seed));
         var bytes = Encoding.UTF8.GetBytes(sha);
         var l = BitConverter.ToInt64(bytes[new Range(0, 8)]);
         return l;
      }
   }

   public bool IsDebug { get; private set; } = false;
   public bool IsFlat { get; private set; } = false;
   // How many ticks need to elapse before a player can go through 
   // the portal they are currently inside of.
   public int PortalCooldown { get; private set; } = 150;
   public int SeaLevel { get; private set; } = 70;
   public bool EnforcesSecureChat { get; private set; } = false;
}