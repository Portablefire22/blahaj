using System.Net.Sockets;
using System.Text;

namespace blahaj.blahaj.Stream;

public class MinecraftStream : IDisposable,MinecraftWriter, MinecraftReader
{
    private System.IO.Stream Stream { get; }

    public MinecraftStream(System.IO.Stream ns)
    {
        Stream = ns;
    }
    public void Dispose()
    {
        Stream.Dispose();
    }

    public bool WriteBool()
    {
        throw new NotImplementedException();
    }

    public void WriteByte(sbyte b)
    {
        Stream.WriteByte((byte) b);
    }

    public void WriteUnsignedByte(byte b)
    {
        Stream.WriteByte(b);
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

    public void WriteString(string str)
    {
        WriteVarInt(str.Length);
        WriteByteArray(Encoding.UTF8.GetBytes(str));
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

    public void WriteVarInt(int val)
    {
        while (true)
        {
            if ((val & ~0x7f) == 0)
            {
                Stream.WriteByte((byte)val);
                break;
            }
            Stream.WriteByte((byte) ((val & 0x7f) | 0x80));
            val >>>= 7;
        }
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

    public void WriteByteArray(byte[] arr)
    {
        Stream.Write(arr);
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
        return (byte) Stream.ReadByte();
    }

    public short ReadShort()
    {
        var dat = ReadByteArray(2);
        return BitConverter.ToInt16(dat);
    }

    public ushort ReadUnsignedShort()
    {
        var dat = ReadByteArray(2);
        return BitConverter.ToUInt16(dat);
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
        var length = ReadVarInt();
        var dat = ReadByteArray(length);
        return Encoding.UTF8.GetString(dat);
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
        int result = 0, shift = 0;
        var size = sizeof(int);
        byte b;
        do
        {
            b = ReadUnsignedByte();
            result |= (b & 0x7f) << shift;
            shift += 7;
        } while ((b & 0x80) != 0);

        if ((shift < size) && ((b & 0x40) != 0))
        {
            result |= (~0 << shift);
        }
        return result;
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

    public byte[] ReadByteArray(int count)
    {
        var data = new byte[count];
        Stream.ReadExactly(data, 0, count);
        return data;
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