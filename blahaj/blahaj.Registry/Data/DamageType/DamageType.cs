using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.DamageType;

public class DamageType : IRegistryEntry
{
   public DamageType(string messageId, float exhaustion, string scaling, 
      string? effects = null, string? deathMessageType = null)
   {
      MessageId = messageId;
      Exhaustion = exhaustion;
      Scaling = scaling;
      Effects = effects;
      DeathMessageType = deathMessageType;
   }

   public required string MessageId { get; set; }
   public string? Identifier { get; set; }
   public float Exhaustion { get; set; }

   public required string Scaling
   {
      get;
      set
      {
         if (ValidScaling.Contains(value))
         {
            field = value;
         }
         else
         {
            throw new ArgumentException($"Invalid scaling value: {value}");
         }
      }
   }

   public string? Effects
   {
      get;
      set
      {
         if (ValidEffects.Contains(value) || value == null)
         {
            field = value;
         }
         else
         {
            throw new ArgumentException($"Invalid effect value: {value}");
         }
      }
   }

   public string? DeathMessageType
   {
      get;
      set
      {
         if (ValidDeathMessageTypes.Contains(value) || value == null)
         {
            field = value;
         }
         else
         {
            throw new ArgumentException($"Invalid death message value: {value}");
         }
      }
   }

   public static string[] ValidScaling { get; } = new[]
   {
      "never",
      "always",
      "when_caused_by_living_non_player"
   };
   
   public static string[] ValidEffects { get; } = new[]
   {
      "hurt",
      "thorns",
      "drowning",
      "burning",
      "poking",
      "freezing"
   };

   public static string[] ValidDeathMessageTypes { get; } = new[]
   {
      "default",
      "fall_variants",
      "intentional_game_design"
   };

   public void Write(MinecraftStream writer)
   {
      writer.WriteString(Identifier ?? "");
      var buffer = new ArrayBufferWriter<byte>();
      var msWriter = new TagWriter(buffer, false, true);
       
      writer.WriteBool(true);
      
      var comp = CompoundTagBuilder.Create().AddString(MessageId, "message_id")
            .AddFloat(Exhaustion, "exhaustion")
            .AddString(Scaling, "scaling");
      
      if (Effects != null) comp.AddString(Effects, "effects");
      if (DeathMessageType != null) comp.AddString(DeathMessageType, "death_message_type");
      var result = comp.Build();

      TagSerializer.Serialize(buffer, result, new TagSerializerOptions()
      {
         Network = true
      });
      writer.WriteByteArray([.. buffer.WrittenSpan]);
   }

   public IRegistryEntry Read(MinecraftStream writer)
   {
      throw new NotImplementedException();
   }
}