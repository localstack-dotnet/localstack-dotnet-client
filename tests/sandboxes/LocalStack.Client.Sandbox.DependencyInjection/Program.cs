using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

using LocalStack.Client.Contracts;
using LocalStack.Client.Utils;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Threading.Tasks;

using LocalStack.Client.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LocalStack.Client.Sandbox.DependencyInjection
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", true);
            builder.AddJsonFile("appsettings.Development.json", true);
            builder.AddEnvironmentVariables();
            builder.AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            collection.Configure<LocalStackOptions>(options => configuration.GetSection("LocalStack").Bind(options, c => c.BindNonPublicProperties = true));
            /*
            * ==== Default Values ====
            * AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
            * AwsAccessKey: secretKey (It doesn't matter to LocalStack)
            * AwsSessionToken: token (It doesn't matter to LocalStack)
            * RegionName: us-east-1
             */
            collection.Configure<SessionOptions>(options => configuration.GetSection("LocalStack")
                                                                         .GetSection(nameof(LocalStackOptions.Session))
                                                                         .Bind(options, c => c.BindNonPublicProperties = true));
            /*
             * ==== Default Values ====
             * LocalStackHost: localhost
             * UseSsl: false
             * UseLegacyPorts: false (Set true if your LocalStack version is 0.11.4 or below)
             * EdgePort: 4566 (It doesn't matter if use legacy ports)
             */
            collection.Configure<ConfigOptions>(options => configuration.GetSection("LocalStack")
                                                                        .GetSection(nameof(LocalStackOptions.Config))
                                                                        .Bind(options, c => c.BindNonPublicProperties = true));


            collection.AddTransient<IConfig, Config>(provider =>
                      {
                          ConfigOptions options = provider.GetRequiredService<IOptions<ConfigOptions>>().Value;

                          return new Config(options);
                      })
                      .AddSingleton<ISessionReflection, SessionReflection>()
                      .AddSingleton<ISession, Session>(provider =>
                      {
                          SessionOptions sessionOptions = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
                          var config = provider.GetRequiredService<IConfig>();
                          var sessionReflection = provider.GetRequiredService<ISessionReflection>();

                          return new Session(sessionOptions, config, sessionReflection);
                      })
                      .AddTransient<IAmazonS3>(provider =>
                      {
                          var session = provider.GetRequiredService<ISession>();

                          return (IAmazonS3) session.CreateClientByInterface<IAmazonS3>();
                      });

            ServiceProvider serviceProvider = collection.BuildServiceProvider();

            var amazonS3Client = serviceProvider.GetRequiredService<IAmazonS3>();

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
                var putBucketRequest = new PutBucketRequest {BucketName = bucketName, UseClientRegion = true};
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
            var request = new GetBucketLocationRequest() {BucketName = bucketName};
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            var bucketLocation = response.Location.ToString();

            return bucketLocation;
        }
    }
}