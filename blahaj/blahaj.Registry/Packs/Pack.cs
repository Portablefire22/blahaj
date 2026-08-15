namespace blahaj.blahaj.Registry.Packs;

public class Pack
{
   public string Namespace { get; private set; }
   public string Id { get; private set; }
   public string Version { get; private set; }

   public Pack(string ns, string id, string version)
   {
      Namespace = ns;
      Id = id;
      Version = version;
   }
}