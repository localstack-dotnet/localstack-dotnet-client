namespace LocalStack.Client.Functional.Tests.Scenarios.SNS;

public abstract class BaseSnsScenario : BaseScenario
{
    protected BaseSnsScenario(TestFixture testFixture, string configFile, bool useServiceUrl = false) 
        : base(testFixture, configFile, useServiceUrl)
    {
        AmazonSimpleNotificationService = ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();
    }

    protected IAmazonSimpleNotificationService AmazonSimpleNotificationService { get; }

    protected async Task<CreateTopicResponse> CreateSnsTopic(string topic)
    {
        var createTopicRequest = new CreateTopicRequest(topic);

        CreateTopicResponse createTopicResponse = await AmazonSimpleNotificationService.CreateTopicAsync(createTopicRequest);

        return createTopicResponse;
    }

    protected async Task<DeleteTopicResponse> DeleteSnsTopic(string topic)
    {
        var deleteTopicRequest = new DeleteTopicRequest(topic);

        DeleteTopicResponse deleteTopicResponse = await AmazonSimpleNotificationService.DeleteTopicAsync(deleteTopicRequest);

        return deleteTopicResponse;
    }
}