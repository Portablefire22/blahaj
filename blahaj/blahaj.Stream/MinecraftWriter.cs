namespace blahaj.blahaj.Stream;

public interface MinecraftWriter
{
    public void WriteBool(bool val);
    public void WriteByte(sbyte b);
    public void WriteUnsignedByte(byte b);
    public void WriteShort(short val);
    public ushort WriteUnsignedShort();
    public void WriteInt(int val);
    public void WriteLong(long val);
    public void WriteFloat(float val);
    public void WriteDouble(double val);
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
    public void WriteUuid(Guid uuid);
    public object WriteBitSet();
    public object WriteFixedBitSet();
    public T? WriteOptional<T>();
    public void WritePrefixedOptionalString(string str);
    public void WritePrefixedByteArray(byte[] arr);
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