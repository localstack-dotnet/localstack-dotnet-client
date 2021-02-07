using System;
using System.Threading;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LocalStack.Client.Sandbox.WithGenericHost
{
    public class SampleS3Service : IHostedService
    {
        private const string BucketName = "test-bucket-3";
        private const string FilePath = "SampleData.txt";
        private const string Key = "SampleData.txt";

        private readonly ILogger<SampleS3Service> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAmazonS3 _amazonS3;

        public SampleS3Service(ILogger<SampleS3Service> logger, IHostApplicationLifetime hostApplicationLifetime, IAmazonS3 amazonS3)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _amazonS3 = amazonS3;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var putBucketRequest = new PutBucketRequest {BucketName = BucketName, UseClientRegion = true};
                PutBucketResponse putBucketResponse = await _amazonS3.PutBucketAsync(putBucketRequest, cancellationToken);

                _logger.LogInformation("The bucket {0} created", BucketName);

                // Retrieve the bucket location.
                string bucketLocation = await FindBucketLocationAsync(_amazonS3, BucketName);
                _logger.LogInformation("The bucket's location: {0}", bucketLocation);

                var fileTransferUtility = new TransferUtility(_amazonS3);

                _logger.LogInformation("Uploading the file {0}...", FilePath);
                await fileTransferUtility.UploadAsync(FilePath, BucketName, Key, cancellationToken);
                _logger.LogInformation("The file {0} created", FilePath);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping the application");
            return Task.CompletedTask;
        }

        private static async Task<string> FindBucketLocationAsync(IAmazonS3 client, string bucketName)
        {
            var request = new GetBucketLocationRequest() { BucketName = bucketName };
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            var bucketLocation = response.Location.ToString();

            return bucketLocation;
        }
    }
}
