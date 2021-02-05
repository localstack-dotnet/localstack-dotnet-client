using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;

using LocalStack.Client.Contracts;
using LocalStack.Client.Utils;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

using LocalStack.Client.Options;

namespace LocalStack.Client.Sandbox.DependencyInjection
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();

            collection
                /*
                * ==== Default Values ====
                * AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
                * AwsAccessKey: secretKey (It doesn't matter to LocalStack)
                * AwsSessionToken: token (It doesn't matter to LocalStack)
                * RegionName: us-east-1
                * ==== Custom Values ====
                * .AddScoped<ISessionOptions, SessionOptions>(provider => new SessionOptions("someAwsAccessKeyId", "someAwsAccessKey", "someAwsSessionToken", "eu-central-"))
                */
                .AddScoped<ISessionOptions, SessionOptions>()

                /*
                 * ==== Default Values ====
                 * LocalStackHost: localhost
                 * UseSsl: false
                 * UseLegacyPorts: false (Set true if your LocalStack version is 0.11.5 or above)
                 * EdgePort: 4566 (It doesn't matter if use legacy ports)
                 * ==== Custom Values ====
                 * .AddScoped<IConfigOptions, ConfigOptions>(provider => new ConfigOptions("mylocalhost", false, false, 4566))
                 */
                .AddScoped<IConfigOptions, ConfigOptions>()
                .AddScoped<IConfig, Config>()
                .AddScoped<ISessionReflection, SessionReflection>()
                .AddScoped<ISession, Session>();

            ServiceProvider serviceProvider = collection.BuildServiceProvider();
            var session = serviceProvider.GetRequiredService<ISession>();

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
                    var putBucketRequest = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };

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
            var request = new GetBucketLocationRequest() { BucketName = bucketName };
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            var bucketLocation = response.Location.ToString();

            return bucketLocation;
        }
    }
}