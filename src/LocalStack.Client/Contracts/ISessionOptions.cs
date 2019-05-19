namespace LocalStack.Client.Contracts
{
    public interface ISessionOptions
    {
        string AwsAccessKeyId { get; }

        string AwsAccessKey { get; }

        string AwsSessionToken { get; }

        string RegionName { get; }

        string LocalStackHost { get; }
    }
}