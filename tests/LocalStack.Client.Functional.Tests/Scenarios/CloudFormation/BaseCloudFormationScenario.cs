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
            catch (AmazonCloudFormationException exception) when (IsStackGone(exception))
            {
                // Once the stack is fully gone CloudFormation refuses to describe it, which is the success signal.
                return;
            }

            await Task.Delay(StackDeletionPollInterval).ConfigureAwait(false);
        }

        throw new TimeoutException($"CloudFormation stack '{stackName}' was not deleted within {StackDeletionTimeout.TotalSeconds:0} seconds.");
    }

    /// <summary>
    /// Distinguishes "the stack is gone" from every other CloudFormation failure.
    /// </summary>
    /// <remarks>
    /// Catching <see cref="AmazonCloudFormationException" /> indiscriminately would also swallow throttling, credential
    /// and service errors, reporting a clean teardown while the stack - and the SNS topic and SQS queue its template
    /// owns - stayed behind in the shared container for the next test.
    /// <para>
    /// The error code below is measured, not assumed. Against both LocalStack versions these scenarios run on, creating
    /// a stack, deleting it and then describing it by name gives:
    /// </para>
    /// <code>
    /// AmazonCloudFormationException
    ///   ErrorCode  = ValidationError
    ///   StatusCode = 400 BadRequest
    ///   Message    = Stack with id &lt;name&gt; does not exist
    /// </code>
    /// <para>
    /// Identical on 3.7.1 and 4.6.0, and the throw arrives on the first poll - LocalStack drops the stack immediately
    /// rather than parking it in <c>DELETE_COMPLETE</c>, so this catch really is the loop's exit path, not a fallback.
    /// If a future LocalStack starts reporting <c>DELETE_COMPLETE</c> instead, the status check above already covers it.
    /// </para>
    /// </remarks>
    private static bool IsStackGone(AmazonCloudFormationException exception)
    {
        return string.Equals(exception.ErrorCode, "ValidationError", StringComparison.Ordinal);
    }
}