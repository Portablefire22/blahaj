namespace blahaj.blahaj.Stream;

public interface MinecraftReader
{
   public bool ReadBool();
   public sbyte ReadByte();
   public byte ReadUnsignedByte();
   public short ReadShort();
   public ushort ReadUnsignedShort();
   public int ReadInt();
   public long ReadLong();
   public float ReadFloat();
   public double ReadDouble();
   public string ReadString();
   public object ReadTextComponent();
   public object ReadJsonTextComponent();
   public object ReadIdentifier();
   public int ReadVarInt();
   public int ReadVarLong();
   public object ReadEntityMetadata();
   public object ReadSlot();
   public object ReadHashedSlot();
   public object ReadNbt();
   public object ReadPosition();
   public Guid ReadUuid();
   public object ReadBitSet();
   public object ReadFixedBitSet();
   public T? ReadOptional<T>();
   public T? ReadPrefixedOptional<T>();
   public T[] ReadArray<T>();
   public T ReadEnum<T>();
   public T[] ReadEnumSet<T>();
   public byte[] ReadByteArray(int count);
   public byte[] ReadPrefixedByteArray();
   public object ReadId();
   public object[] ReadIdSet();
   public object ReadSoundEvent();
   public object ReadChatType();
   public object ReadTeleportFlags();
   public object ReadRecipeDisplay();
   public object ReadSlotDisplay();
   public object ReadChunkData();
   public object ReadLightData();
   public object ReadOr<A, B>();
}