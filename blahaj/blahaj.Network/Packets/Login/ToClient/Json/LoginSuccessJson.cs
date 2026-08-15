namespace blahaj.blahaj.Network.Packets.Login.ToClient.Json;

public class LoginSuccessJson
{
    public string Id { get; set; }
    public string Name { get; set; }
    public LoginSuccessProperties[] Properties { get; set; }
}