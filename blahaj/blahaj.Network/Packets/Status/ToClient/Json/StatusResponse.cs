using System.Text.Json.Serialization;

namespace blahaj.blahaj.Network.Packets.Status.ToClient.Json;

public class StatusResponse
{
    public StatusResponse(StatusVersion version, StatusPlayers players, StatusDescription description, string favicon, bool enforcesSecureChat)
    {
        Version = version;
        Players = players;
        Description = description;
        Favicon = favicon;
        EnforcesSecureChat = enforcesSecureChat;
    }

    public StatusVersion Version { get; }
    public StatusPlayers Players { get; }
    public StatusDescription Description { get; }
    public string Favicon { get; }
    public bool EnforcesSecureChat { get; } = false;
}

public class StatusVersion
{
    public StatusVersion(string name, short protocol)
    {
        Name = name;
        Protocol = protocol;
    }

    public string Name { get; }
    public short Protocol { get; }
}

public class StatusPlayers
{
    public StatusPlayers(int max, int online, StatusPlayer[] sample)
    {
        Max = max;
        Online = online;
        _sample = sample;
    }

    public int Max { get; }
    public int Online { get; }

    private StatusPlayer[] _sample;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StatusPlayer[]? Sample
    {
        get => _sample.Length > 0 ? _sample : null;
    }
}

public class StatusPlayer
{
    public StatusPlayer(string name, string id)
    {
        Name = name;
        Id = id;
    }

    public string Name { get; }
    public string Id { get; }
}

public class StatusDescription
{
    public StatusDescription(string text)
    {
        Text = text;
    }

    public string Text { get; }
}