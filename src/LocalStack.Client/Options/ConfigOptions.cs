namespace LocalStack.Client.Options;

public class ConfigOptions : IConfigOptions
{
    public ConfigOptions()
    {
    }

    public ConfigOptions(
        string localStackHost = Constants.LocalStackHost, 
        bool useSsl = Constants.UseSsl, 
        bool useLegacyPorts = Constants.UseLegacyPorts, 
        int edgePort = Constants.EdgePort)
    {
        LocalStackHost = localStackHost;
        UseSsl = useSsl;
        UseLegacyPorts = useLegacyPorts;
        EdgePort = edgePort;
    }

    public string LocalStackHost { get; private set; } = Constants.LocalStackHost;

    public bool UseSsl { get; private set; } = Constants.UseSsl;

    public bool UseLegacyPorts { get; private set; } = Constants.UseLegacyPorts;

    public int EdgePort { get; private set; } = Constants.EdgePort;
}