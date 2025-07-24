using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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

        // Discover AWS service clients from compilation metadata (referenced assemblies)
        var awsClients = context.CompilationProvider
            .Select((compilation, _) => FindAwsClientsInMetadata(compilation).ToImmutableArray());

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

    private static IEnumerable<AwsClientInfo> FindAwsClientsInMetadata(Compilation compilation)
    {
        // Find the AmazonServiceClient base type
        var baseSym = compilation.GetTypeByMetadataName("Amazon.Runtime.AmazonServiceClient");
        if (baseSym is null)
        {
            // AWS SDK not referenced, no clients to find
            yield break;
        }

        foreach (var reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assembly)
            {
                foreach (var client in GetAwsClientsFromAssembly(assembly.GlobalNamespace, baseSym))
                {
                    yield return client;
                }
            }
        }
    }

    private static IEnumerable<AwsClientInfo> GetAwsClientsFromAssembly(INamespaceSymbol namespaceSymbol, INamedTypeSymbol baseType)
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            if (member is INamespaceSymbol nestedNamespace)
            {
                foreach (var client in GetAwsClientsFromAssembly(nestedNamespace, baseType))
                {
                    yield return client;
                }
            }
            else if (member is INamedTypeSymbol typeSymbol && InheritsFromAmazonServiceClient(typeSymbol, baseType))
            {
                // Try to find the corresponding ClientConfig type
                var configType = FindClientConfigType(typeSymbol);
                if (configType != null)
                {
                    // Find the corresponding service interface
                    var serviceInterface = FindServiceInterface(typeSymbol);

                    yield return new AwsClientInfo(
                        clientType: typeSymbol,
                        configType: configType,
                        serviceInterface: serviceInterface);
                }
            }
        }
    }

    private static bool InheritsFromAmazonServiceClient(INamedTypeSymbol typeSymbol, INamedTypeSymbol baseType)
    {
        var current = typeSymbol.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }

    private static INamedTypeSymbol? FindClientConfigType(INamedTypeSymbol clientSymbol)
    {
        // Convention: AmazonS3Client -> AmazonS3Config
        var clientName = clientSymbol.Name;
        if (!clientName.EndsWith("Client", StringComparison.Ordinal))
            return null;

        var configName = clientName.Substring(0, clientName.Length - 6) + "Config"; // Remove "Client", add "Config"
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

        var interfaceName = "I" + clientName.Substring(0, clientName.Length - 6); // Remove "Client", add "I" prefix
        var containingNamespace = clientSymbol.ContainingNamespace;

        // Look for the interface in the same namespace
        return containingNamespace.GetTypeMembers(interfaceName).FirstOrDefault();
    }

    private static void GenerateAccessors(SourceProductionContext context, ImmutableArray<AwsClientInfo> clients)
    {
        var sourceBuilder = new StringBuilder();

        // Generate file header with single namespace
        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("// Generated by LocalStack.Client.Generators");
        sourceBuilder.AppendLine("#nullable enable");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using System.Diagnostics.CodeAnalysis;");
        sourceBuilder.AppendLine("using System.Runtime.CompilerServices;");
        sourceBuilder.AppendLine("using Amazon;");
        sourceBuilder.AppendLine("using Amazon.Runtime;");
        sourceBuilder.AppendLine("using Amazon.Runtime.Internal;");
        sourceBuilder.AppendLine("using LocalStack.Client.Utils;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("namespace LocalStack.Client.Generated");
        sourceBuilder.AppendLine("{");

        // Generate accessor for each client
        for (int i = 0; i < clients.Length; i++)
        {
            if (i > 0)
            {
                sourceBuilder.AppendLine();
            }
            GenerateAccessorClass(sourceBuilder, clients[i]);
        }

        sourceBuilder.AppendLine();

        // Generate module initializer
        GenerateModuleInitializer(sourceBuilder, clients);

        sourceBuilder.AppendLine("}"); // Close namespace

        // Add the generated source
        context.AddSource("AwsAccessors.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    private static void GenerateAccessorClass(StringBuilder builder, AwsClientInfo client)
    {
        var clientTypeName = client.ClientType.ToDisplayString();
        var configTypeName = client.ConfigType.ToDisplayString();
        var accessorClassName = client.ClientType.Name + "_Accessor";

        builder.AppendLine($"    internal sealed class {accessorClassName} : IAwsAccessor");
        builder.AppendLine("    {");

        // Type properties
        builder.AppendLine($"        public System.Type ClientType => typeof({clientTypeName});");
        builder.AppendLine($"        public System.Type ConfigType => typeof({configTypeName});");
        builder.AppendLine();

        // UnsafeAccessor methods
        builder.AppendLine("        [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = \"serviceMetadata\")]");
        builder.AppendLine($"        private static extern ref IServiceMetadata GetServiceMetadataField({clientTypeName}? instance);");
        builder.AppendLine();

        builder.AppendLine("        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]");
        builder.AppendLine($"        private static extern {configTypeName} CreateConfig();");
        builder.AppendLine();

        builder.AppendLine("        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]");
        builder.AppendLine($"        private static extern {clientTypeName} CreateClient(AWSCredentials credentials, {configTypeName} config);");
        builder.AppendLine();

        // Interface implementations - add DynamicDependency to the methods that need them
        builder.AppendLine($"        [DynamicDependency(\"serviceMetadata\", typeof({clientTypeName}))]");
        builder.AppendLine("        public IServiceMetadata GetServiceMetadata()");
        builder.AppendLine("        {");
        builder.AppendLine("            ref var metadata = ref GetServiceMetadataField(null);");
        builder.AppendLine("            return metadata;");
        builder.AppendLine("        }");
        builder.AppendLine();

        builder.AppendLine($"        [DynamicDependency(\".ctor\", typeof({configTypeName}))]");
        builder.AppendLine("        public ClientConfig CreateClientConfig()");
        builder.AppendLine("            => CreateConfig();");
        builder.AppendLine();

        builder.AppendLine("        public AmazonServiceClient CreateClient(AWSCredentials credentials, ClientConfig clientConfig)");
        builder.AppendLine($"            => CreateClient(credentials, ({configTypeName})clientConfig);");
        builder.AppendLine();

        // Generate SetRegion implementation
        GenerateSetRegionMethod(builder, client);
        builder.AppendLine();

        // Generate TrySetForcePathStyle implementation
        GenerateForcePathStyleSetMethod(builder, client);
        builder.AppendLine();

        // Generate TryGetForcePathStyle implementation
        GenerateForcePathStyleGetMethod(builder, client);

        builder.AppendLine("    }");
    }

    private static void GenerateSetRegionMethod(StringBuilder builder, AwsClientInfo client)
    {
        var configTypeName = client.ConfigType.ToDisplayString();
        
        // Check if the config type has RegionEndpoint property (including base classes)
        var hasRegionEndpointProperty = GetAllProperties(client.ConfigType)
            .Any(p => p.Name == "RegionEndpoint" && p.SetMethod != null);

        builder.AppendLine("        public void SetRegion(ClientConfig clientConfig, RegionEndpoint regionEndpoint)");
        builder.AppendLine("        {");
        
        // Always add null check (matches SessionReflectionLegacy behavior)
        builder.AppendLine("            if (clientConfig == null)");
        builder.AppendLine("                throw new ArgumentNullException(nameof(clientConfig));");
        builder.AppendLine();
        
        if (hasRegionEndpointProperty)
        {
            // Use public property approach - matches legacy: amazonServiceClient.Config.RegionEndpoint = ...
            builder.AppendLine($"            (({configTypeName})clientConfig).RegionEndpoint = regionEndpoint;");
        }
        else
        {
            // Silent no-op if property doesn't exist (matches legacy behavior with optional chaining)
            builder.AppendLine($"            // RegionEndpoint property not available on {configTypeName}");
        }
        
        builder.AppendLine("        }");
    }

    private static void GenerateForcePathStyleSetMethod(StringBuilder builder, AwsClientInfo client)
    {
        var configTypeName = client.ConfigType.ToDisplayString();
        
        // Pure property discovery - matches SessionReflectionLegacy.SetForcePathStyle exactly
        var hasForcePathStyleProperty = GetAllProperties(client.ConfigType)
            .Any(p => p.Name == "ForcePathStyle" && 
                     p.SetMethod?.DeclaredAccessibility == Accessibility.Public &&
                     (p.Type.SpecialType == SpecialType.System_Boolean || 
                      p.Type.Name == "Boolean"));

        builder.AppendLine("        public bool TrySetForcePathStyle(ClientConfig clientConfig, bool value)");
        builder.AppendLine("        {");
        
        // Always add null check (matches SessionReflectionLegacy behavior)
        builder.AppendLine("            if (clientConfig == null)");
        builder.AppendLine("                throw new ArgumentNullException(nameof(clientConfig));");
        builder.AppendLine();
        
        if (hasForcePathStyleProperty)
        {
            // Property exists - set it and return true (matches legacy)
            builder.AppendLine($"            (({configTypeName})clientConfig).ForcePathStyle = value;");
            builder.AppendLine("            return true;");
        }
        else
        {
            // Property doesn't exist - return false (matches legacy: forcePathStyleProperty == null)
            builder.AppendLine("            return false;");
        }
        
        builder.AppendLine("        }");
    }

    private static void GenerateForcePathStyleGetMethod(StringBuilder builder, AwsClientInfo client)
    {
        var configTypeName = client.ConfigType.ToDisplayString();
        
        // Same property discovery logic as the set method
        var hasForcePathStyleProperty = GetAllProperties(client.ConfigType)
            .Any(p => p.Name == "ForcePathStyle" && 
                     p.GetMethod?.DeclaredAccessibility == Accessibility.Public &&
                     (p.Type.SpecialType == SpecialType.System_Boolean || 
                      p.Type.Name == "Boolean"));

        builder.AppendLine("        public bool TryGetForcePathStyle(ClientConfig clientConfig, out bool? value)");
        builder.AppendLine("        {");
        
        // Always add null check
        builder.AppendLine("            if (clientConfig == null)");
        builder.AppendLine("                throw new ArgumentNullException(nameof(clientConfig));");
        builder.AppendLine();
        
        if (hasForcePathStyleProperty)
        {
            // Property exists - get its value and return true
            builder.AppendLine($"            value = (({configTypeName})clientConfig).ForcePathStyle;");
            builder.AppendLine("            return true;");
        }
        else
        {
            // Property doesn't exist - set value to null and return false
            builder.AppendLine("            value = null;");
            builder.AppendLine("            return false;");
        }
        
        builder.AppendLine("        }");
    }

    private static IEnumerable<IPropertySymbol> GetAllProperties(INamedTypeSymbol typeSymbol)
    {
        var type = typeSymbol;
        while (type != null && type.SpecialType != SpecialType.System_Object)
        {
            foreach (var member in type.GetMembers())
            {
                if (member is IPropertySymbol property)
                {
                    yield return property;
                }
            }
            type = type.BaseType;
        }
    }

    private static void GenerateModuleInitializer(StringBuilder builder, ImmutableArray<AwsClientInfo> clients)
    {
        builder.AppendLine("    internal static class GeneratedModuleInitializer");
        builder.AppendLine("    {");
        builder.AppendLine("        [ModuleInitializer]");
        builder.AppendLine("        public static void RegisterGeneratedAccessors()");
        builder.AppendLine("        {");

        foreach (var client in clients)
        {
            var clientTypeName = client.ClientType.ToDisplayString();
            var accessorClassName = client.ClientType.Name + "_Accessor";

            builder.AppendLine($"            AwsAccessorRegistry.Register<{clientTypeName}>(new {accessorClassName}());");

            if (client.ServiceInterface != null)
            {
                var interfaceTypeName = client.ServiceInterface.ToDisplayString();
                builder.AppendLine($"            AwsAccessorRegistry.RegisterInterface<{interfaceTypeName}, {clientTypeName}>();");
            }
        }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }
}

/// <summary>
/// Information about a discovered AWS client and its related types.
/// </summary>
internal sealed class AwsClientInfo
{
    public AwsClientInfo(INamedTypeSymbol clientType, INamedTypeSymbol configType, INamedTypeSymbol? serviceInterface)
    {
        ClientType = clientType;
        ConfigType = configType;
        ServiceInterface = serviceInterface;
    }

    /// <summary>
    /// The AWS service client type (e.g., AmazonS3Client)
    /// </summary>
    public INamedTypeSymbol ClientType { get; }

    /// <summary>
    /// The corresponding client configuration type (e.g., AmazonS3Config)
    /// </summary>
    public INamedTypeSymbol ConfigType { get; }

    /// <summary>
    /// The service interface, if found (e.g., IAmazonS3)
    /// </summary>
    public INamedTypeSymbol? ServiceInterface { get; }
}