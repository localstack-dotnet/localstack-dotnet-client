using System.Collections.Concurrent;

namespace LocalStack.Client.Utils;

/// <summary>
/// Thread-safe registry for AWS client accessors.
/// Populated by generated ModuleInitializer methods for .NET 8+ builds.
/// </summary>
public static class AwsAccessorRegistry
{
    private static readonly ConcurrentDictionary<Type, IAwsAccessor> _accessors = new();

    /// <summary>
    /// Registers an accessor for a specific AWS client type.
    /// Called by generated ModuleInitializer methods.
    /// </summary>
    public static void Register<TClient>(IAwsAccessor accessor) where TClient : AmazonServiceClient
    {
        _accessors.TryAdd(typeof(TClient), accessor);
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
}
