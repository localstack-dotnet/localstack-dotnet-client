namespace LocalStack.Client.Options;

public class SessionOptions : ISessionOptions
{
    public SessionOptions()
    {
    }

    public SessionOptions(string awsAccessKeyId = Constants.AwsAccessKeyId,
                          string awsAccessKey = Constants.AwsAccessKey, 
                          string awsSessionToken = Constants.AwsSessionToken,
                          string regionName = default)
    {
        AwsAccessKeyId = awsAccessKeyId;
        AwsAccessKey = awsAccessKey;
        AwsSessionToken = awsSessionToken;
        RegionName = regionName;
    }

    public string AwsAccessKeyId { get; private set; } = Constants.AwsAccessKeyId;

    public string AwsAccessKey { get; private set; } = Constants.AwsAccessKey;

    public string AwsSessionToken { get; private set; } = Constants.AwsSessionToken;

    public string RegionName { get; private set; } = default;
}