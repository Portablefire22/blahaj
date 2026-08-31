using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.Serialization;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Entities.Events;
using blahaj.blahaj.Logging;
using blahaj.blahaj.Network;
using blahaj.blahaj.Network.Packets.Play.ToClient;
using blahaj.blahaj.World.Actions;
using blahaj.blahaj.World.Chunks;
using Microsoft.Extensions.Logging;

namespace blahaj.blahaj.World;


/// <summary>
/// An individual 'world' e.g. Overworld, Nether, etc
/// Contains and processes everything contained, e.g. chunk-loading, entity processing, physics
/// </summary>
public class MinecraftDimension
{
    public MinecraftDimension(NetServer server, MinecraftWorld world, string identifier, long seed)
    {
        Logger = LoggingProvider.NewLogger($"blahaj.blahaj.World.{identifier}");

        World = world;
        Identifier = identifier;

        var generator = ChunkGenerator.FromIdentifier(identifier, seed);
        ChunkManager = new ChunkManager(generator, identifier);
        
        
        
    }
    
    private MinecraftWorld World { get; set; }
    
    public WorldSettings WorldSettings => World.WorldSettings;

    public NetServer Server { get; set; }
    
    public bool IsDefault { get; set; } = false;
    public string Identifier {get;}
    private ChunkManager ChunkManager { get; }
    
    private ConcurrentDictionary<int, Entity> _entities = [];

    private List<MinecraftPlayer> _players = [];
    
    private ConcurrentQueue<WorldAction>  _actions = [];

    private Thread DimensionThread { get; set; }
    
    private ILogger Logger { get; set; }

    public void Start()
    {
        DimensionThread = new Thread(DimensionLoop);
        DimensionThread.Start();
        
        
        QueueAction(new WorldAction(ActionType.AddEntity, 
            new Entity(new Vector3(-82.5f, 320f, -501.5f), 54)));
    }

    
    /// <summary>
    /// Forever loop to process Ticks at a maximum of 20 per second.
    /// </summary>
    private void DimensionLoop()
    {
        DateTimeOffset tickStart = default;
        int count = 0;
        var lastSecond = DateTimeOffset.UnixEpoch;
        while (true)
        {
            var temp = DateTimeOffset.Now;
            Logger.LogTrace($"Millis since last Tick: {(temp - tickStart).TotalMilliseconds} ms");
            tickStart = temp;
            Tick();

            var end = DateTimeOffset.Now - tickStart;
            var totalMillis = (int)Math.Floor(end.TotalMilliseconds);
            
            count++;
            if ((temp - lastSecond).TotalSeconds >= 1)
            {
                Logger.LogTrace($"Ticks in last second: {count}");
                lastSecond = temp;
                count = 0;
            }
            if (totalMillis <= 50)
            {
                Thread.Sleep(50 -  totalMillis);
            }
        }
        Logger.LogError($"Dimension has stopped ticking!");
    }
    
    private void Tick()
    {
        while (_actions.TryDequeue(out var action))
        {
            PerformAction(action);
        }

        ChunkManager.Tick();

        foreach (var (id, entity) in _entities)
        {
            entity.Tick();
        }
        
    }

    private void PerformAction(WorldAction action)
    {
        switch (action.Type)
        {
            case ActionType.AddEntity:
                AddEntity((Entity)action.Data);
                break;
            case ActionType.LoadChunk:
                if (!ChunkManager.LoadChunk((KeyValuePair<int, int>)action.Data, out var chunk)) break;
                var data = new ChunkDataWithLight(chunk);
                foreach (var player in _players)
                {
                    //if (Vector2.Distance(player.ChunkPosition, chunk.ChunkPosition) > 10) continue;
                    player.QueuePacket(data);
                }
                break;
            default:
                throw new ArgumentException($"Invalid action type {action.Type}");
        }
    }

    public void QueueAction(WorldAction action) => _actions.Enqueue(action);

    private void OnPlayerChunkPositionChanged(object? sender, ChunkPosChangedArgs args)
    {
        Logger.LogInformation($"Player changed position");
        var player =  sender as MinecraftPlayer;
        if (player == null) return;
        var width = 7 + (World.WorldSettings.ViewDistance * 2);
        for (int x = -(int) Math.Ceiling(width / 2f); x < (int) Math.Floor(width / 2f); x++)
        {
            for (int z = -(int) Math.Ceiling(width / 2f); z < (int) Math.Floor(width / 2f); z++)
            {
                QueueAction(new WorldAction(ActionType.LoadChunk, new KeyValuePair<int, int>((int)(args.ChunkPos.X + x), (int)(args.ChunkPos.Y + z))));
            }
        }
    }
    
    private bool AddEntity(Entity entity)
    {


        if (entity.Id != 0)
        {
            if (!_entities.TryAdd(entity.Id, entity)) return false;
        }
        else
        {
            var random = new Random();
            do
            {
                entity.Id = random.Next();
            } while (!_entities.TryAdd(entity.Id, entity));
        }
        entity.Server = Server; 
       
        if (entity is MinecraftPlayer player)
        {
            _players.Add(player);
            player.ChunkPosChanged += OnPlayerChunkPositionChanged;
            
            var x = new LoginPacket(WorldSettings, player.Id, Identifier, false, null, null, 1, 1);
            player.QueuePacket(x);
            
            SpawnEntitiesForPlayer(player);
        }
        
        
        SpawnEntityForPlayers(entity, true);
        
        return true;
    }
    
    
    public void UpdateEntityMetadata(Entity entity)
    {
        var packet = new SetEntityData(entity.Id, entity.Metadata);
        foreach (var player in _players)
        {
            player.QueuePacket(packet);
        }
    }
    
    

    public void SpawnEntityForPlayers(Entity entity, bool checkLocal = false)
    {
        foreach (var player in _players)
        {
            if (checkLocal && entity is MinecraftPlayer playerEntity && playerEntity.Id == player.Id)
            {
                continue;
            }
            player.QueuePacket(new AddEntity(entity));
        }
        UpdateEntityMetadata(entity);
    }
    
    public void SpawnEntitiesForPlayer(MinecraftPlayer player)
    {
        foreach (var (id, entity) in _entities)
        {
            player.QueuePacket(new AddEntity(entity));
            UpdateEntityMetadata(entity);
        }
    }

    
}