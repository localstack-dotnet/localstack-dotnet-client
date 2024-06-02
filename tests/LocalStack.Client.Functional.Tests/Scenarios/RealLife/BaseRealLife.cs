using MessageAttributeValue = Amazon.SimpleNotificationService.Model.MessageAttributeValue;

namespace LocalStack.Client.Functional.Tests.Scenarios.RealLife;

public abstract class BaseRealLife : BaseScenario
{
    protected BaseRealLife(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig, bool useServiceUrl = false)
        : base(testFixture, localStackFixture, configFile, useServiceUrl)
    {
        AmazonSimpleNotificationService = ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();
        AmazonSqs = ServiceProvider.GetRequiredService<IAmazonSQS>();
    }

    protected IAmazonSimpleNotificationService AmazonSimpleNotificationService { get; set; }

    protected IAmazonSQS AmazonSqs { get; set; }

    [Fact, SuppressMessage("Test", "MA0051:Method is too long", Justification = "Test method")]
    public virtual async Task
        Should_Create_A_SNS_Topic_And_SQS_Queue_Then_Subscribe_To_The_Topic_Using_SQS_Then_Publish_A_Message_To_Topic_And_Read_It_From_The_Queue_Async()
    {
        var topicName = Guid.NewGuid().ToString();
        var queueName = Guid.NewGuid().ToString();
        var jobCreatedEvent = new JobCreatedEvent(423565221, 191, 125522, "Painting Service");

        var createTopicRequest = new CreateTopicRequest(topicName);
        CreateTopicResponse createTopicResponse = await AmazonSimpleNotificationService.CreateTopicAsync(createTopicRequest);

        Assert.Equal(HttpStatusCode.OK, createTopicResponse.HttpStatusCode);

        var createQueueRequest = new CreateQueueRequest(queueName);
        CreateQueueResponse createQueueResponse = await AmazonSqs.CreateQueueAsync(createQueueRequest);

        Assert.Equal(HttpStatusCode.OK, createQueueResponse.HttpStatusCode);

        const string queueArnAttribute = "QueueArn";
        var getQueueAttributesRequest = new GetQueueAttributesRequest(createQueueResponse.QueueUrl, new List<string> { queueArnAttribute });
        GetQueueAttributesResponse getQueueAttributesResponse = await AmazonSqs.GetQueueAttributesAsync(getQueueAttributesRequest);

        Assert.Equal(HttpStatusCode.OK, getQueueAttributesResponse.HttpStatusCode);

        string queueArn = getQueueAttributesResponse.Attributes[queueArnAttribute];

        var subscribeRequest = new SubscribeRequest(createTopicResponse.TopicArn, "sqs", queueArn);
        SubscribeResponse subscribeResponse = await AmazonSimpleNotificationService.SubscribeAsync(subscribeRequest);

        Assert.Equal(HttpStatusCode.OK, subscribeResponse.HttpStatusCode);

        string serializedObject = JsonConvert.SerializeObject(jobCreatedEvent);
        var messageAttributes = new Dictionary<string, MessageAttributeValue>(StringComparer.Ordinal)
        {
            { nameof(jobCreatedEvent.EventName), new MessageAttributeValue { DataType = "String", StringValue = jobCreatedEvent.EventName } },
        };

        var publishRequest = new PublishRequest
        {
            Message = serializedObject, TopicArn = createTopicResponse.TopicArn, Subject = jobCreatedEvent.EventName, MessageAttributes = messageAttributes
        };

        PublishResponse publishResponse = await AmazonSimpleNotificationService.PublishAsync(publishRequest);

        Assert.Equal(HttpStatusCode.OK, publishResponse.HttpStatusCode);

        var receiveMessageRequest = new ReceiveMessageRequest(createQueueResponse.QueueUrl);
        ReceiveMessageResponse receiveMessageResponse = await AmazonSqs.ReceiveMessageAsync(receiveMessageRequest);

        Assert.Equal(HttpStatusCode.OK, receiveMessageResponse.HttpStatusCode);

        if (receiveMessageResponse.Messages.Count == 0)
        {
            await Task.Delay(2000);
            receiveMessageResponse = await AmazonSqs.ReceiveMessageAsync(receiveMessageRequest);

            Assert.Equal(HttpStatusCode.OK, receiveMessageResponse.HttpStatusCode);
        }

        Assert.NotNull(receiveMessageResponse.Messages);
        Assert.NotEmpty(receiveMessageResponse.Messages);
        Assert.Single(receiveMessageResponse.Messages);

        dynamic? deserializedMessage = JsonConvert.DeserializeObject<ExpandoObject>(receiveMessageResponse.Messages[0].Body, new ExpandoObjectConverter());

        Assert.NotNull(deserializedMessage);
        Assert.NotNull(deserializedMessage!.MessageId);
        Assert.Equal(publishResponse.MessageId, (string)deserializedMessage.MessageId);

        JobCreatedEvent sqsJobCreatedEvent = JsonConvert.DeserializeObject<JobCreatedEvent>(deserializedMessage.Message);

        Assert.NotNull(sqsJobCreatedEvent);
        Assert.Equal(jobCreatedEvent.EventName, sqsJobCreatedEvent.EventName);
        Assert.Equal(jobCreatedEvent.Description, sqsJobCreatedEvent.Description);
        Assert.Equal(jobCreatedEvent.JobId, sqsJobCreatedEvent.JobId);
        Assert.Equal(jobCreatedEvent.ServiceId, sqsJobCreatedEvent.ServiceId);
        Assert.Equal(jobCreatedEvent.UserId, sqsJobCreatedEvent.UserId);
    }
}