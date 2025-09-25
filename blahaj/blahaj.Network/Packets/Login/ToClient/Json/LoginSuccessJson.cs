namespace blahaj.Network.Packets.Login.Json;

public class LoginSuccessJson
{
    public string Id { get; set; }
    public string Name { get; set; }
    public LoginSuccessProperties[] Properties { get; set; }
}