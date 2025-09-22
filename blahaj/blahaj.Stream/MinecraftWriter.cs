namespace blahaj.blahaj.Stream;

public interface MinecraftWriter
{
    public bool WriteBool();
    public sbyte WriteByte();
    public byte WriteUnsignedByte();
    public short WriteShort();
    public ushort WriteUnsignedShort();
    public int WriteInt();
    public long WriteLong();
    public float WriteFloat();
    public double WriteDouble();
    public string WriteString();
    public object WriteTextComponent();
    public object WriteJsonTextComponent();
    public object WriteIdentifier();
    public int WriteVarInt();
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
    public T[] WriteArray<T>();
    public T WriteEnum<T>();
    public T[] WriteEnumSet<T>();
    public byte[] WriteByteArray();
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