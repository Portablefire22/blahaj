using System.Reflection;
using System.Runtime.CompilerServices;
using blahaj.blahaj.Stream;
using Microsoft.Extensions.Logging;

namespace blahaj.blahaj.Network.Packets;

public static class MinecraftPacketFactory
{
    private static PacketFactory HandshakePacketFactory { get; }
    private static PacketFactory StatusPacketFactory { get; }
    private static PacketFactory LoginPacketFactory { get; }
    private static PacketFactory ConfigurationPacketFactory { get; }
    private static PacketFactory PlayPacketFactory { get; }
    
    private static readonly string _targetNamespace = "blahaj.blahaj.Network.Packets";

    private static ILogger Logger;
    
    static MinecraftPacketFactory()
    {
        HandshakePacketFactory = new PacketFactory();
        StatusPacketFactory = new PacketFactory();
        LoginPacketFactory = new PacketFactory();
        ConfigurationPacketFactory = new PacketFactory();
        PlayPacketFactory = new PacketFactory();

        using ILoggerFactory factory = LoggerFactory.Create(build => build.AddConsole());
        Logger = factory.CreateLogger<NetServer>();
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
        var packetTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, Namespace: not null } 
                        && !x.IsDefined(typeof(CompilerGeneratedAttribute), false)
                        && x.Namespace.Contains(_targetNamespace)
            );
        foreach (var type in packetTypes)
        {
            Logger.LogDebug($"Found type: {type}");
            if (type.GetTypeInfo().GetCustomAttribute(typeof(PacketDirectionAttribute)) == null)
            {
                continue;
            }
            var packet = Activator.CreateInstance(type);
            if (packet != null && packet is Packet gamePacket)
            {
                RegisterPacket(gamePacket);
                Logger.LogDebug($"Added GamePacket ID: 0x{gamePacket.Id:x} ({type})");
            }
        }
    }

    private static void RegisterPacket(Packet packet)
    {
        var attribute = packet.GetType().GetTypeInfo().GetCustomAttribute(typeof(PacketDirectionAttribute), true);
        if (attribute is not PacketDirectionAttribute directionAttribute) return;
        var state = packet.GetType().GetTypeInfo().GetCustomAttribute(typeof(PacketStateAttribute), true);
        if (state is not PacketStateAttribute packetStateAttribute) return;
        foreach (var dir in directionAttribute.Directions)
        {
            if (dir == PacketDirection.ServerToClient) continue;
            Packet Func() => (Packet)Activator.CreateInstance(packet.GetType());            
            switch (packetStateAttribute.State)
            {
                case ConnectionState.Handshake:
                    HandshakePacketFactory.Register(Func);
                    break;
                case ConnectionState.Status:
                    StatusPacketFactory.Register(Func);
                    break;
                case ConnectionState.Login:
                    LoginPacketFactory.Register(Func);
                    break;
                case ConnectionState.Configuration:
                    ConfigurationPacketFactory.Register(Func);
                    break;
                case ConnectionState.Play:
                    PlayPacketFactory.Register(Func);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            } 
            Logger.LogInformation($"Registered ID: {packet.Id} State: {packetStateAttribute.State}");
        } 
    }
}