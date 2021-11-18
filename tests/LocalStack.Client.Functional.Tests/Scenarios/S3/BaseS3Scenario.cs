namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

public abstract class BaseS3Scenario : BaseScenario
{
    protected const string BucketName = "test-bucket-3";
    private const string FilePath = "SampleData.txt";
    protected const string Key = "SampleData.txt";

    protected BaseS3Scenario(TestFixture testFixture, string configFile)
        : base(testFixture, configFile)
    {
        AmazonS3 = ServiceProvider.GetRequiredService<IAmazonS3>();
    }

    protected IAmazonS3 AmazonS3 { get; private set; }

    protected Task<PutBucketResponse> CreateTestBucket(string bucketName = null)
    {
        var putBucketRequest = new PutBucketRequest { BucketName = bucketName ?? BucketName, UseClientRegion = true };
        return AmazonS3.PutBucketAsync(putBucketRequest);
    }

    protected Task<DeleteBucketResponse> DeleteTestBucket(string bucketName = null)
    {
        var deleteBucketRequest = new DeleteBucketRequest { BucketName = bucketName ?? BucketName, UseClientRegion = true };
        return AmazonS3.DeleteBucketAsync(deleteBucketRequest);
    }

    protected Task UploadTestFile(string key = null, string bucketName = null)
    {
        var fileTransferUtility = new TransferUtility(AmazonS3);
        return fileTransferUtility.UploadAsync(FilePath, bucketName ?? BucketName, key ?? Key);
    }
}
