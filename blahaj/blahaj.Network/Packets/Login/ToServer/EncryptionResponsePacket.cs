using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using blahaj.blahaj.Crypto;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Network.Packets.Login.ToClient;
using blahaj.blahaj.Network.Packets.Login.ToClient.Json;
using blahaj.blahaj.Stream;
using Microsoft.Extensions.Logging;

namespace blahaj.blahaj.Network.Packets.Login.ToServer;

[PacketState(ConnectionState.Login)]
[PacketDirection(PacketDirection.ClientToServer)]
public class EncryptionResponsePacket : Packet, IInvokableWithClient, IInvokableWithPlayer
{
    public byte[] SharedSecret { get; private set; }
    public byte[] VerifyToken { get; private set; }
   
    private ILogger Logger { get; }
    
    public EncryptionResponsePacket() : base(0x01)
    {
        using ILoggerFactory factory = LoggerFactory.Create(build => build
#if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
#endif
            .AddConsole());
        Logger = factory.CreateLogger<NetClient>();
    }

    public override void Read(MinecraftStream stream)
    {
        var encryptedShared = stream.ReadPrefixedByteArray();
        SharedSecret = Encryption.Key.Decrypt(encryptedShared, RSAEncryptionPadding.Pkcs1);
        VerifyToken = stream.ReadPrefixedByteArray();
    }

    public override void Write(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public Packet? Invoke()
    {
        string serverHash;
        LoginSuccessJson? json;
        using (var ms = new MemoryStream())
        {
            var ascii = Encoding.ASCII.GetBytes("BlahajCSharpMeowPurr");
            ms.Write(ascii, 0, ascii.Length);
            ms.Write(SharedSecret, 0, 16);
            var publicKey = Encryption.ExportKeyAsDer();
            ms.Write(publicKey, 0, publicKey.Length);
            serverHash = MinecraftShaDigest.Sha(ms.ToArray());
        }
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://sessionserver.mojang.com/");
            Logger.LogDebug($"{client.BaseAddress}session/minecraft/hasJoined?username={Player.Name}&serverId={serverHash}");
            var res = client.GetStringAsync($"session/minecraft/hasJoined?username={Player.Name}&serverId={serverHash}").Result;
            if (res.Length == 0) {
                Client.Disconnect();
                return null;
            }

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            json = JsonSerializer.Deserialize<LoginSuccessJson>(res, options);  
            if (json == null) {
                Client.Disconnect();
                return null;
            }
            Logger.LogInformation("Authentication Successful");
        }
        Client.InitEncryption(SharedSecret);
        return new LoginSuccessPacket(json, Player.SessionId);
    }

    public NetClient Client { get; set; }
    public MinecraftPlayer Player { get; set; }
}