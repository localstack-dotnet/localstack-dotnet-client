namespace LocalStack.Client.Sandbox.WithGenericHost;

public class SampleS3Service : IHostedService
{
    private const string BucketName = "localstack-sandbox-with-host";
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
            var putBucketRequest = new PutBucketRequest { BucketName = BucketName };
            await _amazonS3.PutBucketAsync(putBucketRequest, cancellationToken);

            _logger.LogInformation("The bucket {BucketName} created", BucketName);

            // Retrieve the bucket location.
            string bucketLocation = await FindBucketLocationAsync(_amazonS3, BucketName);
            _logger.LogInformation("The bucket's location: {BucketLocation}", bucketLocation);

            using var fileTransferUtility = new TransferUtility(_amazonS3);

            _logger.LogInformation("Uploading the file {FilePath}...", FilePath);
            await fileTransferUtility.UploadAsync(FilePath, BucketName, Key, cancellationToken);
            _logger.LogInformation("The file {FilePath} created", FilePath);
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, "Error encountered on server. Message:'{EMessage}' when writing an object", e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unknown encountered on server. Message:'{EMessage}' when writing an object", e.Message);
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
