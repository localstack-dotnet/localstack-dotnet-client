namespace LocalStack.Client.Functional.Tests.CloudFormation;

public interface ICloudFormationResource
{
    string TemplatePath { get; }

    void AddParameter(string parameterName, string parameterValue);

    string? RoleArn { get; set; }

    int StackPollingInterval { get; set; }

    bool DisableDiffCheck { get; set; }

    IList<string> DisabledCapabilities { get; }

    IAmazonCloudFormation? CloudFormationClient { get; set; }

    IList<Output>? Outputs { get; }

    TaskCompletionSource? ProvisioningTaskCompletionSource { get; set; }
}