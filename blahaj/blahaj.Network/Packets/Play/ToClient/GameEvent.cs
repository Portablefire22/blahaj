
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

public class GameEvent : Packet
{
    public GameEvent() : base (0x26)
    {
    }

    public GameEvent(byte @event, float value) : this()
    {
        Event = @event;
        Value = value;
    }

    public byte Event { get; set; }
    public float Value { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        Event = stream.ReadUnsignedByte();
        Value = stream.ReadFloat();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteUnsignedByte(Event);
        stream.WriteFloat(Value);
    }
}