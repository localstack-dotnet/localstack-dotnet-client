# LocalStack.Client.Generators

**Roslyn Incremental Source Generator for Native AOT Support**

This project contains the source generator that enables Native AOT compatibility for LocalStack.Client by eliminating runtime reflection usage.

## What it does

- **Discovers AWS SDK clients** at compile-time in consumer projects
- **Generates strongly-typed UnsafeAccessor implementations** for private member access
- **Emits ModuleInitializer** for automatic runtime registration
- **Adds DynamicDependency attributes** for trimming safety

## Architecture

- **`AwsAccessorGenerator`**: Main incremental source generator
- **`AwsClientDiscoverer`**: Semantic analysis for finding AWS clients
- **`AccessorEmitter`**: Code generation for UnsafeAccessor implementations  
- **`DiagnosticDescriptors`**: Error and warning messages

## Generated Code Pattern

For each discovered AWS client (e.g., `AmazonS3Client`), generates:

```csharp
[DynamicDependency("serviceMetadata", typeof(Amazon.S3.AmazonS3Client))]
internal sealed class AmazonS3Client_Accessor : IAwsAccessor
{
    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "serviceMetadata")]
    private static extern ref IServiceMetadata GetServiceMetadataField(Amazon.S3.AmazonS3Client? instance);
    
    // Additional UnsafeAccessor methods...
}
```

## Integration

The generator is conditionally included in the main `LocalStack.Client` package for .NET 8+ targets only, ensuring zero impact on legacy frameworks. 