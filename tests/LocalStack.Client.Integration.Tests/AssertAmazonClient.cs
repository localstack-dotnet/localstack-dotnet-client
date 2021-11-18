namespace LocalStack.Client.Integration.Tests;

public static class AssertAmazonClient
{
    public static void AssertClientConfiguration(AmazonServiceClient amazonServiceClient)
    {
        IClientConfig clientConfig = amazonServiceClient.Config;

        Assert.Equal($"http://{Constants.LocalStackHost}:{Constants.EdgePort}", clientConfig.ServiceURL);
        Assert.True(clientConfig.UseHttp);

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
