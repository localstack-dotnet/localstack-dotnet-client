using System.Runtime.CompilerServices;
using Amazon.S3;
using LocalStack.Client.Utils;

namespace LocalStack.Client.AotCompatibility.Tests;

/// <summary>
/// Manual registration to test registry infrastructure.
/// This simulates what the generated ModuleInitializer should do.
/// </summary>
internal static class ManualRegistration
{
    [ModuleInitializer]
    public static void RegisterTestAccessors()
    {
        // Register our manual test accessor
        AwsAccessorRegistry.Register<AmazonS3Client>(new ManualS3Accessor());
        AwsAccessorRegistry.RegisterInterface<IAmazonS3, AmazonS3Client>();
        
        System.Console.WriteLine("Manual test accessor registered for AmazonS3Client");
    }
} 