using System.Collections.Concurrent;

namespace LocalStack.Client.Utils;

/// <summary>
/// Thread-safe registry for AWS client accessors.
/// Populated by generated ModuleInitializer methods for .NET 8+ builds.
/// </summary>
public static class AwsAccessorRegistry
{
    private static readonly ConcurrentDictionary<Type, IAwsAccessor> _accessors = new();
    private static readonly ConcurrentDictionary<Type, Type> _interfaceToClientMap = new();

    /// <summary>
    /// Registers an accessor for a specific AWS client type.
    /// Called by generated ModuleInitializer methods.
    /// </summary>
    public static void Register<TClient>(IAwsAccessor accessor) where TClient : AmazonServiceClient
    {
        _accessors.TryAdd(typeof(TClient), accessor);
    }

    /// <summary>
    /// Registers the mapping from an AWS service interface to its concrete client type.
    /// Called by generated ModuleInitializer methods.
    /// </summary>
    public static void RegisterInterface<TInterface, TClient>()
        where TInterface : IAmazonService
        where TClient : AmazonServiceClient
    {
        _interfaceToClientMap.TryAdd(typeof(TInterface), typeof(TClient));
    }

    /// <summary>
    /// Gets the registered accessor for the specified AWS client type.
    /// Throws NotSupportedException if no accessor is registered.
    /// </summary>
    public static IAwsAccessor Get(Type clientType)
    {
        if (clientType == null)
        {
            throw new ArgumentNullException(nameof(clientType));
        }

        if (_accessors.TryGetValue(clientType, out var accessor))
        {
            return accessor;
        }

        throw new NotSupportedException(
            $"No AWS accessor registered for client type '{clientType.FullName}'. " +
            "Ensure the AWS SDK package is referenced and the project targets .NET 8 or later for AOT compatibility.");
    }

    /// <summary>
    /// Gets the registered accessor for the specified AWS service interface type.
    /// Resolves the interface to its concrete client type, then returns the accessor.
    /// Throws NotSupportedException if no mapping or accessor is registered.
    /// </summary>
    public static IAwsAccessor GetByInterface<TInterface>() where TInterface : IAmazonService
    {
        return GetByInterface(typeof(TInterface));
    }

    /// <summary>
    /// Gets the registered accessor for the specified AWS service interface type.
    /// Resolves the interface to its concrete client type, then returns the accessor.
    /// Throws NotSupportedException if no mapping or accessor is registered.
    /// </summary>
    public static IAwsAccessor GetByInterface(Type interfaceType)
    {
        if (interfaceType == null)
        {
            throw new ArgumentNullException(nameof(interfaceType));
        }

        if (!_interfaceToClientMap.TryGetValue(interfaceType, out var clientType))
        {
            throw new NotSupportedException(
                $"No AWS client type registered for interface '{interfaceType.FullName}'. " +
                "Ensure the AWS SDK package is referenced and the project targets .NET 8 or later for AOT compatibility.");
        }

        return Get(clientType);
    }

    /// <summary>
    /// Attempts to get the registered accessor for the specified AWS client type.
    /// Returns true if found, false otherwise.
    /// </summary>
    public static bool TryGet(Type clientType, out IAwsAccessor? accessor)
    {
        return _accessors.TryGetValue(clientType, out accessor);
    }

    /// <summary>
    /// Gets the number of registered accessors.
    /// Used for diagnostics and testing.
    /// </summary>
    public static int Count => _accessors.Count;

    /// <summary>
    /// Gets all registered client types.
    /// Used for diagnostics and testing.
    /// </summary>
    public static IEnumerable<Type> RegisteredClientTypes => _accessors.Keys;

    /// <summary>
    /// Gets all registered interface types.
    /// Used for diagnostics and testing.
    /// </summary>
    public static IEnumerable<Type> RegisteredInterfaceTypes => _interfaceToClientMap.Keys;
}
