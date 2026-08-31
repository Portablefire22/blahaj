using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Entities;

public class MetadataEntry
{
    public MetadataEntry(byte index, int type, object? value)
    {
        Index = index;
        Type = type;
        Value = value;
    }

    public byte Index { get; set; }
    public int Type { get; set; }
    public object? Value { get; set; }



    public void Write(MinecraftStream stream)
    {
        stream.WriteUnsignedByte(Index);
        if (Index == 0xff) return;
        stream.WriteVarInt(Type);
        WriteValue(stream);
    }


    private void WriteValue(MinecraftStream stream)
    {
        switch (Type)
        {
            case 0:
                stream.WriteUnsignedByte((byte)Value);
                break;
            case 1 or 20 or 16 or 17 or 42:
                stream.WriteVarInt((int)Value);
                break;
            case 3:
                stream.WriteFloat((float)Value);
                break;
            case 6 or 14 or 11:
                stream.WriteBool(Value != null);
                break;
            case 8:
                stream.WriteBool((bool)Value);
                break;
            case 19:
                stream.WriteBool(Value != null);
                if (Value != null) stream.WriteVarInt((int)Value);
                break;
            default:
                throw new NotImplementedException($"{Type} is not implemented");
        }
    }
}