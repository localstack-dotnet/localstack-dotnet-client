namespace LocalStack.Client.Generators;

/// <summary>
/// Diagnostic descriptors for the LocalStack AWS accessor source generator.
/// </summary>
internal static class DiagnosticDescriptors
{
    private const string Category = "LocalStack.Client.Generators";

    public static readonly DiagnosticDescriptor NoAwsClientsFound = new(
        id: "LSG001",
        title: "No AWS clients found for code generation",
        messageFormat: "No AWS SDK client types were found in the compilation. Ensure AWS SDK packages are referenced to enable LocalStack client generation.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "The LocalStack source generator could not find any AWS service client types to generate accessors for. This is expected if no AWS SDK packages are referenced.");

    public static readonly DiagnosticDescriptor MissingConfigType = new(
        id: "LSG002",
        title: "AWS client configuration type not found",
        messageFormat: "Could not find configuration type for AWS client '{0}'. Expected type '{1}' in the same namespace.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The source generator found an AWS client but could not locate its corresponding configuration type using naming conventions.");

    public static readonly DiagnosticDescriptor AccessorGenerationError = new(
        id: "LSG003",
        title: "Error generating AWS client accessor",
        messageFormat: "Failed to generate accessor for AWS client '{0}': {1}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "An error occurred while generating the UnsafeAccessor implementation for an AWS client.");

    public static readonly DiagnosticDescriptor AwsClientsGenerated = new(
        id: "LSG004",
        title: "AWS client accessors generated successfully",
        messageFormat: "Generated {0} AWS client accessor(s) for LocalStack Native AOT support",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Successfully generated UnsafeAccessor implementations for the discovered AWS clients.");
}