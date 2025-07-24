using System.Reflection;

using JsonSerializer = System.Text.Json.JsonSerializer;
using MessageAttributeValue = Amazon.SimpleNotificationService.Model.MessageAttributeValue;

namespace LocalStack.Client.Functional.Tests.Scenarios.SNS;

public abstract class BaseSnsScenario : BaseScenario
{
    protected BaseSnsScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig,
                              bool useServiceUrl = false) : base(testFixture, localStackFixture, configFile, useServiceUrl)
    {
        AmazonSimpleNotificationService = ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();
    }

    protected IAmazonSimpleNotificationService AmazonSimpleNotificationService { get; }

    [Fact]
    public async Task SnsService_Should_Create_A_Sns_Topic_Async()
    {
        var topicName = Guid.NewGuid().ToString();
        CreateTopicResponse createTopicResponse = await CreateSnsTopicAsync(topicName);

        Assert.Equal(HttpStatusCode.OK, createTopicResponse.HttpStatusCode);

        ListTopicsResponse listTopicsResponse = await AmazonSimpleNotificationService.ListTopicsAsync();
        Topic? snsTopic = listTopicsResponse.Topics?.SingleOrDefault(topic => topic.TopicArn == createTopicResponse.TopicArn);

        Assert.NotNull(snsTopic);
        Assert.EndsWith(topicName, snsTopic.TopicArn, StringComparison.Ordinal);

        await DeleteSnsTopicAsync(createTopicResponse.TopicArn); //Cleanup
    }

    [Fact]
    public async Task SnsService_Should_Delete_A_Sns_Topic_Async()
    {
        var topicName = Guid.NewGuid().ToString();

        CreateTopicResponse createTopicResponse = await CreateSnsTopicAsync(topicName);
        DeleteTopicResponse deleteTopicResponse = await DeleteSnsTopicAsync(createTopicResponse.TopicArn);

        Assert.Equal(HttpStatusCode.OK, deleteTopicResponse.HttpStatusCode);

        ListTopicsResponse listTopicsResponse = await AmazonSimpleNotificationService.ListTopicsAsync();
        bool hasAny = listTopicsResponse.Topics?.Exists(topic => topic.TopicArn == createTopicResponse.TopicArn) ?? false;

        Assert.False(hasAny);
    }

    [Fact]
    public async Task SnsService_Should_Send_Publish_A_Message_Async()
    {
        var topicName = Guid.NewGuid().ToString();
        CreateTopicResponse createTopicResponse = await CreateSnsTopicAsync(topicName);

        var jobCreatedEvent = new JobCreatedEvent(423565221, 191, 125522, "Painting Service");
        string serializedObject = JsonSerializer.Serialize(jobCreatedEvent);

        var messageAttributes = new Dictionary<string, MessageAttributeValue>(StringComparer.Ordinal)
        {
            [nameof(jobCreatedEvent.EventName)] = new() { DataType = "String", StringValue = jobCreatedEvent.EventName },
        };

        var publishRequest = new PublishRequest
        {
            Message = serializedObject, TopicArn = createTopicResponse.TopicArn, Subject = jobCreatedEvent.EventName, MessageAttributes = messageAttributes
        };

        PublishResponse publishResponse = await AmazonSimpleNotificationService.PublishAsync(publishRequest);

        Assert.Equal(HttpStatusCode.OK, publishResponse.HttpStatusCode);

        await DeleteSnsTopicAsync(createTopicResponse.TopicArn); //Cleanup
    }

    [Theory, InlineData("eu-central-1"), InlineData("us-west-1"), InlineData("af-south-1"), InlineData("ap-southeast-1"), InlineData("ca-central-1"),
     InlineData("eu-west-2"), InlineData("sa-east-1")]
    public virtual async Task Multi_Region_Tests_Async(string systemName)
    {
        var amazonSimpleNotificationService = ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();

        Type clientType = amazonSimpleNotificationService.Config.GetType();
        const string configRegionEndpointName = nameof(amazonSimpleNotificationService.Config.RegionEndpoint);
        PropertyInfo? regionEndpointProperty = clientType.GetProperty(configRegionEndpointName, BindingFlags.Public | BindingFlags.Instance);
        regionEndpointProperty?.SetValue(amazonSimpleNotificationService.Config, RegionEndpoint.GetBySystemName(systemName));

        Assert.Equal(RegionEndpoint.GetBySystemName(systemName), amazonSimpleNotificationService.Config.RegionEndpoint);

        var topicName = Guid.NewGuid().ToString();
        CreateTopicResponse createTopicResponse = await CreateSnsTopicAsync(topicName);

        Assert.Equal(HttpStatusCode.OK, createTopicResponse.HttpStatusCode);

        var topicArn = $"arn:aws:sns:{systemName}:000000000000:{topicName}";

        ListTopicsResponse listTopicsResponse = await AmazonSimpleNotificationService.ListTopicsAsync();
        Topic? snsTopic = listTopicsResponse.Topics?.SingleOrDefault(topic => topic.TopicArn == topicArn);

        Assert.NotNull(snsTopic);
        Assert.NotNull(listTopicsResponse.Topics);
        Assert.Single(listTopicsResponse.Topics);

        await DeleteSnsTopicAsync(topicArn); //Cleanup
    }

    protected async Task<CreateTopicResponse> CreateSnsTopicAsync(string topic)
    {
        var createTopicRequest = new CreateTopicRequest(topic);

        CreateTopicResponse createTopicResponse = await AmazonSimpleNotificationService.CreateTopicAsync(createTopicRequest);

        return createTopicResponse;
    }

    protected async Task<DeleteTopicResponse> DeleteSnsTopicAsync(string topic)
    {
        var deleteTopicRequest = new DeleteTopicRequest(topic);

        DeleteTopicResponse deleteTopicResponse = await AmazonSimpleNotificationService.DeleteTopicAsync(deleteTopicRequest);

        return deleteTopicResponse;
    }
}