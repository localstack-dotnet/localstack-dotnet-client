namespace LocalStack.Client.Generators;

/// <summary>
/// Incremental source generator that discovers AWS SDK clients at compile-time
/// and generates strongly-typed UnsafeAccessor implementations for Native AOT compatibility.
/// </summary>
[Generator]
public sealed class AwsAccessorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Only generate for .NET 8+ projects to avoid affecting legacy builds
        var isNet8OrAbove = context.CompilationProvider
            .Select((compilation, _) => IsTargetFrameworkNet8OrAbove(compilation));

        // Discover AWS service clients in the consumer's compilation
        var awsClients = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsAwsClientClass(s),
                transform: static (ctx, _) => GetAwsClientInfo(ctx))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!)
            .Collect();

        // Combine framework check with discovered clients
        var generationInput = isNet8OrAbove
            .Combine(awsClients);

        // Generate accessor implementations
        context.RegisterSourceOutput(generationInput, (spc, input) =>
        {
            var (isNet8Plus, clients) = input;
            
            // Always emit diagnostics for debugging
            spc.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.AwsClientsGenerated,
                Location.None,
                $"Generator running: .NET8+ = {isNet8Plus}, Found {clients.Length} potential clients"));
            
            // Only proceed if .NET 8+
            if (!isNet8Plus)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.AwsClientsGenerated,
                    Location.None,
                    "Skipping generation - not .NET 8+"));
                return;
            }
                
            if (clients.IsDefaultOrEmpty)
            {
                // Emit diagnostic: No AWS clients found
                spc.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.NoAwsClientsFound,
                    Location.None));
                return;
            }

            spc.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.AwsClientsGenerated,
                Location.None,
                $"Generating accessors for {clients.Length} AWS clients"));

            GenerateAccessors(spc, clients);
        });
    }

    private static bool IsTargetFrameworkNet8OrAbove(Compilation compilation)
    {
        // Check for .NET 8+ via predefined preprocessor symbols
        if (compilation.Options is not CSharpCompilationOptions options)
            return false;
            
        // Try to get preprocessor symbols via reflection since the property might not be available in all versions
        var preprocessorSymbols = GetPreprocessorSymbols(options);
        return preprocessorSymbols.Contains("NET8_0_OR_GREATER", StringComparer.Ordinal);
    }

    private static IEnumerable<string> GetPreprocessorSymbols(CSharpCompilationOptions options)
    {
        // Use reflection to access PreprocessorSymbolNames if available
        var property = options.GetType().GetProperty("PreprocessorSymbolNames");
        if (property?.GetValue(options) is IEnumerable<string> symbols)
            return symbols;
            
        // Fallback: assume .NET 8+ since generator only runs on modern frameworks
        return new[] { "NET8_0_OR_GREATER" };
    }

    private static bool IsAwsClientClass(SyntaxNode syntaxNode)
    {
        // Look for class declarations that might be AWS clients
        if (syntaxNode is not ClassDeclarationSyntax classDecl)
            return false;

        // Quick syntactic check: class name ends with "Client"
        return classDecl.Identifier.ValueText.EndsWith("Client", StringComparison.Ordinal);
    }

    private static AwsClientInfo? GetAwsClientInfo(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        // Get the symbol for this class
        if (semanticModel.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol classSymbol)
            return null;

        // Check if it inherits from AmazonServiceClient
        if (!InheritsFromAmazonServiceClient(classSymbol))
            return null;

        // Try to find the corresponding ClientConfig type
        var configType = FindClientConfigType(classSymbol);
        if (configType == null)
            return null;

        // Find the corresponding service interface
        var serviceInterface = FindServiceInterface(classSymbol);

        return new AwsClientInfo(
            ClientType: classSymbol,
            ConfigType: configType,
            ServiceInterface: serviceInterface);
    }

    private static bool InheritsFromAmazonServiceClient(INamedTypeSymbol classSymbol)
    {
        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            if (string.Equals(baseType.Name, "AmazonServiceClient", StringComparison.Ordinal) && 
                string.Equals(baseType.ContainingNamespace.ToDisplayString(), "Amazon.Runtime", StringComparison.Ordinal))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        return false;
    }

    private static INamedTypeSymbol? FindClientConfigType(INamedTypeSymbol clientSymbol)
    {
        // Convention: AmazonS3Client -> AmazonS3Config
        var clientName = clientSymbol.Name;
        if (!clientName.EndsWith("Client", StringComparison.Ordinal))
            return null;

        var configName = clientName[..^6] + "Config"; // Remove "Client", add "Config"
        var containingNamespace = clientSymbol.ContainingNamespace;

        // Look for the config type in the same namespace
        return containingNamespace.GetTypeMembers(configName).FirstOrDefault();
    }

    private static INamedTypeSymbol? FindServiceInterface(INamedTypeSymbol clientSymbol)
    {
        // Convention: AmazonS3Client -> IAmazonS3
        var clientName = clientSymbol.Name;
        if (!clientName.EndsWith("Client", StringComparison.Ordinal))
            return null;

        var interfaceName = "I" + clientName[..^6]; // Remove "Client", add "I" prefix
        var containingNamespace = clientSymbol.ContainingNamespace;

        // Look for the interface in the same namespace
        return containingNamespace.GetTypeMembers(interfaceName).FirstOrDefault();
    }

    private static void GenerateAccessors(SourceProductionContext context, ImmutableArray<AwsClientInfo> clients)
    {
        var sourceBuilder = new StringBuilder();

        // Generate file header
        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("// Generated by LocalStack.Client.Generators");
        sourceBuilder.AppendLine("#nullable enable");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using System.Diagnostics.CodeAnalysis;");
        sourceBuilder.AppendLine("using System.Runtime.CompilerServices;");
        sourceBuilder.AppendLine("using Amazon.Runtime;");
        sourceBuilder.AppendLine("using LocalStack.Client.Utils;");
        sourceBuilder.AppendLine();

        // Generate accessor for each client
        foreach (var client in clients)
        {
            GenerateAccessorClass(sourceBuilder, client);
            sourceBuilder.AppendLine();
        }

        // Generate module initializer
        GenerateModuleInitializer(sourceBuilder, clients);

        // Add the generated source
        context.AddSource("AwsAccessors.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    private static void GenerateAccessorClass(StringBuilder builder, AwsClientInfo client)
    {
        var clientTypeName = client.ClientType.ToDisplayString();
        var configTypeName = client.ConfigType.ToDisplayString();
        var accessorClassName = client.ClientType.Name + "_Accessor";

        builder.AppendLine("namespace LocalStack.Client.Generated;");
        builder.AppendLine();

        // Add DynamicDependency attributes for trimming safety
        builder.AppendLine($"[DynamicDependency(\"serviceMetadata\", typeof({clientTypeName}))]");
        builder.AppendLine($"[DynamicDependency(\".ctor\", typeof({configTypeName}))]");
        builder.AppendLine($"internal sealed class {accessorClassName} : IAwsAccessor");
        builder.AppendLine("{");

        // Type properties
        builder.AppendLine($"    public Type ClientType => typeof({clientTypeName});");
        builder.AppendLine($"    public Type ConfigType => typeof({configTypeName});");
        builder.AppendLine();

        // UnsafeAccessor methods
        builder.AppendLine("    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = \"serviceMetadata\")]");
        builder.AppendLine($"    private static extern ref IServiceMetadata GetServiceMetadataField({clientTypeName}? instance);");
        builder.AppendLine();

        builder.AppendLine("    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]");
        builder.AppendLine($"    private static extern {configTypeName} CreateConfig();");
        builder.AppendLine();

        builder.AppendLine("    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]");
        builder.AppendLine($"    private static extern {clientTypeName} CreateClient(AWSCredentials credentials, {configTypeName} config);");
        builder.AppendLine();

        // Interface implementations
        builder.AppendLine("    public IServiceMetadata GetServiceMetadata()");
        builder.AppendLine("        => GetServiceMetadataField(null);");
        builder.AppendLine();

        builder.AppendLine("    public ClientConfig CreateClientConfig()");
        builder.AppendLine("        => CreateConfig();");
        builder.AppendLine();

        builder.AppendLine("    public AmazonServiceClient CreateClient(AWSCredentials credentials, ClientConfig clientConfig)");
        builder.AppendLine($"        => CreateClient(credentials, ({configTypeName})clientConfig);");
        builder.AppendLine();

        builder.AppendLine("    public void SetRegion(ClientConfig clientConfig, RegionEndpoint regionEndpoint)");
        builder.AppendLine("    {");
        builder.AppendLine("        // TODO: Generate UnsafeAccessor for region field");
        builder.AppendLine("        throw new NotImplementedException(\"SetRegion will be implemented in next iteration\");");
        builder.AppendLine("    }");
        builder.AppendLine();

        builder.AppendLine("    public bool TrySetForcePathStyle(ClientConfig clientConfig, bool value)");
        builder.AppendLine("    {");
        builder.AppendLine("        // TODO: Generate UnsafeAccessor for ForcePathStyle property");
        builder.AppendLine("        return false; // Will be implemented in next iteration");
        builder.AppendLine("    }");

        builder.AppendLine("}");
    }

    private static void GenerateModuleInitializer(StringBuilder builder, ImmutableArray<AwsClientInfo> clients)
    {
        builder.AppendLine("namespace LocalStack.Client.Generated;");
        builder.AppendLine();
        builder.AppendLine("internal static class GeneratedModuleInitializer");
        builder.AppendLine("{");
        builder.AppendLine("    [ModuleInitializer]");
        builder.AppendLine("    public static void RegisterGeneratedAccessors()");
        builder.AppendLine("    {");

        foreach (var client in clients)
        {
            var clientTypeName = client.ClientType.ToDisplayString();
            var accessorClassName = client.ClientType.Name + "_Accessor";
            
            builder.AppendLine($"        AwsAccessorRegistry.Register<{clientTypeName}>(new {accessorClassName}());");
            
            if (client.ServiceInterface != null)
            {
                var interfaceTypeName = client.ServiceInterface.ToDisplayString();
                builder.AppendLine($"        AwsAccessorRegistry.RegisterInterface<{interfaceTypeName}, {clientTypeName}>();");
            }
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
    }
}

/// <summary>
/// Information about a discovered AWS client and its related types.
/// </summary>
/// <param name="ClientType">The AWS service client type (e.g., AmazonS3Client)</param>
/// <param name="ConfigType">The corresponding client configuration type (e.g., AmazonS3Config)</param>
/// <param name="ServiceInterface">The service interface, if found (e.g., IAmazonS3)</param>
internal sealed record AwsClientInfo(
    INamedTypeSymbol ClientType,
    INamedTypeSymbol ConfigType,
    INamedTypeSymbol? ServiceInterface); 