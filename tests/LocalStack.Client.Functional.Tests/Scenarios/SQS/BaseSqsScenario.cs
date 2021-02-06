using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.SQS;
using Amazon.SQS.Model;

using LocalStack.Client.Functional.Tests.Fixtures;

using Microsoft.Extensions.DependencyInjection;

namespace LocalStack.Client.Functional.Tests.Scenarios.SQS
{
    public abstract class BaseSqsScenario : BaseScenario
    {
        protected const string TestDlQueueName = "ArmutLocalStack-Test-DLQ.fifo";
        protected const string TestQueueName = "ArmutLocalStack-Test.fifo";

        protected BaseSqsScenario(TestFixture testFixture, string configFile) : base(testFixture, configFile)
        {
            AmazonSqs = ServiceProvider.GetRequiredService<IAmazonSQS>();
        }

        protected IAmazonSQS AmazonSqs { get; set; }

        protected async Task<CreateQueueResponse> CreateQueue(string queueName = null, string dlQueueName = null)
        {
            var createDlqRequest = new CreateQueueRequest {QueueName = dlQueueName ?? TestDlQueueName, Attributes = new Dictionary<string, string> {{"FifoQueue", "true"},}};

            CreateQueueResponse createDlqResult = await AmazonSqs.CreateQueueAsync(createDlqRequest);

            GetQueueAttributesResponse attributes = await AmazonSqs.GetQueueAttributesAsync(new GetQueueAttributesRequest
            {
                QueueUrl = createDlqResult.QueueUrl,
                AttributeNames = new List<string> {"QueueArn"}
            });

            var redrivePolicy = new {maxReceiveCount = "1", deadLetterTargetArn = attributes.Attributes["QueueArn"]};

            var createQueueRequest = new CreateQueueRequest
            {
                QueueName = queueName ?? TestQueueName,
                Attributes = new Dictionary<string, string> {{"FifoQueue", "true"}, {"RedrivePolicy", JsonSerializer.Serialize(redrivePolicy)},}
            };

            return await AmazonSqs.CreateQueueAsync(createQueueRequest);
        }

        protected Task<DeleteQueueResponse> DeleteQueue(string queueUrl)
        {
            var deleteQueueRequest = new DeleteQueueRequest(queueUrl);
            return AmazonSqs.DeleteQueueAsync(deleteQueueRequest);
        }
    }
}
