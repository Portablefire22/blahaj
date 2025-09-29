using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace blahaj.blahaj.Stream;

public class MinecraftStream : IDisposable,MinecraftWriter, MinecraftReader
{
    private System.IO.Stream Stream { get; set; }

    public MinecraftStream(System.IO.Stream ns)
    {
        Stream = ns;
    }

    public void InitEncryption(byte[] key, bool isWriting)
    {
        var EncryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        EncryptCipher
            .Init(true, new ParametersWithIV(new KeyParameter(key), key, 0 ,16));
        var DecryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        DecryptCipher
            .Init(false, new ParametersWithIV(new KeyParameter(key), key, 0, 16));
        Stream = new CipherStream(Stream, DecryptCipher, EncryptCipher);
    }
    
    public void Dispose()
    {
        Stream.Dispose();
    }

    public void WriteBool(bool val)
    {
        WriteUnsignedByte((byte)(val? 0x01: 0x00));
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

    public void WriteLong(long val)
    {
        Stream.Write(BitConverter.GetBytes(val));
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

    public void WriteUuid(Guid uuid)
    {
        var guid = uuid.ToByteArray();
        var l1 = new byte[8];
        var l2 = new byte[8];
        Array.Copy(guid, 0, l1, 0, 8);
        Array.Copy(guid, 8, l2, 0, 8);
        WriteByteArray(l1);
        WriteByteArray(l2);
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

    public void WritePrefixedOptionalString(string? str)
    {
        WriteBool(str != null);
        if (str != null)
        {
            WriteString(str);
        }
    }

    public void WritePrefixedByteArray(byte[] arr)
    {
        if (arr == null)
        {
            WriteVarInt(0);
            return;
        }
        WriteVarInt(arr.Length);
        foreach (var item in arr)
        {
            WriteByte((sbyte)item);
        }
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
        return ReadUnsignedByte() == 1;
    }

    public sbyte ReadByte()
    {
        var b = Stream.ReadByte();
        if (b == -1) throw new EndOfStreamException();
        return (sbyte)b;
    }

    public byte ReadUnsignedByte()
    {
        return (byte) ReadByte();
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
        var dat = ReadByteArray(8);
        return BitConverter.ToInt64(dat);
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
        var dat = ReadByteArray(16);
        return new Guid(dat);
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
    public byte[] ReadPrefixedByteArray()
    {
        var length = ReadVarInt();
        var data = new byte[length];
        Stream.ReadExactly(data, 0, length);
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