#pragma warning disable CA1031

namespace LocalStack.Client.Functional.Tests.CloudFormation;

public sealed class CloudFormationProvisioner
{
    private readonly IAmazonCloudFormation _amazonCloudFormation;
    private readonly ILogger _logger;

    public CloudFormationProvisioner(IAmazonCloudFormation amazonCloudFormation, ILogger logger)
    {
        _amazonCloudFormation = amazonCloudFormation;
        _logger = logger;
    }

    internal async Task ConfigureCloudFormationAsync(CloudFormationResource resource, CancellationToken cancellationToken = default)
    {
        await ProcessCloudFormationStackResourceAsync(resource, cancellationToken).ConfigureAwait(false);
        await ProcessCloudFormationTemplateResourceAsync(resource, cancellationToken).ConfigureAwait(false);
    }

    private async Task ProcessCloudFormationStackResourceAsync(CloudFormationResource resource, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new DescribeStacksRequest { StackName = resource.Name };
            DescribeStacksResponse? response = await _amazonCloudFormation.DescribeStacksAsync(request, cancellationToken).ConfigureAwait(false);

            // If the stack didn't exist then a StackNotFoundException would have been thrown.
            Stack? stack = response.Stacks[0];

            // Capture the CloudFormation stack output parameters on to the Aspire CloudFormation resource. This
            // allows projects that have a reference to the stack have the output parameters applied to the
            // projects IConfiguration.
            resource.Outputs = stack!.Outputs;

            resource.ProvisioningTaskCompletionSource?.TrySetResult();
        }
        catch (Exception e)
        {
            if (e is AmazonCloudFormationException ce && string.Equals(ce.ErrorCode, "ValidationError", StringComparison.Ordinal))
            {
                _logger.LogError("Stack {StackName} does not exists to add as a resource.", resource.Name);
            }
            else
            {
                _logger.LogError(e, "Error reading {StackName}.", resource.Name);
            }

            resource.ProvisioningTaskCompletionSource?.TrySetException(e);
        }
    }

    private async Task ProcessCloudFormationTemplateResourceAsync(CloudFormationResource resource, CancellationToken cancellationToken = default)
    {
        try
        {
            var executor = new CloudFormationStackExecutor(_amazonCloudFormation, resource, _logger);
            Stack? stack = await executor.ExecuteTemplateAsync(cancellationToken).ConfigureAwait(false);

            if (stack != null)
            {
                _logger.LogInformation("CloudFormation stack has {Count} output parameters", stack.Outputs.Count);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    foreach (Output? output in stack.Outputs)
                    {
                        _logger.LogInformation("Output Name: {Name}, Value {Value}", output.OutputKey, output.OutputValue);
                    }
                }

                _logger.LogInformation("CloudFormation provisioning complete");

                resource.Outputs = stack.Outputs;
                resource.ProvisioningTaskCompletionSource?.TrySetResult();
            }
            else
            {
                _logger.LogError("CloudFormation provisioning failed");

                resource.ProvisioningTaskCompletionSource?.TrySetException(new AwsProvisioningException("Failed to apply CloudFormation template"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error provisioning {ResourceName} CloudFormation resource", resource.Name);
            resource.ProvisioningTaskCompletionSource?.TrySetException(ex);
        }
    }
}