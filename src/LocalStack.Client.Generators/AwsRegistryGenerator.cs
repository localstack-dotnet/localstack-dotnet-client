using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace LocalStack.Client.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class AwsRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Symbols never escape this transform. The emitted value is equatable, so edits which
        // do not change the discovered graph do not invalidate source output.
        IncrementalValueProvider<string> source = context.CompilationProvider
            .Select(static (compilation, cancellationToken) => Generate(compilation, cancellationToken))
            .WithComparer(StringComparer.Ordinal);
        context.RegisterSourceOutput(source, static (output, text) =>
        {
            if (text.Length != 0)
            {
                output.AddSource("LocalStackGeneratedRegistry.g.cs", SourceText.From(text, Encoding.UTF8));
            }
        });
    }

    private static string Generate(Compilation compilation, CancellationToken cancellationToken)
    {
        INamedTypeSymbol? clientBase = compilation.GetTypeByMetadataName("Amazon.Runtime.AmazonServiceClient");
        INamedTypeSymbol? configBase = compilation.GetTypeByMetadataName("Amazon.Runtime.ClientConfig");
        if (clientBase == null || configBase == null || compilation.GetTypeByMetadataName("LocalStack.Client.Session") == null ||
            compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.UnsafeAccessorAttribute") == null)
        {
            return "";
        }

        var clients = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var configs = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        void Discover(INamedTypeSymbol type)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ContainsTypeParameter(type))
            {
                return;
            }

            if (DerivesFrom(type, configBase))
            {
                configs.Add(type);
            }
            else if (DerivesFrom(type, clientBase) || type.GetMembers("serviceMetadata").OfType<IFieldSymbol>().Any() || HasConfigurationConstructor(type, clientBase, configBase))
            {
                clients.Add(type);
            }
        }

        var visitedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        Action<ITypeSymbol> discoverSignature = type => DiscoverSignature(type, visitedTypes, Discover, cancellationToken);
        DiscoverReferences(compilation, clientBase, discoverSignature, cancellationToken);

        // Definitions alone cannot describe closed custom generic clients. Obtain actual
        // constructed symbols from consumer type syntax, including typeof and Type variables.
        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(tree);
            foreach (GenericNameSyntax syntax in tree.GetRoot(cancellationToken).DescendantNodes().OfType<GenericNameSyntax>())
            {
                if (semanticModel.GetTypeInfo(syntax, cancellationToken).Type is INamedTypeSymbol type)
                {
                    discoverSignature(type);
                }
            }
        }

        foreach (IParameterSymbol parameter in clients.SelectMany(client => client.InstanceConstructors).SelectMany(constructor => constructor.Parameters))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (parameter.Type is INamedTypeSymbol type && !ContainsTypeParameter(type) && DerivesFrom(type, configBase))
            {
                configs.Add(type);
            }
        }

        return Emit(compilation, clients, configs);
    }

    private static bool HasConfigurationConstructor(INamedTypeSymbol type, INamedTypeSymbol clientBase, INamedTypeSymbol configBase)
    {
        INamedTypeSymbol? credentials = clientBase.ContainingAssembly.GetTypeByMetadataName("Amazon.Runtime.AWSCredentials");
        return type.InstanceConstructors.Any(constructor => constructor.DeclaredAccessibility == Accessibility.Public && constructor.Parameters.Length == 2 &&
            string.Equals(constructor.Parameters[0].Name, "credentials", StringComparison.Ordinal) &&
            SymbolEqualityComparer.Default.Equals(constructor.Parameters[0].Type, credentials) &&
            string.Equals(constructor.Parameters[1].Name, "clientConfig", StringComparison.Ordinal) &&
            constructor.Parameters[1].Type is INamedTypeSymbol config && !SymbolEqualityComparer.Default.Equals(config, configBase) && DerivesFrom(config, configBase));
    }

    private static void DiscoverSignature(ITypeSymbol type, HashSet<ITypeSymbol> visited, Action<INamedTypeSymbol> discover, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!visited.Add(type))
        {
            return;
        }

        if (type is IArrayTypeSymbol array)
        {
            DiscoverSignature(array.ElementType, visited, discover, cancellationToken);
        }
        else if (type is INamedTypeSymbol named)
        {
            discover(named);
            foreach (ITypeSymbol argument in named.TypeArguments)
            {
                DiscoverSignature(argument, visited, discover, cancellationToken);
            }

            if (named.BaseType != null)
            {
                DiscoverSignature(named.BaseType, visited, discover, cancellationToken);
            }
        }
    }

    private static void DiscoverReferences(Compilation compilation, INamedTypeSymbol clientBase, Action<ITypeSymbol> discover, CancellationToken cancellationToken)
    {
        // Other libraries may contain custom clients/configs. Do not restrict discovery to
        // AWSSDK.* names or to AddAwsService syntax (generic forwarding has no such syntax).
        foreach (IAssemblySymbol assembly in compilation.SourceModule.ReferencedAssemblySymbols.Prepend(compilation.Assembly).Where(assembly =>
                     SymbolEqualityComparer.Default.Equals(assembly, compilation.Assembly) ||
                      ReferencesAssembly(assembly, clientBase.ContainingAssembly, new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default), cancellationToken)))
        {
            VisitNamespace(assembly.GlobalNamespace, discover, cancellationToken);
        }
    }

    private static bool ReferencesAssembly(IAssemblySymbol assembly, IAssemblySymbol target, HashSet<IAssemblySymbol> visited, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (SymbolEqualityComparer.Default.Equals(assembly, target))
        {
            return true;
        }

        return visited.Add(assembly) && assembly.Modules.Any(module => module.ReferencedAssemblySymbols.Any(reference => ReferencesAssembly(reference, target, visited, cancellationToken)));
    }

    private static string Emit(Compilation compilation, HashSet<INamedTypeSymbol> clients, HashSet<INamedTypeSymbol> configs)
    {
        var source = new StringBuilder(GeneratedReflection.Header);
        INamedTypeSymbol[] orderedClients = clients.OrderBy(AssemblyQualifiedName, StringComparer.Ordinal).ToArray();
        INamedTypeSymbol[] orderedConfigs = configs.OrderBy(AssemblyQualifiedName, StringComparer.Ordinal).ToArray();
        source.AppendLine("[global::System.Runtime.CompilerServices.ModuleInitializerAttribute] internal static void Initialize() {");
        for (int index = 0; index < orderedClients.Length; index++)
        {
            string suffix = index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            source.Append("RegisterClient(null!, ").Append(TypeExpression(compilation, orderedClients[index]))
                .Append(", Metadata").Append(suffix).Append(", Config").Append(suffix)
                .Append(", Constructor").Append(suffix).Append(", Create").Append(suffix).AppendLine(");");
        }

        for (int index = 0; index < orderedConfigs.Length; index++)
        {
            string suffix = index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            source.Append("RegisterConfig(null!, ").Append(TypeExpression(compilation, orderedConfigs[index]))
                .Append(", Style").Append(suffix).Append(", Region").Append(suffix).AppendLine(");");
        }

        source.AppendLine("}");
        for (int index = 0; index < orderedClients.Length; index++)
        {
            string suffix = index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string type = TypeExpression(compilation, orderedClients[index]);
            source.Append("private static IServiceMetadata Metadata").Append(suffix).Append("() => Metadata(").Append(type).AppendLine(");");
            source.Append("private static ClientConfig Config").Append(suffix).Append("() => CreateConfig(FindConfigType(").Append(type).AppendLine("));");
            source.Append("private static ConstructorInfo? Constructor").Append(suffix).Append("(Type config) => Constructor(").Append(type).AppendLine(", config);");
            source.Append("private static AmazonServiceClient Create").Append(suffix).Append("(AWSCredentials credentials, ClientConfig config) => Create(")
                .Append(type).AppendLine(", credentials, config);");
        }

        for (int index = 0; index < orderedConfigs.Length; index++)
        {
            string suffix = index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string type = TypeExpression(compilation, orderedConfigs[index]);
            source.Append("private static bool Style").Append(suffix).Append("(ClientConfig config, bool value) => Style(").Append(type).AppendLine(", config, value);");
            source.Append("private static void Region").Append(suffix).Append("(object config, string name) => Region(").Append(type).AppendLine(", config, name);");
        }

        source.AppendLine("private static ClientConfig CreateConfig(Type type) {");
        foreach (INamedTypeSymbol config in orderedConfigs)
        {
            string type = TypeExpression(compilation, config);
            source.Append("if (type == ").Append(type).Append(") return (ClientConfig)Activator.CreateInstance(").Append(type).AppendLine(")!;");
        }

        source.AppendLine("throw new InvalidOperationException($\"Invalid client configuration type {type}\"); }");
        return source.Append(GeneratedReflection.Methods).AppendLine("}").ToString();
    }

    private static void VisitNamespace(INamespaceSymbol space, Action<ITypeSymbol> visit, CancellationToken cancellationToken)
    {
        foreach (INamespaceOrTypeSymbol member in space.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (member is INamespaceSymbol child)
            {
                VisitNamespace(child, visit, cancellationToken);
            }
            else if (member is INamedTypeSymbol type)
            {
                VisitType(type, visit, cancellationToken);
            }
        }
    }

    private static void VisitType(INamedTypeSymbol type, Action<ITypeSymbol> visit, CancellationToken cancellationToken)
    {
        visit(type);
        // Inspect declarations, not referenced IL or all members of arbitrary constructed
        // containers. The type walk unwraps signatures and constructed bases with cycle checks.
        foreach (ISymbol member in type.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            switch (member)
            {
                case IMethodSymbol method:
                    visit(method.ReturnType);
                    foreach (IParameterSymbol parameter in method.Parameters)
                    {
                        visit(parameter.Type);
                    }

                    break;
                case IPropertySymbol property:
                    visit(property.Type);
                    break;
                case IFieldSymbol field:
                    visit(field.Type);
                    break;
            }
        }

        foreach (INamedTypeSymbol nested in type.GetTypeMembers())
        {
            VisitType(nested, visit, cancellationToken);
        }
    }

    private static bool ContainsTypeParameter(ITypeSymbol type) => type switch
    {
        ITypeParameterSymbol => true,
        IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType),
        INamedTypeSymbol named => named.TypeArguments.Any(ContainsTypeParameter) ||
                                 (named.ContainingType != null && ContainsTypeParameter(named.ContainingType)),
        _ => false,
    };

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (INamedTypeSymbol? current = type; current != null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
        }

        return false;
    }

    private static string TypeExpression(Compilation compilation, INamedTypeSymbol type)
    {
        if (compilation.IsSymbolAccessibleWithin(type, compilation.Assembly) &&
            SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(MetadataName(type)), type.OriginalDefinition))
        {
            return "typeof(" + type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + ")";
        }

        // Constant assembly-qualified Type.GetType is understood by the linker and can root
        // inaccessible nested types without exposing a runtime registration API.
        return "Type.GetType(" + SymbolDisplay.FormatLiteral(AssemblyQualifiedName(type), quote: true) + ", throwOnError: true)!";
    }

    private static string MetadataName(INamedTypeSymbol type)
    {
        string prefix = type.ContainingNamespace.IsGlobalNamespace ? "" : type.ContainingNamespace.ToDisplayString() + ".";
        return type.ContainingType != null ? MetadataName(type.ContainingType) + "+" + type.MetadataName : prefix + type.MetadataName;
    }

    private static string AssemblyQualifiedName(ITypeSymbol type) => ReflectionName(type) + ", " + TypeAssembly(type);

    private static string TypeAssembly(ITypeSymbol type) => type is IArrayTypeSymbol array
        ? TypeAssembly(array.ElementType)
        : type.ContainingAssembly?.Identity.GetDisplayName() ?? "System.Runtime";

    private static string ReflectionName(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol array)
        {
            return ReflectionName(array.ElementType) + "[" + new string(',', array.Rank - 1) + "]";
        }

        if (type is not INamedTypeSymbol named)
        {
            return "System.Object"; // dynamic is System.Object in runtime type identity.
        }

        var arguments = new List<ITypeSymbol>();
        CollectArguments(named, arguments);
        string name = MetadataName(named);
        if (arguments.Count != 0)
        {
            name += "[" + string.Join(",", arguments.Select(argument => "[" + AssemblyQualifiedName(argument) + "]")) + "]";
        }

        return name;
    }

    private static void CollectArguments(INamedTypeSymbol type, List<ITypeSymbol> arguments)
    {
        if (type.ContainingType != null)
        {
            CollectArguments(type.ContainingType, arguments);
        }

        arguments.AddRange(type.TypeArguments);
    }
}
