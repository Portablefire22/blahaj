using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

public class ChunkDataWithLight : Packet
{
    public ChunkDataWithLight(int chunkX, int chunkZ) : base(0x2d)
    {
        ChunkZ =  chunkZ;
        ChunkX = chunkX;
        for (int i = 0; i < 24; i++)
        {
            var chunk = new ChunkSection();
                chunk.BlockStates[0].Palette = [(byte)1];
            ChunkSections[i] = chunk;
        }
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
    public PalettedContainer[] BlockStates { get; set; } = new []
    {
        new PalettedContainer()
    };
    public PalettedContainer[] Biomes { get; set; } = new []
    {
        new PalettedContainer()
        {
            BitsPerEntry = 0,
            Palette = [0x0]
        }
    };

    public void Write(MinecraftStream writer)
    {
        writer.WriteShort(BlockCount);
        writer.WriteShort(FluidCount);
        foreach (var block in BlockStates)
        {
           block.Write(writer); 
        }
        foreach (var biome in Biomes)
        {
            biome.Write(writer);
        }
    }
}

public class PalettedContainer
{
    public byte BitsPerEntry { get; set; } = 0;
    public byte[] Palette { get; set; } = [1];
    public ulong? Data { get; set; } = null;

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
            writer.WriteLong((long)Data.Value);
        }
    }
}