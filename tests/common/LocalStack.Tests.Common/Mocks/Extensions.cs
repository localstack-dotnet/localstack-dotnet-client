namespace LocalStack.Tests.Common.Mocks;

public static class Extensions
{
    public static (string awsAccessKeyId, string awsAccessKey, string awsSessionToken, string regionName) SetupDefault(
        this Mock<ISessionOptions> mock, string awsAccessKeyId = "AwsAccessKeyId", string awsAccessKey = "AwsAccessKey", string awsSessionToken = "AwsSessionToken",
        string regionName = "eu-central-1")
    {
        if (mock == null)
        {
            throw new ArgumentNullException(nameof(mock));
        }

        mock.SetupGet(options => options.AwsAccessKeyId).Returns(awsAccessKeyId);
        mock.SetupGet(options => options.AwsAccessKey).Returns(awsAccessKey);
        mock.SetupGet(options => options.AwsSessionToken).Returns(awsSessionToken);
        mock.SetupGet(options => options.RegionName).Returns(regionName);

        return (awsAccessKeyId, awsAccessKey, awsSessionToken, regionName);
    }
}