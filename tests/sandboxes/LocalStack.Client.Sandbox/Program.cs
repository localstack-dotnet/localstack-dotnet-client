using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;

using LocalStack.Client.Contracts;

using System;
using System.Threading.Tasks;

namespace LocalStack.Client.Sandbox
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var awsAccessKeyId = "Key Id";
            var awsAccessKey = "Secret Key";
            var awsSessionToken = "Token";
            var regionName = "us-west-1";
            var localStackHost = "localhost";

            ISession session = SessionStandalone.Init().WithSessionOptions(awsAccessKeyId, awsAccessKey, awsSessionToken, regionName).WithConfig(localStackHost).Create();

            var amazonS3Client = session.CreateClient<AmazonS3Client>();

            const string bucketName = "test-bucket-3";
            const string filePath = "SampleData.txt";
            const string key = "SampleData.txt";

            await CreateBucketAndUploadFileAsync(amazonS3Client, bucketName, filePath, key);
        }

        private static async Task CreateBucketAndUploadFileAsync(IAmazonS3 s3Client, string bucketName, string path, string key)
        {
            try
            {
                if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName))
                {
                    var putBucketRequest = new PutBucketRequest {BucketName = bucketName, UseClientRegion = true};

                    PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
                }

                // Retrieve the bucket location.
                string bucketLocation = await FindBucketLocationAsync(s3Client, bucketName);

                var fileTransferUtility = new TransferUtility(s3Client);

                await fileTransferUtility.UploadAsync(path, bucketName, key);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        private static async Task<string> FindBucketLocationAsync(IAmazonS3 client, string bucketName)
        {
            var request = new GetBucketLocationRequest() {BucketName = bucketName};
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            string bucketLocation = response.Location.ToString();

            return bucketLocation;
        }
    }
}