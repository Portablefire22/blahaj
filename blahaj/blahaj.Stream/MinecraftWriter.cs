namespace blahaj.blahaj.Stream;

public interface MinecraftWriter
{
    public void WriteBool(bool val);
    public void WriteByte(sbyte b);
    public void WriteUnsignedByte(byte b);
    public short WriteShort();
    public ushort WriteUnsignedShort();
    public int WriteInt();
    public void WriteLong(long val);
    public float WriteFloat();
    public double WriteDouble();
    public void WriteString(string str);
    public object WriteTextComponent();
    public object WriteJsonTextComponent();
    public object WriteIdentifier();
    public void WriteVarInt(int val);
    public int WriteVarLong();
    public object WriteEntityMetadata();
    public object WriteSlot();
    public object WriteHashedSlot();
    public object WriteNbt();
    public object WritePosition();
    public Guid WriteUuid();
    public object WriteBitSet();
    public object WriteFixedBitSet();
    public T? WriteOptional<T>();
    public T? WritePrefixedOptional<T>();
    public void WriteArray<T>(T[] arr);
    public void WritePrefixedArray<T>(T[] arr);
    public T WriteEnum<T>();
    public T[] WriteEnumSet<T>();
    public void WriteByteArray(byte[] arr);
    public object WriteId();
    public object[] WriteIdSet();
    public object WriteSoundEvent();
    public object WriteChatType();
    public object WriteTeleportFlags();
    public object WriteRecipeDisplay();
    public object WriteSlotDisplay();
    public object WriteChunkData();
    public object WriteLightData();
    public object WriteOr<A, B>();
}