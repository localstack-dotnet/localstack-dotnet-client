namespace LocalStack.Client.Options;

#if NET8_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
#endif
public class ConfigOptions : IConfigOptions
{
    public ConfigOptions()
    {
    }

    [SuppressMessage("Usage", "S3427: Method overloads with default parameter values should not overlap",
                     Justification = "The default constructor need for ConfigureOptions")]
    public ConfigOptions(string localStackHost = Constants.LocalStackHost, bool useSsl = Constants.UseSsl, bool useLegacyPorts = Constants.UseLegacyPorts,
                         int edgePort = Constants.EdgePort)
    {
        LocalStackHost = localStackHost;
        UseSsl = useSsl;
        UseLegacyPorts = useLegacyPorts;
        EdgePort = edgePort;
    }

    public string LocalStackHost { get; private set; } = Constants.LocalStackHost;

    public bool UseSsl { get; private set;} = Constants.UseSsl;

    public bool UseLegacyPorts { get; private set;} = Constants.UseLegacyPorts;

    public int EdgePort { get; private set;} = Constants.EdgePort;
}