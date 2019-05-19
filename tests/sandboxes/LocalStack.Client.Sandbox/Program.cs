using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using LocalStack.Client.Models;
using LocalStack.Client.Utils;

namespace LocalStack.Client.Sandbox
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Session session = new Session(new SessionOptions(), new Config(), new SessionReflection());
            var amazonS3Client = session.CreateClient<AmazonS3Client>();

            var listBucketsResponse = await amazonS3Client.ListBucketsAsync();

            await CreateBucketAsync(amazonS3Client, "test-bucket");
        }

        static async Task CreateBucketAsync(AmazonS3Client s3Client, string bucketName)
        {
            try
            {
                if (!(await AmazonS3Util.DoesS3BucketExistAsync(s3Client, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
                }
                // Retrieve the bucket location.
                string bucketLocation = await FindBucketLocationAsync(s3Client, bucketName);
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

        static async Task<string> FindBucketLocationAsync(IAmazonS3 client, string bucketName)
        {
            var request = new GetBucketLocationRequest()
            {
                BucketName = bucketName
            };
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            string bucketLocation = response.Location.ToString();
            return bucketLocation;
        }
    }
}
