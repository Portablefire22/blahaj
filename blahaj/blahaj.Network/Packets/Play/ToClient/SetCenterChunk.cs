using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

[PacketDirection(PacketDirection.ServerToClient)]
[PacketState(ConnectionState.Play)]
public class SetCenterChunk : Packet
{
    public SetCenterChunk() : base (0x5e)
    {
    }

    public SetCenterChunk(int chunkX, int chunkZ) : this()
    {
        ChunkX = chunkX;
        ChunkZ = chunkZ;
    }

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        ChunkX = stream.ReadVarInt();
        ChunkZ = stream.ReadVarInt();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteVarInt(ChunkX);
        stream.WriteVarInt(ChunkZ);
    }
}