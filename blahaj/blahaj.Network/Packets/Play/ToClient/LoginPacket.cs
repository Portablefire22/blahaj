using System.Numerics;
using blahaj.blahaj.Stream;
using blahaj.blahaj.World;

namespace blahaj.Network.Packets.Play.ToClient;

public class LoginPacket : Packet
{
    public int EntityId { get; private set; }

    public WorldSettings Settings;

    public string DimensionName { get; private set; } = "overworld";
    public bool HasDeathLocation { get; private set; }
    public string? DeathDimensionName { get; private set; }
    public Vector3? DeathLocation { get; private set; }
    public byte GameMode { get; private set; }
    public byte PreviousGameMode { get; private set; }
    public LoginPacket() :  base(0x31) {}

    public LoginPacket(WorldSettings settings, int entityId, string dimensionName, 
        bool hasDeathLocation, string? deathDimensionName, Vector3? deathLocation, 
        byte gameMode, byte previousGameMode) : this()
    {
        Settings = settings;
        EntityId = entityId;
        DimensionName = dimensionName;
        HasDeathLocation = hasDeathLocation;
        DeathDimensionName = deathDimensionName;
        DeathLocation = deathLocation;
        GameMode = gameMode;
        PreviousGameMode = previousGameMode;
    }

    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteInt(EntityId);
        stream.WriteBool(Settings.IsHardcore);
        stream.WriteVarInt(Settings.DimensionNames.Length);
        foreach (var dimension in Settings.DimensionNames)
        {
            stream.WriteString(dimension);
        }
        stream.WriteVarInt(Settings.MaxPlayers);
        stream.WriteVarInt(Settings.ViewDistance);
        stream.WriteVarInt(Settings.SimulationDistance);
        stream.WriteBool(Settings.ReducedDebugInfo);
        stream.WriteBool(Settings.EnableRespawnScreen);
        stream.WriteBool(Settings.DoLimitedCrafting);
        stream.WriteVarInt(Settings.DimensionType);
        stream.WriteString(DimensionName);
        stream.WriteLong(Settings.HashedSeed);
        stream.WriteUnsignedByte(Settings.DefaultGameMode);
        stream.WriteByte((sbyte)Settings.DefaultGameMode);
        stream.WriteBool(Settings.IsDebug);
        stream.WriteBool(Settings.IsFlat);
        stream.WriteBool(HasDeathLocation);
        if (HasDeathLocation)
        {
            stream.WriteString(DeathDimensionName);
        }
        stream.WriteVarInt(Settings.PortalCooldown);
        stream.WriteVarInt(Settings.SeaLevel);
        stream.WriteBool(Settings.OnlineMode);
        stream.WriteBool(Settings.EnforcesSecureChat);
    }
}