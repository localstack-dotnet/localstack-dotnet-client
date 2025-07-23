using LocalStack.Client.Utils;

namespace LocalStack.Client.AotCompatibility.Tests;

/// <summary>
/// Simple test to verify that the source generator populated the AWS accessor registry.
/// </summary>
internal static class SimpleRegistryTest
{
    public static void Run()
    {
        Console.WriteLine("=== Simple Registry Test ===");
        Console.WriteLine($"Registry Count: {AwsAccessorRegistry.Count}");

        if (AwsAccessorRegistry.Count > 0)
        {
            Console.WriteLine("Registered client types:");
            foreach (var clientType in AwsAccessorRegistry.RegisteredClientTypes)
            {
                Console.WriteLine($"  - {clientType.Name}");
            }
        }
        else
        {
            Console.WriteLine("No AWS client accessors registered.");
            Console.WriteLine("This suggests the source generator did not run or did not find any AWS clients.");
        }
    }
}