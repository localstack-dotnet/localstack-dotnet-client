using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

using LocalStack.Client.Contracts;

using System;
using System.Threading.Tasks;

using LocalStack.Client.Options;

namespace LocalStack.Client.Sandbox
{
    internal static class Program
    {
        private static async Task Main()
        {
            /*
             * ==== Default Values ====
             * AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
             * AwsAccessKey: secretKey (It doesn't matter to LocalStack)
             * AwsSessionToken: token (It doesn't matter to LocalStack)
             * RegionName: us-east-1
             * ==== Custom Values ====
             * var sessionOptions = new SessionOptions("someAwsAccessKeyId", "someAwsAccessKey", "someAwsSessionToken", "eu-central-");
             */
            var sessionOptions = new SessionOptions();

            /*
             * ==== Default Values ====
             * LocalStackHost: localhost
             * UseSsl: false
             * UseLegacyPorts: false (Set true if your LocalStack version is 0.11.5 or above)
             * EdgePort: 4566 (It doesn't matter if use legacy ports)
             * ==== Custom Values ====
             * var configOptions = new ConfigOptions("mylocalhost", false, false, 4566);
             */
            var configOptions = new ConfigOptions();

            ISession session = SessionStandalone.Init()
                                                .WithSessionOptions(sessionOptions)
                                                .WithConfigurationOptions(configOptions).Create();

            var amazonS3Client = session.CreateClientByImplementation<AmazonS3Client>();

            const string bucketName = "test-bucket-3";
            const string filePath = "SampleData.txt";
            const string key = "SampleData.txt";

            Console.WriteLine("Press any key to start Sandbox application");
            Console.ReadLine();

            await CreateBucketAndUploadFileAsync(amazonS3Client, bucketName, filePath, key);
        }

        private static async Task CreateBucketAndUploadFileAsync(IAmazonS3 s3Client, string bucketName, string path, string key)
        {
            try
            {
                var putBucketRequest = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };
                PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);

                Console.WriteLine("The bucket {0} created", bucketName);

                // Retrieve the bucket location.
                string bucketLocation = await FindBucketLocationAsync(s3Client, bucketName);
                Console.WriteLine("The bucket's location: {0}", bucketLocation);

                var fileTransferUtility = new TransferUtility(s3Client);

                Console.WriteLine("Uploading the file {0}...", path);
                await fileTransferUtility.UploadAsync(path, bucketName, key);
                Console.WriteLine("The file {0} created", path);
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
            var request = new GetBucketLocationRequest() { BucketName = bucketName };
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            var bucketLocation = response.Location.ToString();

            return bucketLocation;
        }
    }
}