using System.Diagnostics.CodeAnalysis;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LocalStack.Client.Functional.Tests.Scenarios.SQS;

public abstract class BaseSqsScenario : BaseScenario
{
    protected const string TestDlQueueName = "ArmutLocalStack-Test-DLQ.fifo";
    protected const string TestQueueName = "ArmutLocalStack-Test.fifo";

    protected BaseSqsScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig,
                              bool useServiceUrl = false) : base(testFixture, localStackFixture, configFile, useServiceUrl)
    {
        AmazonSqs = ServiceProvider.GetRequiredService<IAmazonSQS>();
    }

    protected IAmazonSQS AmazonSqs { get; set; }

    [Fact]
    public async Task AmazonSqsService_Should_Create_A_Queue_Async()
    {
        var guid = Guid.NewGuid();
        var queueName = $"{guid}.fifo";
        var dlQueueName = $"{guid}-DLQ.fifo";

        CreateQueueResponse createQueueResponse = await CreateFifoQueueWithRedriveAsync(queueName, dlQueueName).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, createQueueResponse.HttpStatusCode);
    }

    [Fact]
    public async Task AmazonSqsService_Should_Delete_A_Queue_Async()
    {
        var guid = Guid.NewGuid();
        var queueName = $"{guid}.fifo";
        var dlQueueName = $"{guid}-DLQ.fifo";

        CreateQueueResponse createQueueResponse = await CreateFifoQueueWithRedriveAsync(queueName, dlQueueName).ConfigureAwait(false);
        DeleteQueueResponse deleteQueueResponse = await DeleteQueueAsync(createQueueResponse.QueueUrl).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, deleteQueueResponse.HttpStatusCode);
    }

    [Fact]
    public async Task AmazonSqsService_Should_Send_A_Message_To_A_Queue_Async()
    {
        var guid = Guid.NewGuid();
        var queueName = $"{guid}.fifo";
        var dlQueueName = $"{guid}-DLQ.fifo";

        CreateQueueResponse createQueueResponse = await CreateFifoQueueWithRedriveAsync(queueName, dlQueueName).ConfigureAwait(false);

        var commentModel = new Fixture().Create<CommentModel>();
        string serializedModel = JsonSerializer.Serialize(commentModel);

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = createQueueResponse.QueueUrl,
            MessageGroupId = commentModel.MovieId.ToString(),
            MessageDeduplicationId = Guid.NewGuid().ToString(),
            MessageBody = serializedModel,
        };

        SendMessageResponse messageResponse = await AmazonSqs.SendMessageAsync(sendMessageRequest).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, messageResponse.HttpStatusCode);
    }

    [Fact]
    public async Task AmazonSqsService_Should_Receive_Messages_From_A_Queue_Async()
    {
        var guid = Guid.NewGuid();
        var queueName = $"{guid}.fifo";
        var dlQueueName = $"{guid}-DLQ.fifo";

        CreateQueueResponse createQueueResponse = await CreateFifoQueueWithRedriveAsync(queueName, dlQueueName).ConfigureAwait(false);

        var commentModel = new Fixture().Create<CommentModel>();
        string serializedModel = JsonSerializer.Serialize(commentModel);

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = createQueueResponse.QueueUrl,
            MessageGroupId = commentModel.MovieId.ToString(),
            MessageDeduplicationId = Guid.NewGuid().ToString(),
            MessageBody = serializedModel
        };

        await AmazonSqs.SendMessageAsync(sendMessageRequest).ConfigureAwait(false);

        var req = new ReceiveMessageRequest { MaxNumberOfMessages = 1, QueueUrl = createQueueResponse.QueueUrl };

        ReceiveMessageResponse receiveMessages = await AmazonSqs.ReceiveMessageAsync(req).ConfigureAwait(false);
        Assert.Equal(HttpStatusCode.OK, receiveMessages.HttpStatusCode);

        Message? currentMessage = receiveMessages.Messages.FirstOrDefault();
        Assert.NotNull(currentMessage);

        var deserializedComment = JsonSerializer.Deserialize<CommentModel>(currentMessage.Body);
        Assert.NotNull(deserializedComment);
        Assert.True(commentModel.DeepEquals(deserializedComment));
    }

    protected async Task<CreateQueueResponse> CreateFifoQueueWithRedriveAsync(string? queueName = null, string? dlQueueName = null)
    {
        var createDlqRequest = new CreateQueueRequest
        {
            QueueName = dlQueueName ?? TestDlQueueName, Attributes = new Dictionary<string, string>(StringComparer.Ordinal) { { "FifoQueue", "true" }, },
        };

        CreateQueueResponse createDlqResult = await AmazonSqs.CreateQueueAsync(createDlqRequest).ConfigureAwait(false);

        GetQueueAttributesResponse attributes = await AmazonSqs.GetQueueAttributesAsync(new GetQueueAttributesRequest
                                                               {
                                                                   QueueUrl = createDlqResult.QueueUrl,
                                                                   AttributeNames = new List<string> { "QueueArn" },
                                                               })
                                                               .ConfigureAwait(false);

        var redrivePolicy = new { maxReceiveCount = "1", deadLetterTargetArn = attributes.Attributes["QueueArn"] };

        var createQueueRequest = new CreateQueueRequest
        {
            QueueName = queueName ?? TestQueueName,
            Attributes = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "FifoQueue", "true" }, { "RedrivePolicy", JsonSerializer.Serialize(redrivePolicy) },
            },
        };

        return await AmazonSqs.CreateQueueAsync(createQueueRequest).ConfigureAwait(false);
    }

    protected async Task<CreateQueueResponse> CreateQueueAsync(string? queueName = null)
    {
        var createQueueRequest = new CreateQueueRequest(queueName ?? TestQueueName);

        return await AmazonSqs.CreateQueueAsync(createQueueRequest).ConfigureAwait(false);
    }

    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    protected async Task<DeleteQueueResponse> DeleteQueueAsync(string queueUrl)
    {
        var deleteQueueRequest = new DeleteQueueRequest(queueUrl);

        return await AmazonSqs.DeleteQueueAsync(deleteQueueRequest).ConfigureAwait(false);
    }
}