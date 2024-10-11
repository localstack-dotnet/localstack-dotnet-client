namespace LocalStack.Client.Functional.Tests.CloudFormation;

internal sealed class CloudFormationResource : ICloudFormationResource
{
    public CloudFormationResource(string name, string templatePath)
    {
        Name = name;
        TemplatePath = templatePath;
        CloudFormationParameters = new Dictionary<string, string>(StringComparer.Ordinal);
        StackPollingInterval = 3;
        DisabledCapabilities = new List<string>();
    }

    public IDictionary<string, string> CloudFormationParameters { get; }

    public string Name { get; }

    public string TemplatePath { get; }

    public string? RoleArn { get; set; }

    public int StackPollingInterval { get; set; }

    public bool DisableDiffCheck { get; set; }

    public IList<string> DisabledCapabilities { get; }

    public IAmazonCloudFormation? CloudFormationClient { get; set; }

    public IList<Output>? Outputs { get; set; }

    public TaskCompletionSource? ProvisioningTaskCompletionSource { get; set; }

    public void AddParameter(string parameterName, string parameterValue)
    {
        CloudFormationParameters[parameterName] = parameterValue;
    }
}