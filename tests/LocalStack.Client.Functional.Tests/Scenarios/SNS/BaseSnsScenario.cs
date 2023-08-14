using JsonSerializer = System.Text.Json.JsonSerializer;
using MessageAttributeValue = Amazon.SimpleNotificationService.Model.MessageAttributeValue;

namespace LocalStack.Client.Functional.Tests.Scenarios.SNS;

public abstract class BaseSnsScenario : BaseScenario
{
    protected BaseSnsScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig, bool useServiceUrl = false) : base(
        testFixture, localStackFixture, configFile, useServiceUrl)
    {
        AmazonSimpleNotificationService = ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();
    }

    protected IAmazonSimpleNotificationService AmazonSimpleNotificationService { get; }

    [Fact]
    public async Task SnsService_Should_Create_A_Sns_Topic()
    {
        var topicName = Guid.NewGuid().ToString();
        CreateTopicResponse createTopicResponse = await CreateSnsTopic(topicName);

        Assert.Equal(HttpStatusCode.OK, createTopicResponse.HttpStatusCode);

        ListTopicsResponse listTopicsResponse = await AmazonSimpleNotificationService.ListTopicsAsync();
        Topic snsTopic = listTopicsResponse.Topics.SingleOrDefault(topic => topic.TopicArn == createTopicResponse.TopicArn);

        Assert.NotNull(snsTopic);
        Assert.EndsWith(topicName, snsTopic.TopicArn);

        await DeleteSnsTopic(createTopicResponse.TopicArn); //Cleanup
    }

    [Fact]
    public async Task SnsService_Should_Delete_A_Sns_Topic()
    {
        var topicName = Guid.NewGuid().ToString();

        CreateTopicResponse createTopicResponse = await CreateSnsTopic(topicName);
        DeleteTopicResponse deleteTopicResponse = await DeleteSnsTopic(createTopicResponse.TopicArn);

        Assert.Equal(HttpStatusCode.OK, deleteTopicResponse.HttpStatusCode);

        ListTopicsResponse listTopicsResponse = await AmazonSimpleNotificationService.ListTopicsAsync();
        bool hasAny = listTopicsResponse.Topics.Any(topic => topic.TopicArn == createTopicResponse.TopicArn);

        Assert.False(hasAny);
    }

    [Fact]
    public async Task SnsService_Should_Send_Publish_A_Message()
    {
        var topicName = Guid.NewGuid().ToString();
        CreateTopicResponse createTopicResponse = await CreateSnsTopic(topicName);

        var jobCreatedEvent = new JobCreatedEvent(423565221, 191, 125522, "Painting Service");
        string serializedObject = JsonSerializer.Serialize(jobCreatedEvent);

        var messageAttributes = new Dictionary<string, MessageAttributeValue>
        {
            { nameof(jobCreatedEvent.EventName), new MessageAttributeValue { DataType = "String", StringValue = jobCreatedEvent.EventName } }
        };

        var publishRequest = new PublishRequest
        {
            Message = serializedObject, TopicArn = createTopicResponse.TopicArn, Subject = jobCreatedEvent.EventName, MessageAttributes = messageAttributes
        };

        PublishResponse publishResponse = await AmazonSimpleNotificationService.PublishAsync(publishRequest);

        Assert.Equal(HttpStatusCode.OK, publishResponse.HttpStatusCode);

        await DeleteSnsTopic(createTopicResponse.TopicArn); //Cleanup
    }

    [Theory, InlineData("eu-central-1"), InlineData("us-west-1"), InlineData("af-south-1"), InlineData("ap-southeast-1"), InlineData("ca-central-1"),
     InlineData("eu-west-2"), InlineData("sa-east-1")]
    public virtual async Task Multi_Region_Tests(string systemName)
    {
        var sessionReflection = ServiceProvider.GetRequiredService<ISessionReflection>();
        var amazonSimpleNotificationService = ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();

        sessionReflection.SetClientRegion((AmazonSimpleNotificationServiceClient)amazonSimpleNotificationService, systemName);

        Assert.Equal(RegionEndpoint.GetBySystemName(systemName), amazonSimpleNotificationService.Config.RegionEndpoint);

        var topicName = Guid.NewGuid().ToString();
        CreateTopicResponse createTopicResponse = await CreateSnsTopic(topicName);

        Assert.Equal(HttpStatusCode.OK, createTopicResponse.HttpStatusCode);

        var topicArn = $"arn:aws:sns:{systemName}:000000000000:{topicName}";

        ListTopicsResponse listTopicsResponse = await AmazonSimpleNotificationService.ListTopicsAsync();
        Topic snsTopic = listTopicsResponse.Topics.SingleOrDefault(topic => topic.TopicArn == topicArn);

        Assert.NotNull(snsTopic);
        Assert.Single(listTopicsResponse.Topics);

        await DeleteSnsTopic(topicArn); //Cleanup
    }

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