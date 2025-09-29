using System.Text;

namespace blahaj.blahaj.World;

public class WorldSettings
{
   public bool IsHardcore { get; private set; } 
   // All dimension identifiers
   public string[] DimensionNames { get; private set; }
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
}