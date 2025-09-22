using System.Net.Sockets;

namespace blahaj.blahaj.Stream;

public class MinecraftStream : IDisposable,MinecraftWriter, MinecraftReader
{
    private NetworkStream NetworkStream { get; }

    public MinecraftStream(NetworkStream ns)
    {
        NetworkStream = ns;
    }
    public void Dispose()
    {
        NetworkStream.Dispose();
    }

    public bool WriteBool()
    {
        throw new NotImplementedException();
    }

    public sbyte WriteByte()
    {
        throw new NotImplementedException();
    }

    public byte WriteUnsignedByte()
    {
        throw new NotImplementedException();
    }

    public short WriteShort()
    {
        throw new NotImplementedException();
    }

    public ushort WriteUnsignedShort()
    {
        throw new NotImplementedException();
    }

    public int WriteInt()
    {
        throw new NotImplementedException();
    }

    public long WriteLong()
    {
        throw new NotImplementedException();
    }

    public float WriteFloat()
    {
        throw new NotImplementedException();
    }

    public double WriteDouble()
    {
        throw new NotImplementedException();
    }

    public string WriteString()
    {
        throw new NotImplementedException();
    }

    public object WriteTextComponent()
    {
        throw new NotImplementedException();
    }

    public object WriteJsonTextComponent()
    {
        throw new NotImplementedException();
    }

    public object WriteIdentifier()
    {
        throw new NotImplementedException();
    }

    public int WriteVarInt()
    {
        throw new NotImplementedException();
    }

    public int WriteVarLong()
    {
        throw new NotImplementedException();
    }

    public object WriteEntityMetadata()
    {
        throw new NotImplementedException();
    }

    public object WriteSlot()
    {
        throw new NotImplementedException();
    }

    public object WriteHashedSlot()
    {
        throw new NotImplementedException();
    }

    public object WriteNbt()
    {
        throw new NotImplementedException();
    }

    public object WritePosition()
    {
        throw new NotImplementedException();
    }

    public Guid WriteUuid()
    {
        throw new NotImplementedException();
    }

    public object WriteBitSet()
    {
        throw new NotImplementedException();
    }

    public object WriteFixedBitSet()
    {
        throw new NotImplementedException();
    }

    public T? WriteOptional<T>()
    {
        throw new NotImplementedException();
    }

    public T? WritePrefixedOptional<T>()
    {
        throw new NotImplementedException();
    }

    public T[] WriteArray<T>()
    {
        throw new NotImplementedException();
    }

    public T WriteEnum<T>()
    {
        throw new NotImplementedException();
    }

    public T[] WriteEnumSet<T>()
    {
        throw new NotImplementedException();
    }

    public byte[] WriteByteArray()
    {
        throw new NotImplementedException();
    }

    public object WriteId()
    {
        throw new NotImplementedException();
    }

    public object[] WriteIdSet()
    {
        throw new NotImplementedException();
    }

    public object WriteSoundEvent()
    {
        throw new NotImplementedException();
    }

    public object WriteChatType()
    {
        throw new NotImplementedException();
    }

    public object WriteTeleportFlags()
    {
        throw new NotImplementedException();
    }

    public object WriteRecipeDisplay()
    {
        throw new NotImplementedException();
    }

    public object WriteSlotDisplay()
    {
        throw new NotImplementedException();
    }

    public object WriteChunkData()
    {
        throw new NotImplementedException();
    }

    public object WriteLightData()
    {
        throw new NotImplementedException();
    }

    public object WriteOr<A, B>()
    {
        throw new NotImplementedException();
    }

    public bool ReadBool()
    {
        throw new NotImplementedException();
    }

    public sbyte ReadByte()
    {
        throw new NotImplementedException();
    }

    public byte ReadUnsignedByte()
    {
        throw new NotImplementedException();
    }

    public short ReadShort()
    {
        throw new NotImplementedException();
    }

    public ushort ReadUnsignedShort()
    {
        throw new NotImplementedException();
    }

    public int ReadInt()
    {
        throw new NotImplementedException();
    }

    public long ReadLong()
    {
        throw new NotImplementedException();
    }

    public float ReadFloat()
    {
        throw new NotImplementedException();
    }

    public double ReadDouble()
    {
        throw new NotImplementedException();
    }

    public string ReadString()
    {
        throw new NotImplementedException();
    }

    public object ReadTextComponent()
    {
        throw new NotImplementedException();
    }

    public object ReadJsonTextComponent()
    {
        throw new NotImplementedException();
    }

    public object ReadIdentifier()
    {
        throw new NotImplementedException();
    }

    public int ReadVarInt()
    {
        throw new NotImplementedException();
    }

    public int ReadVarLong()
    {
        throw new NotImplementedException();
    }

    public object ReadEntityMetadata()
    {
        throw new NotImplementedException();
    }

    public object ReadSlot()
    {
        throw new NotImplementedException();
    }

    public object ReadHashedSlot()
    {
        throw new NotImplementedException();
    }

    public object ReadNbt()
    {
        throw new NotImplementedException();
    }

    public object ReadPosition()
    {
        throw new NotImplementedException();
    }

    public Guid ReadUuid()
    {
        throw new NotImplementedException();
    }

    public object ReadBitSet()
    {
        throw new NotImplementedException();
    }

    public object ReadFixedBitSet()
    {
        throw new NotImplementedException();
    }

    public T? ReadOptional<T>()
    {
        throw new NotImplementedException();
    }

    public T? ReadPrefixedOptional<T>()
    {
        throw new NotImplementedException();
    }

    public T[] ReadArray<T>()
    {
        throw new NotImplementedException();
    }

    public T ReadEnum<T>()
    {
        throw new NotImplementedException();
    }

    public T[] ReadEnumSet<T>()
    {
        throw new NotImplementedException();
    }

    public byte[] ReadByteArray()
    {
        throw new NotImplementedException();
    }

    public object ReadId()
    {
        throw new NotImplementedException();
    }

    public object[] ReadIdSet()
    {
        throw new NotImplementedException();
    }

    public object ReadSoundEvent()
    {
        throw new NotImplementedException();
    }

    public object ReadChatType()
    {
        throw new NotImplementedException();
    }

    public object ReadTeleportFlags()
    {
        throw new NotImplementedException();
    }

    public object ReadRecipeDisplay()
    {
        throw new NotImplementedException();
    }

    public object ReadSlotDisplay()
    {
        throw new NotImplementedException();
    }

    public object ReadChunkData()
    {
        throw new NotImplementedException();
    }

    public object ReadLightData()
    {
        throw new NotImplementedException();
    }

    public object ReadOr<A, B>()
    {
        throw new NotImplementedException();
    }
}