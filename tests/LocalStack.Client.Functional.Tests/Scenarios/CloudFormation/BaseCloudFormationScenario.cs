namespace LocalStack.Client.Functional.Tests.Scenarios.CloudFormation;

public abstract class BaseCloudFormationScenario : BaseScenario
{
    private static readonly TimeSpan StackDeletionTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan StackDeletionPollInterval = TimeSpan.FromMilliseconds(200);

    protected BaseCloudFormationScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig,
                                         bool useServiceUrl = false) : base(testFixture, localStackFixture, configFile, useServiceUrl)
    {
        AmazonCloudFormation = ServiceProvider.GetRequiredService<IAmazonCloudFormation>();
        AmazonSqs = ServiceProvider.GetRequiredService<IAmazonSQS>();
        AmazonSns = ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();

        var logger = ServiceProvider.GetRequiredService<ILogger<BaseCloudFormationScenario>>();
        CloudFormationProvisioner = new CloudFormationProvisioner(AmazonCloudFormation, logger);
    }

    protected IAmazonCloudFormation AmazonCloudFormation { get; private set; }

    protected IAmazonSQS AmazonSqs { get; private set; }

    protected IAmazonSimpleNotificationService AmazonSns { get; private set; }

    protected CloudFormationProvisioner CloudFormationProvisioner { get; private set; }

    [Fact]
    public virtual async Task CloudFormationService_Should_Create_A_CloudFormation_Stack_Async()
    {
        var stackName = Guid.NewGuid().ToString();
        const string templatePath = "./Scenarios/CloudFormation/app-resources.template";

        var cloudFormationResource = new CloudFormationResource(stackName, templatePath);
        cloudFormationResource.AddParameter("DefaultVisibilityTimeout", "30");

        // Registered before provisioning so a partially created stack is still torn down.
        TrackForCleanup(stackName, () => DeleteStackAndWaitAsync(stackName));

        await CloudFormationProvisioner.ConfigureCloudFormationAsync(cloudFormationResource);

        DescribeStacksResponse response = await AmazonCloudFormation.DescribeStacksAsync(new DescribeStacksRequest() { StackName = stackName });
        Stack? stack = response.Stacks[0];

        Assert.NotNull(stack);
        Assert.NotNull(cloudFormationResource.Outputs);
        Assert.NotEmpty(cloudFormationResource.Outputs);
        Assert.Equal(2, cloudFormationResource.Outputs.Count);

        string queueUrl = cloudFormationResource.Outputs.Single(output => output.OutputKey == "ChatMessagesQueueUrl").OutputValue;
        string snsArn = cloudFormationResource.Outputs.Single(output => output.OutputKey == "ChatTopicArn").OutputValue;

        GetTopicAttributesResponse topicAttResponse = await AmazonSns.GetTopicAttributesAsync(snsArn);

        if (topicAttResponse.HttpStatusCode == HttpStatusCode.OK)
        {
            Assert.Equal(snsArn, topicAttResponse.Attributes["TopicArn"]);
        }

        GetQueueAttributesResponse queueAttResponse = await AmazonSqs.GetQueueAttributesAsync(queueUrl, ["QueueArn"]);

        if (queueAttResponse.HttpStatusCode == HttpStatusCode.OK)
        {
            Assert.NotNull(queueAttResponse.Attributes["QueueArn"]);
        }
    }

    /// <summary>
    /// Deletes the stack and waits until CloudFormation has actually removed it.
    /// </summary>
    /// <remarks>
    /// <c>DeleteStack</c> returns as soon as the request is accepted. The SNS topic and SQS queue the template
    /// owns only disappear once deletion completes, so returning early would leave them in the shared container
    /// for whichever test runs next.
    /// </remarks>
    private async Task DeleteStackAndWaitAsync(string stackName)
    {
        await AmazonCloudFormation.DeleteStackAsync(new DeleteStackRequest { StackName = stackName }).ConfigureAwait(false);

        using var timeout = new CancellationTokenSource(StackDeletionTimeout);

        while (!timeout.IsCancellationRequested)
        {
            try
            {
                DescribeStacksResponse response =
                    await AmazonCloudFormation.DescribeStacksAsync(new DescribeStacksRequest { StackName = stackName }).ConfigureAwait(false);

                Stack? stack = response.Stacks?.FirstOrDefault();

                if (stack is null || stack.StackStatus == StackStatus.DELETE_COMPLETE)
                {
                    return;
                }
            }
            catch (AmazonCloudFormationException)
            {
                // Once the stack is fully gone CloudFormation refuses to describe it, which is the success signal.
                return;
            }

            await Task.Delay(StackDeletionPollInterval).ConfigureAwait(false);
        }

        throw new TimeoutException($"CloudFormation stack '{stackName}' was not deleted within {StackDeletionTimeout.TotalSeconds:0} seconds.");
    }
}