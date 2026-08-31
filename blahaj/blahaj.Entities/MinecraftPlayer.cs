using System.Numerics;
using blahaj.blahaj.Network;
using blahaj.blahaj.Network.Events;
using blahaj.blahaj.Network.Packets;
using blahaj.blahaj.Network.Packets.Configuration.ToServer;
using blahaj.blahaj.Network.Packets.Login.ToClient;
using blahaj.blahaj.Network.Packets.Play.PlayerInfo;
using blahaj.blahaj.Network.Packets.Play.ToClient;
using blahaj.blahaj.Network.Packets.Status.ToClient.Json;
using blahaj.blahaj.World;
using Microsoft.Extensions.Logging;

namespace blahaj.blahaj.Entities;

public class MinecraftPlayer : Entity
{
    public string Name { get; set; } = "";

    private Guid _providedUuid;

    public Guid? Uuid => GameProfile?.Uuid ?? _providedUuid;

    public Guid SessionId { get; private set; } = Guid.NewGuid();
   
    public GameProfile? GameProfile { get; set; }
    
    private NetClient NetClient { get; set; }
    
    public ClientInformationPacket ClientInformation { get; set; }

    private int LastTeleportId { get; set; }

    public bool Connected { get; set; } = false;
   
    public bool IgnoreMovement { get; set; } = false;
    
    private Task KeepAliveTask { get; set; }

    private long LastKeepAliveId { get; set; } = 0;
    private long LastReceivedKeepAliveId { get; set; } = 0;
    
    private ILogger Logger { get; set; }
    
    public MinecraftPlayer(string name, Guid uuid, NetClient netClient)
    {
        Type = 156; 
        
        Name = name;
        _providedUuid = uuid;
        NetClient = netClient;
        Metadata = new PlayerMetadata();
        
        using ILoggerFactory factory = LoggerFactory.Create(build => build
#if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
#endif
            .AddConsole());
        Logger = factory.CreateLogger($"Player.{name}");
        
    }
    
    private async void KeepAlive()
    {
        Connected = true;
        while (Connected)
        {
            if (LastKeepAliveId != 0 && LastReceivedKeepAliveId != LastKeepAliveId)
            {
                Connected = false;
                return;
            }
            
            var random = new Random();
            var id = random.Next();
            var x = new KeepAlive(id);
            QueuePacket(x);
            LastKeepAliveId = id;
            await Task.Delay(5000);
        }
        // Task will be immediately collected without
        GC.KeepAlive(KeepAliveTask);
    }

    public void UpdateKeepAlive(long keepAliveId)
    {
        LastReceivedKeepAliveId = keepAliveId;
    }
    
    public bool IsValid()
    {
        return Name.Length > 0 || Uuid != null;
    }

    public StatusPlayer ToStatus()
    {
        return new StatusPlayer(Name, Uuid.ToString());
    }

    public void QueuePacket(Packet packet)
    {
        NetClient.QueuePacket(packet);
    }

    public void Join(WorldSettings settings)
    {
        var x = new LoginPacket(settings, Id, "overworld", false, null, null, 0, 1);
        QueuePacket(x);
        
        NetClient.Server.OnPlayerConnected.Invoke(this, new PlayerConnectedArgs(this));
        
        NetClient.OnConnectionClosed += (object sender, ConnectionClosedArgs args) =>
        {
            Logger.LogInformation($"Disconnected");
            Connected = false;
        };
        
        KeepAliveTask = new Task(KeepAlive);
        KeepAliveTask.Start();
    }

    public void SetPosition(Vector3? position = null, Vector3?  velocity = null, Vector2? rotation = null)
    {
        SetPosition(position ?? Position, velocity ?? Velocity, rotation ?? Rotation);
    }


    
    private void SetPosition(Vector3 position, Vector3 velocity, Vector2 rotation)
    {
        var lastChunkX = Math.Floor(Position.X / 16);
        var lastChunkZ = Math.Floor(Position.Z / 16);
        var currentChunkX = Math.Floor(position.X / 16);
        var currentChunkZ = Math.Floor(position.Z / 16);

        var lastPos = Position;

        Position = position;
        Velocity = velocity;
        Rotation = rotation;
       
        
        if (Math.Abs(lastChunkX - currentChunkX) > 0.1 || Math.Abs(lastChunkZ - currentChunkZ) > 0.1)
        {
            // Chunk changed
            var x = new SetCenterChunk((int)currentChunkX, (int)currentChunkZ);
            QueuePacket(x);
        }
        
        if (Vector3.Distance(lastPos, Position) > 1)
        {
            SynchronizePosition(); 
        }
    }

    private void SynchronizePosition()
    {
        var rand = new Random();
        var id = rand.Next();

        var synchPlayer = new SynchronizePlayerPosition(
            id, Position, Velocity, Rotation, 0);
        LastTeleportId = id;
        IgnoreMovement = true;
        QueuePacket(synchPlayer);
    }
    
    public void Teleport(Vector3 position,  Vector3 velocity, Vector2 rotation)
    {
        SetPosition(position, velocity, rotation);
        SynchronizePosition();
    }

    public void UpdateTeleport(int teleportId)
    {
        if (LastTeleportId == teleportId) IgnoreMovement = false;
    }
    
    
}