namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

[Collection(nameof(LocalStackCollection))]
public class S3Scenario : BaseS3Scenario
{
    public S3Scenario(TestFixture testFixture, string configFile = TestConstants.LocalStackConfig, bool useServiceUrl = false)
        : base(testFixture, configFile, useServiceUrl)
    {
    }

    [Fact]
    public async Task S3Service_Should_Create_A_Bucket()
    {
        var bucketName = Guid.NewGuid().ToString();
        PutBucketResponse putBucketResponse = await CreateTestBucket(bucketName);

        Assert.Equal(HttpStatusCode.OK, putBucketResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_Delete_A_Bucket()
    {
        var bucketName = Guid.NewGuid().ToString();
        PutBucketResponse putBucketResponse = await CreateTestBucket(bucketName);

        Assert.Equal(HttpStatusCode.OK, putBucketResponse.HttpStatusCode);

        DeleteBucketResponse deleteBucketResponse = await DeleteTestBucket(bucketName);
        Assert.Equal(HttpStatusCode.NoContent, deleteBucketResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_Upload_A_File_To_A_Bucket()
    {
        var bucketName = Guid.NewGuid().ToString();
        await CreateTestBucket(bucketName);
        await UploadTestFile(bucketName: bucketName, key: Key);

        GetObjectResponse getObjectResponse = await AmazonS3.GetObjectAsync(bucketName, Key);

        Assert.Equal(HttpStatusCode.OK, getObjectResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_Delete_A_File_To_A_Bucket()
    {
        var bucketName = Guid.NewGuid().ToString();
        await CreateTestBucket(bucketName);
        await UploadTestFile(bucketName: bucketName, key: Key);

        DeleteObjectResponse deleteObjectResponse = await AmazonS3.DeleteObjectAsync(bucketName, Key);

        Assert.Equal(HttpStatusCode.NoContent, deleteObjectResponse.HttpStatusCode);
    }

    [Fact]
    public async Task S3Service_Should_List_Files_In_A_Bucket()
    {
        var bucketName = Guid.NewGuid().ToString();
        await CreateTestBucket(bucketName);

        const int uploadCount = 4;
        var fileNames = new string[uploadCount];

        for (var i = 0; i < uploadCount; i++)
        {
            var fileName = $"SampleData{i}.txt";

            await UploadTestFile(fileName, bucketName);
            fileNames[i] = fileName;
        }

        ListObjectsResponse listObjectsResponse = await AmazonS3.ListObjectsAsync(bucketName);
        List<S3Object> s3Objects = listObjectsResponse.S3Objects;

        Assert.Equal(uploadCount, s3Objects.Count);
        Assert.All(fileNames, s => Assert.NotNull(s3Objects.FirstOrDefault(o => o.Key == s)));
    }
}
