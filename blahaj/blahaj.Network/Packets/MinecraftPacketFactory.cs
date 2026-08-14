using blahaj.blahaj.Stream;
using blahaj.Network.Packets.Configuration;
using blahaj.Network.Packets.Configuration.ToServer;
using blahaj.Network.Packets.Handshake;
using blahaj.Network.Packets.Login;
using blahaj.Network.Packets.Play.ToServer;
using blahaj.Network.Packets.Status;

namespace blahaj.Network.Packets;

public static class MinecraftPacketFactory
{
    private static PacketFactory<ConnectionState, MinecraftStream, Packet> HandshakePacketFactory { get; }
    private static PacketFactory<ConnectionState, MinecraftStream, Packet> StatusPacketFactory { get; }
    private static PacketFactory<ConnectionState, MinecraftStream, Packet> LoginPacketFactory { get; }
    private static PacketFactory<ConnectionState, MinecraftStream, Packet> ConfigurationPacketFactory { get; }
    private static PacketFactory<ConnectionState, MinecraftStream, Packet> PlayPacketFactory { get; }
    
    static MinecraftPacketFactory()
    {
        HandshakePacketFactory = new PacketFactory<ConnectionState, MinecraftStream, Packet>();
        StatusPacketFactory = new PacketFactory<ConnectionState, MinecraftStream, Packet>();
        LoginPacketFactory = new PacketFactory<ConnectionState, MinecraftStream, Packet>();
        ConfigurationPacketFactory = new PacketFactory<ConnectionState, MinecraftStream, Packet>();
        PlayPacketFactory = new PacketFactory<ConnectionState, MinecraftStream, Packet>();

        RegisterPackets();
    }
    
    public static Packet? GetPacket(ConnectionState state, int packetId)
    {
        Packet? packet = null;
        var success = false;
        switch (state)
        {
            case ConnectionState.Handshake:
                success = HandshakePacketFactory.TryGet(packetId, out packet);
                break;
            case ConnectionState.Status:
                success = StatusPacketFactory.TryGet(packetId, out packet);
                break;
            case ConnectionState.Login:
                success = LoginPacketFactory.TryGet(packetId, out packet);
                break;
            case ConnectionState.Configuration:
                success = ConfigurationPacketFactory.TryGet(packetId, out packet);
                break;  
            case ConnectionState.Play:
                success = PlayPacketFactory.TryGet(packetId, out packet);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        if (!success) return null;
        return packet;
    }

    private static void RegisterPackets()
    {
        Register<HandshakePacket>(ConnectionState.Handshake);
        
        Register<StatusRequestPacket>(ConnectionState.Status);
        Register<PingPacket>(ConnectionState.Status);

        Register<LoginStartPacket>(ConnectionState.Login);
        Register<EncryptionResponsePacket>(ConnectionState.Login);
        Register<LoginAcknowledgedPacket>(ConnectionState.Login);

        Register<BrandPacket>(ConnectionState.Configuration);
        Register<ClientInformationPacket>(ConnectionState.Configuration);
        Register<KnownPacksPacket>(ConnectionState.Configuration);
        Register<AcknowledgeFinishConfiguration>(ConnectionState.Configuration);
        
        Register<ClientTickEndPacket>(ConnectionState.Play);
    }
    
    private static void Register<Pt>(ConnectionState state) where Pt : Packet, new()
    {switch (state)
        {
            case ConnectionState.Handshake:
                Register<Pt>(HandshakePacketFactory);
                break;
            case ConnectionState.Status:
                Register<Pt>(StatusPacketFactory);
                break;
            case ConnectionState.Login:
                Register<Pt>(LoginPacketFactory);
                break;
            case ConnectionState.Configuration:
                Register<Pt>(ConfigurationPacketFactory);
                break;
            case ConnectionState.Play:
                Register<Pt>(PlayPacketFactory);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    private static void Register<Pt>(PacketFactory<ConnectionState, MinecraftStream, Packet> factory)
    where Pt : Packet, new()
    {
        factory.Register(() => new Pt());
    }
}