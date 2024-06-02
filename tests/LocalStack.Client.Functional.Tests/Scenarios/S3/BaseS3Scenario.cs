namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

public abstract class BaseS3Scenario : BaseScenario
{
    protected const string BucketName = "test-bucket-3";
    private const string FilePath = "SampleData.txt";
    protected const string Key = "SampleData.txt";

    protected BaseS3Scenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig,
                             bool useServiceUrl = false) : base(testFixture, localStackFixture, configFile, useServiceUrl)
    {
        AmazonS3 = ServiceProvider.GetRequiredService<IAmazonS3>();
    }

    protected IAmazonS3 AmazonS3 { get; private set; }

    [Fact]
    public async Task S3Service_Should_Create_A_Bucket_Async()
    {
        var bucketName = Guid.NewGuid().ToString();
        PutBucketResponse putBucketResponse = await CreateTestBucketAsync(bucketName);

        Assert.Equal(HttpStatusCode.OK, putBucketResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_Delete_A_Bucket_Async()
    {
        var bucketName = Guid.NewGuid().ToString();
        PutBucketResponse putBucketResponse = await CreateTestBucketAsync(bucketName);

        Assert.Equal(HttpStatusCode.OK, putBucketResponse.HttpStatusCode);

        DeleteBucketResponse deleteBucketResponse = await DeleteTestBucketAsync(bucketName);
        Assert.Equal(HttpStatusCode.NoContent, deleteBucketResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_Upload_A_File_To_A_Bucket_Async()
    {
        var bucketName = Guid.NewGuid().ToString();
        await CreateTestBucketAsync(bucketName);
        await UploadTestFileAsync(key: Key, bucketName: bucketName);

        GetObjectResponse getObjectResponse = await AmazonS3.GetObjectAsync(bucketName, Key);

        Assert.Equal(HttpStatusCode.OK, getObjectResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_Delete_A_File_To_A_Bucket_Async()
    {
        var bucketName = Guid.NewGuid().ToString();
        await CreateTestBucketAsync(bucketName);
        await UploadTestFileAsync(key: Key, bucketName: bucketName);

        DeleteObjectResponse deleteObjectResponse = await AmazonS3.DeleteObjectAsync(bucketName, Key);

        Assert.Equal(HttpStatusCode.NoContent, deleteObjectResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_List_Files_In_A_Bucket_Async()
    {
        var bucketName = Guid.NewGuid().ToString();
        await CreateTestBucketAsync(bucketName);

        const int uploadCount = 4;
        var fileNames = new string[uploadCount];

        for (var i = 0; i < uploadCount; i++)
        {
            var fileName = $"SampleData{i}.txt";

            await UploadTestFileAsync(fileName, bucketName);
            fileNames[i] = fileName;
        }

        ListObjectsResponse listObjectsResponse = await AmazonS3.ListObjectsAsync(bucketName);
        List<S3Object> s3Objects = listObjectsResponse.S3Objects;

        Assert.Equal(uploadCount, s3Objects.Count);
        Assert.All(fileNames, s => Assert.NotNull(s3Objects.Find(o => o.Key == s)));
    }

    protected Task<PutBucketResponse> CreateTestBucketAsync(string? bucketName = null)
    {
        var putBucketRequest = new PutBucketRequest { BucketName = bucketName ?? BucketName, UseClientRegion = true };

        return AmazonS3.PutBucketAsync(putBucketRequest);
    }

    protected Task<DeleteBucketResponse> DeleteTestBucketAsync(string? bucketName = null)
    {
        var deleteBucketRequest = new DeleteBucketRequest { BucketName = bucketName ?? BucketName, UseClientRegion = true };

        return AmazonS3.DeleteBucketAsync(deleteBucketRequest);
    }

    protected async Task UploadTestFileAsync(string? key = null, string? bucketName = null)
    {
        using var fileTransferUtility = new TransferUtility(AmazonS3);

        await fileTransferUtility.UploadAsync(FilePath, bucketName ?? BucketName, key ?? Key);
    }
}