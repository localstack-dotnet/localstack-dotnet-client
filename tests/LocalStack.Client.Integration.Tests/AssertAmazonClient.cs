using Type = System.Type;

namespace LocalStack.Client.Integration.Tests;

internal static class AssertAmazonClient
{
    public const string TestAwsRegion = "eu-central-1";
    public const bool UseSsl = true;

    [SuppressMessage("Test", "CA1508: Avoid dead conditional code", Justification = "False positive")]
    public static void AssertClientConfiguration(AmazonServiceClient amazonServiceClient)
    {
        if (amazonServiceClient == null)
        {
            throw new ArgumentNullException(nameof(amazonServiceClient));
        }

        IClientConfig clientConfig = amazonServiceClient.Config;

        if (clientConfig.ServiceURL != null)
        {
            string protocol = clientConfig.UseHttp ? "http" : "https";
            Assert.Equal($"{protocol}://{Constants.LocalStackHost}:{Constants.EdgePort}/", clientConfig.ServiceURL);
        }
        else if (clientConfig.ServiceURL == null)
        {
            Assert.Equal(RegionEndpoint.GetBySystemName(TestAwsRegion), amazonServiceClient.Config.RegionEndpoint);
        }
        else
        {
            throw new MisconfiguredClientException(
                "Both ServiceURL and RegionEndpoint properties are null. Under normal conditions, one of these two properties must be a set. This means either something has changed in the new version of the Amazon Client library or there is a bug in the LocalStack.NET Client that we could not detect before. Please open an issue on the subject.");
        }

        Assert.Equal(UseSsl, !clientConfig.UseHttp);

#if NET8_0_OR_GREATER
        // Modern approach: Use accessor-based TryGetForcePathStyle method for .NET 8+ builds
        // This avoids reflection and provides AOT compatibility
        Type clientType = amazonServiceClient.GetType();
        if (AwsAccessorRegistry.TryGet(clientType, out IAwsAccessor? accessor) &&
            accessor != null &&
            clientConfig is ClientConfig config &&
            accessor.TryGetForcePathStyle(config, out bool? forcePathStyle))
        {
            Assert.True(forcePathStyle.HasValue);
            Assert.True(forcePathStyle.Value);
        }
#elif NETFRAMEWORK || NETSTANDARD
        // Legacy approach: Use reflection for .NET Framework and .NET Standard builds
        PropertyInfo? forcePathStyleProperty = clientConfig.GetType().GetProperty("ForcePathStyle", BindingFlags.Public | BindingFlags.Instance);

        if (forcePathStyleProperty != null)
        {
            bool useForcePathStyle = forcePathStyleProperty.GetValue(clientConfig) is bool && (bool)forcePathStyleProperty.GetValue(clientConfig)!;
            Assert.True(useForcePathStyle);
        }
#else
        throw new NotSupportedException("This library is only supported on .NET Framework, .NET Standard, or .NET 8.0 or higher.");
#endif

        Assert.Equal(Constants.LocalStackHost, clientConfig.ProxyHost);
        Assert.Equal(Constants.EdgePort, clientConfig.ProxyPort);
    }
}