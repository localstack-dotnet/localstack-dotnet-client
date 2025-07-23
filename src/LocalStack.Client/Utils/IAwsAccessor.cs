namespace LocalStack.Client.Utils;

/// <summary>
/// Interface for type-safe AWS SDK private member access.
/// Implementations are generated at compile-time for .NET 8+ or use reflection for legacy frameworks.
/// </summary>
public interface IAwsAccessor
{
    /// <summary>
    /// Gets the AWS client type this accessor supports.
    /// </summary>
    Type ClientType { get; }

    /// <summary>
    /// Gets the AWS client configuration type this accessor supports.
    /// </summary>
    Type ConfigType { get; }

    /// <summary>
    /// Gets the service metadata for the AWS client.
    /// Accesses the private static 'serviceMetadata' field.
    /// </summary>
    IServiceMetadata GetServiceMetadata();

    /// <summary>
    /// Creates a new ClientConfig instance for the AWS client.
    /// Uses the appropriate constructor for the client's configuration type.
    /// </summary>
    ClientConfig CreateClientConfig();

    /// <summary>
    /// Creates a new AWS service client instance with the specified credentials and configuration.
    /// Uses the appropriate constructor to instantiate the client.
    /// </summary>
    AmazonServiceClient CreateClient(AWSCredentials credentials, ClientConfig clientConfig);

    /// <summary>
    /// Sets the region endpoint on the client configuration.
    /// Accesses private fields/properties for region configuration.
    /// </summary>
    void SetRegion(ClientConfig clientConfig, RegionEndpoint regionEndpoint);

    /// <summary>
    /// Attempts to set the ForcePathStyle property on the client configuration.
    /// Returns true if the property exists and was set, false otherwise.
    /// </summary>
    bool TrySetForcePathStyle(ClientConfig clientConfig, bool value);
}
