namespace LocalStack.Client.Options;

public class SessionOptions : ISessionOptions
{
    public SessionOptions()
    {
    }

    public SessionOptions(string awsAccessKeyId = Constants.AwsAccessKeyId,
                          string awsAccessKey = Constants.AwsAccessKey, 
                          string awsSessionToken = Constants.AwsSessionToken,
                          string regionName = Constants.RegionName)
    {
        AwsAccessKeyId = awsAccessKeyId;
        AwsAccessKey = awsAccessKey;
        AwsSessionToken = awsSessionToken;
        RegionName = regionName;
    }

    public string AwsAccessKeyId { get; } = Constants.AwsAccessKeyId;

    public string AwsAccessKey { get; } = Constants.AwsAccessKey;

    public string AwsSessionToken { get; } = Constants.AwsSessionToken;

    public string RegionName { get; } = Constants.RegionName;
}