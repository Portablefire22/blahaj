using blahaj.blahaj.Stream;
using blahaj.blahaj.World.Chunks;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

public class ChunkDataWithLight : Packet
{
    public ChunkDataWithLight(Chunk chunk) : base(0x2d)
    {
        ChunkX =  (int)chunk.ChunkPosition.X;
        ChunkZ = (int)chunk.ChunkPosition.Y;
        ChunkSections = chunk.Split();

        ShouldLog = false;
    }
    
    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    public object[] Heightmaps { get; set; } = [];
    public ChunkSection[] ChunkSections { get; set; } = new  ChunkSection[24];

    public object[] BlockEntities { get; set; } = [];
    
    
    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteInt(ChunkX);
        stream.WriteInt(ChunkZ);
        stream.WriteVarInt(Heightmaps.Length);
        foreach (var chunkSection in Heightmaps)
        {
            
        }

        using var ms = new MemoryStream();
        using var streamWriter = new MinecraftStream(ms);
        
        foreach (var chunkSection in ChunkSections)
        {
            chunkSection.Write(streamWriter);
        }
        
        stream.WriteVarInt((int)ms.Length);
        stream.WriteByteArray(ms.ToArray());
        
        stream.WriteVarInt(BlockEntities.Length);
        foreach (var blockEntity in BlockEntities)
        {
            
        }
        stream.WriteVarInt(0);
        stream.WriteVarInt(0);
        stream.WriteVarInt(0);
        stream.WriteVarInt(0);
        stream.WriteVarInt(0);
        stream.WriteVarInt(0);
    }
}

public class ChunkSection
{
    public short BlockCount { get; set; } = 4096;
    public short FluidCount { get; set; } = 0;
    public PalettedContainer BlockStates { get; set; } = new PalettedContainer();
    public PalettedContainer Biomes { get; set; } =
        new PalettedContainer()
        {
            BitsPerEntry = 0,
            Palette = [0],
            Data = null//[0xCCFFCCFFCCFFCCFF]
        };

    public void Write(MinecraftStream writer)
    {
        writer.WriteShort(BlockCount);
        writer.WriteShort(FluidCount);
        BlockStates.Write(writer); 
        Biomes.Write(writer);
    }
}

public class PalettedContainer
{
    public byte BitsPerEntry { get; set; } = 0;
    public byte[] Palette { get; set; } = [1];
    public byte[] Data { get; set; } = null;

    public void Write(MinecraftStream writer)
    {
        writer.WriteUnsignedByte(BitsPerEntry);
        if (BitsPerEntry > 0)
        {
            writer.WriteVarInt(Palette.Length);
        }

        foreach (var palette in Palette)
        {
            writer.WriteUnsignedByte(palette);
        }

        if (Data != null)
        {
            foreach (var val in Data)
            {
                writer.WriteUnsignedByte(val);
            }
        }
    }
}