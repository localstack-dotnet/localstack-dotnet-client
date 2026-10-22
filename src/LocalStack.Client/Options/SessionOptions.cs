namespace LocalStack.Client.Options;

#if NET8_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
#endif
public class SessionOptions : ISessionOptions
{
    public SessionOptions()
    {
    }

    [SuppressMessage("Usage", "S3427: Method overloads with default parameter values should not overlap",
                     Justification = "The default constructor need for ConfigureOptions")]
    public SessionOptions(string awsAccessKeyId = Constants.AwsAccessKeyId, string awsAccessKey = Constants.AwsAccessKey,
                          string awsSessionToken = Constants.AwsSessionToken, string regionName = Constants.RegionName)
    {
        AwsAccessKeyId = awsAccessKeyId;
        AwsAccessKey = awsAccessKey;
        AwsSessionToken = awsSessionToken;
        RegionName = regionName;
    }

    public string AwsAccessKeyId { get; private set; } = Constants.AwsAccessKeyId;

    public string AwsAccessKey { get; private set; } = Constants.AwsAccessKey;

    public string AwsSessionToken { get; private set; } = Constants.AwsSessionToken;

    public string RegionName { get; private set; } = Constants.RegionName;

    internal void ApplyConfiguration(string? awsAccessKeyId = null, string? awsAccessKey = null, string? awsSessionToken = null, string? regionName = null)
    {
        AwsAccessKeyId = awsAccessKeyId ?? AwsAccessKeyId;
        AwsAccessKey = awsAccessKey ?? AwsAccessKey;
        AwsSessionToken = awsSessionToken ?? AwsSessionToken;
        RegionName = regionName ?? RegionName;
    }
}
