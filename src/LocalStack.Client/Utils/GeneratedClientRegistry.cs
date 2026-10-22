#if NET8_0_OR_GREATER
using System.Collections.Concurrent;

namespace LocalStack.Client.Utils;

// Generated initializers only register operations. Metadata, configuration and client creation
// stay lazy, at the original Session/ISessionReflection call sites (including on JIT).
internal static class GeneratedClientRegistry
{
    private static readonly ConcurrentDictionary<Type, ClientEntry> Clients = new();
    private static readonly ConcurrentDictionary<(Assembly Assembly, string Name), Type> ClientNames = new();
    private static readonly ConcurrentDictionary<Type, ConfigEntry> Configs = new();

    public static void RegisterClient(Type type, Func<IServiceMetadata> metadata, Func<ClientConfig> config,
                                      Func<Type, ConstructorInfo?> constructor, Func<AWSCredentials, ClientConfig, AmazonServiceClient> create)
    {
        Clients.TryAdd(type, new ClientEntry(metadata, config, constructor, create));
        if (type.FullName is string name)
        {
            ClientNames.TryAdd((type.Assembly, name), type);
        }
    }

    public static void RegisterConfig(Type type, Func<ClientConfig, bool, bool> style, Action<object, string> region) =>
        Configs.TryAdd(type, new ConfigEntry(style, region));

    public static Type? FindClient(Assembly assembly, string name) => ClientNames.TryGetValue((assembly, name), out Type? type) ? type : null;

    public static bool TryGetClient(Type type, [NotNullWhen(true)] out ClientEntry? entry)
    {
        entry = null;
        return type != null && Clients.TryGetValue(type, out entry);
    }

    public static bool TryGetConfig(Type type, [NotNullWhen(true)] out ConfigEntry? entry) => Configs.TryGetValue(type, out entry);

    internal sealed record ClientEntry(Func<IServiceMetadata> Metadata, Func<ClientConfig> Config, Func<Type, ConstructorInfo?> Constructor,
                                       Func<AWSCredentials, ClientConfig, AmazonServiceClient> Create);

    internal sealed record ConfigEntry(Func<ClientConfig, bool, bool> Style, Action<object, string> Region);
}
#endif
