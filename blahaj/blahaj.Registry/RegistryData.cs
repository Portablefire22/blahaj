using System.Text;
using blahaj.blahaj.Registry.Data;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Registry;

public class RegistryData
{
    public RegistryData(string id, IRegistryEntry[] registryEntries)
    {
        Id = id;
        RegistryEntries = registryEntries;
    }

    public string Id { get; set; }

    public IRegistryEntry[] RegistryEntries { get; set; }
    
    public void Write(MinecraftStream writer)
    {
        using var ms = new MemoryStream();
        using var msWriter = new MinecraftStream(ms);
        msWriter.WriteString(Id);
        //writer.WriteByteArray(Encoding.UTF8.GetBytes(Id));

        msWriter.WriteVarInt(RegistryEntries.Length);
        foreach (var entry in RegistryEntries)
        {
            entry.Write(msWriter);
        }

        var str = "";
        foreach (var b in ms.ToArray())
        {
            str += $"{b} ";
        }
        Console.WriteLine(str);
        writer.WriteByteArray(ms.ToArray());
        /*writer.WritePrefixedByteArray();*/
    }
}