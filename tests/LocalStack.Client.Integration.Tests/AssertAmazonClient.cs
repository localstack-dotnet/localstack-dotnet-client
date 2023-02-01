namespace LocalStack.Client.Integration.Tests;

public static class AssertAmazonClient
{
    public const string TestAwsRegion = "eu-central-1";
    public const bool UseSsl = true;

    public static void AssertClientConfiguration(AmazonServiceClient amazonServiceClient)
    {
        IClientConfig clientConfig = amazonServiceClient.Config;

        if (clientConfig.ServiceURL != null)
        {
            string protocol = clientConfig.UseHttp ? "http" : "https";
            Assert.Equal($"{protocol}://{Constants.LocalStackHost}:{Constants.EdgePort}/", clientConfig.ServiceURL);
        }
        else if(clientConfig.ServiceURL == null)
        {
            Assert.Equal(RegionEndpoint.GetBySystemName(TestAwsRegion), amazonServiceClient.Config.RegionEndpoint);
        }
        else
        {
            throw new MisconfiguredClientException(
                "Both ServiceURL and RegionEndpoint properties are null. Under normal conditions, one of these two properties must be a set. This means either something has changed in the new version of the Amazon Client library or there is a bug in the LocalStack.NET Client that we could not detect before. Please open an issue on the subject.");
        }

        // if(clientConfig.ServiceURL != null)
        Assert.Equal(UseSsl, !clientConfig.UseHttp);

        PropertyInfo forcePathStyleProperty = clientConfig.GetType().GetProperty("ForcePathStyle", BindingFlags.Public | BindingFlags.Instance);

        if (forcePathStyleProperty != null)
        {
            bool useForcePathStyle = forcePathStyleProperty.GetValue(clientConfig) is bool && (bool)forcePathStyleProperty.GetValue(clientConfig);
            Assert.True(useForcePathStyle);
        }

        Assert.Equal(Constants.LocalStackHost, clientConfig.ProxyHost);
        Assert.Equal(Constants.EdgePort, clientConfig.ProxyPort);
    }
}
