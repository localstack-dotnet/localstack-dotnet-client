using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace LocalStack.Client.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class AwsRegistryGenerator : IIncrementalGenerator
{
    private const string Runtime = "global::Amazon.Runtime.";
    private const string Metadata = "global::Amazon.Runtime.Internal.IServiceMetadata";
    private static readonly ConditionalWeakTable<MetadataReference, Dictionary<string, ReferenceModel>> References = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<string> declarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
            static (syntax, token) => syntax.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)syntax.Node, token) is INamedTypeSymbol type
                ? MetadataName(type) : "");
        // String equality prevents unchanged plans from invalidating emission.
        // Symbols remain inside the classification transform.
        var referenceContext = context.CompilationProvider.Select(static (compilation, token) => PrepareReferences(compilation, token))
            .WithTrackingName("ReferenceContext");
        var references = context.MetadataReferencesProvider.Combine(referenceContext).SelectMany(static (input, _) =>
        {
            if (References.TryGetValue(input.Left, out Dictionary<string, ReferenceModel>? cache))
            {
                lock (cache)
                {
                    if (cache.TryGetValue(input.Right.CoreIdentity, out ReferenceModel? model) &&
                        input.Right.Assemblies.IndexOf("\n" + model.AssemblyIdentity + "\n", StringComparison.Ordinal) >= 0)
                    {
                        return ImmutableArray.Create(model);
                    }
                }
            }

            return ImmutableArray<ReferenceModel>.Empty;
        }).WithTrackingName("ReferenceDiscovery");
        IncrementalValueProvider<string> source = declarations.Collect().Combine(references.Collect()).Combine(context.CompilationProvider)
            .Select(static (input, token) => Generate(input.Right, input.Left.Left, input.Left.Right, token))
            .WithComparer(StringComparer.Ordinal).WithTrackingName("RegistryPlan");
        context.RegisterSourceOutput(source, static (output, text) =>
        {
            if (text.Length != 0)
            {
                output.AddSource("LocalStackGeneratedRegistry.g.cs", SourceText.From(text, Encoding.UTF8));
            }
        });
    }

    private static string Generate(Compilation compilation, IEnumerable<string> declarations, IEnumerable<ReferenceModel> references, CancellationToken token)
    {
        // Private SDK fields are omitted by the consumer's default metadata import policy.
        // Rebind symbols with full metadata; the emitted code still uses exact accessors.
        compilation = compilation.WithOptions(compilation.Options.WithMetadataImportOptions(MetadataImportOptions.All));
        INamedTypeSymbol? clientBase = compilation.GetTypeByMetadataName("Amazon.Runtime.AmazonServiceClient");
        INamedTypeSymbol? configBase = compilation.GetTypeByMetadataName("Amazon.Runtime.ClientConfig");
        INamedTypeSymbol? credentials = compilation.GetTypeByMetadataName("Amazon.Runtime.AWSCredentials");
        if (clientBase == null || configBase == null || credentials == null ||
            compilation.GetTypeByMetadataName("LocalStack.Client.Session") == null ||
            compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.UnsafeAccessorTypeAttribute") == null ||
            compilation.GetSpecialType(SpecialType.System_Object).ContainingAssembly.Identity.Version.Major < 10)
        {
            return "";
        }

        var clients = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var configs = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        void Discover(INamedTypeSymbol? type, bool own)
        {
            token.ThrowIfCancellationRequested();
            if (type == null || (!DerivesFrom(type, clientBase) && !(own && DerivesFrom(type, configBase))) || !CanName(compilation, type))
            {
                return;
            }

            if (!type.IsAbstract && DerivesFrom(type, clientBase))
            {
                clients.Add(type);
            }
            else if (own && !Same(type, configBase) && DerivesFrom(type, configBase) && CanUseConfig(compilation, type))
            {
                configs.Add(type);
            }
        }

        foreach (string name in declarations.Distinct(StringComparer.Ordinal))
        {
            Discover(compilation.Assembly.GetTypeByMetadataName(name), own: true);
        }

        Dictionary<string, IAssemblySymbol> assemblies = compilation.References.Select(compilation.GetAssemblyOrModuleSymbol).OfType<IAssemblySymbol>()
            .GroupBy(assembly => assembly.Identity.GetDisplayName(), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);
        foreach (ReferenceModel reference in references)
        {
            if (assemblies.TryGetValue(reference.AssemblyIdentity, out IAssemblySymbol? assembly))
            {
                foreach (string name in reference.Identities)
                {
                    Discover(assembly.GetTypeByMetadataName(name), own: false);
                }
            }
        }
        return Emit(compilation, clients, configs, credentials, configBase);
    }

    private static (string CoreIdentity, string Assemblies) PrepareReferences(Compilation compilation, CancellationToken token)
    {
        INamedTypeSymbol? clientBase = compilation.GetTypeByMetadataName("Amazon.Runtime.AmazonServiceClient");
        INamedTypeSymbol? configBase = compilation.GetTypeByMetadataName("Amazon.Runtime.ClientConfig");
        if (clientBase == null || configBase == null)
        {
            return ("", "");
        }

        IAssemblySymbol[] assemblies = compilation.References.Select(compilation.GetAssemblyOrModuleSymbol).OfType<IAssemblySymbol>().ToArray();
        var eligible = new HashSet<string>(StringComparer.Ordinal) { clientBase.ContainingAssembly.Name };
        bool changed;
        do
        {
            changed = false;
            foreach (IAssemblySymbol assembly in assemblies.Where(assembly =>
                assembly.Modules.Any(module => module.ReferencedAssemblies.Any(dependency => eligible.Contains(dependency.Name)))))
            {
                changed |= eligible.Add(assembly.Name);
            }
        }
        while (changed);

        string coreIdentity = clientBase.ContainingAssembly.Identity.GetDisplayName();
        var identities = new StringBuilder("\n");
        foreach (MetadataReference reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assembly || !eligible.Contains(assembly.Name))
            {
                continue;
            }

            Dictionary<string, ReferenceModel> cache = References.GetValue(reference, _ => new Dictionary<string, ReferenceModel>(StringComparer.Ordinal));
            lock (cache)
            {
                if (!cache.ContainsKey(coreIdentity))
                {
                    cache.Add(coreIdentity, DiscoverReference(assembly, clientBase, configBase, token));
                }
            }

            identities.Append(assembly.Identity.GetDisplayName()).Append('\n');
        }

        return (coreIdentity, identities.ToString());
    }

    private static string Emit(Compilation compilation, HashSet<INamedTypeSymbol> clients, HashSet<INamedTypeSymbol> configs,
        INamedTypeSymbol credentials, INamedTypeSymbol configBase)
    {
        var operations = new List<string>();
        var registrations = new List<string>();
        foreach (INamedTypeSymbol client in clients.OrderBy(Identity, StringComparer.Ordinal))
        {
            string suffix = operations.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string? operation = ClientOperation(compilation, client, credentials, configBase, configs, suffix);
            if (operation != null)
            {
                operations.Add(operation);
                registrations.Add("RegisterClient(null!, typeof(" + Name(client) + "), Metadata" + suffix + ", Config" + suffix + ", Create" + suffix + ");");
            }
        }

        foreach (INamedTypeSymbol config in configs.OrderBy(Identity, StringComparer.Ordinal))
        {
            string? style = StyleOperation(compilation, config, configBase);
            bool plainRegion = !Hierarchy(config, configBase).Any(type => type.GetMembers("RegionEndpoint").Length != 0);
            registrations.Add("RegisterConfig(null!, typeof(" + Name(config) + "), " + (style ?? "null") + ", " + (plainRegion ? "true" : "false") + ");");
        }

        var source = new StringBuilder(Header);
        source.AppendLine("[global::System.Runtime.CompilerServices.ModuleInitializerAttribute] internal static void Initialize() {");
        for (int index = 0; index < registrations.Count; index++)
        {
            source.Append("try { Register").Append(index).AppendLine("(); } catch (global::System.MissingMethodException) { } catch (global::System.IO.FileNotFoundException) { } catch (global::System.IO.FileLoadException) { } catch (global::System.TypeLoadException) { } catch (global::System.BadImageFormatException) { }");
        }

        source.AppendLine("}");
        for (int index = 0; index < registrations.Count; index++)
        {
            source.AppendLine("[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]");
            source.Append("private static void Register").Append(index).Append("() { ").Append(registrations[index]).AppendLine(" }");
        }
        foreach (string operation in operations)
        {
            source.AppendLine(operation);
        }

        return source.AppendLine("} }").ToString();
    }

    private static string? ClientOperation(Compilation compilation, INamedTypeSymbol client, INamedTypeSymbol credentials,
        INamedTypeSymbol configBase, HashSet<INamedTypeSymbol> configs, string suffix)
    {
        IMethodSymbol[] constructors = client.InstanceConstructors.Where(ctor => ctor.DeclaredAccessibility == Accessibility.Public).ToArray();
        if (constructors.Any(ctor => ctor.Parameters.Any(parameter => parameter.IsParams) ||
            (ctor.Parameters.Length > 2 && ctor.Parameters.Skip(2).All(parameter => parameter.IsOptional)) ||
            (ctor.Parameters.Length == 2 &&
                ((!Same(ctor.Parameters[0].Type, credentials) &&
                    (compilation.ClassifyCommonConversion(credentials, ctor.Parameters[0].Type).IsImplicit ||
                     ctor.Parameters[0].Type is INamedTypeSymbol first && DerivesFrom(first, credentials))) ||
                 (ctor.Parameters[1].Type is INamedTypeSymbol other && !DerivesFrom(other, configBase) &&
                    (other.TypeKind == TypeKind.Interface || compilation.ClassifyCommonConversion(configBase, other).IsImplicit))))))
        {
            return null;
        }

        IMethodSymbol[] candidates = constructors.Where(ctor => ctor.Parameters.Length == 2 && Same(ctor.Parameters[0].Type, credentials) &&
            ctor.Parameters[1].Type is INamedTypeSymbol config && DerivesFrom(config, configBase)).ToArray();
        if (candidates.Any(ctor => ctor.Parameters.Any(parameter => parameter.RefKind != RefKind.None) || !CanConstruct(client, ctor) ||
                !CanUseConfig(compilation, (INamedTypeSymbol)ctor.Parameters[1].Type)) ||
            candidates.Any(first => candidates.Any(second => !DerivesFrom((INamedTypeSymbol)first.Parameters[1].Type, (INamedTypeSymbol)second.Parameters[1].Type) &&
                !DerivesFrom((INamedTypeSymbol)second.Parameters[1].Type, (INamedTypeSymbol)first.Parameters[1].Type))))
        {
            return null;
        }

        IFieldSymbol? field = client.GetMembers("serviceMetadata").OfType<IFieldSymbol>()
            .FirstOrDefault(member => member.IsStatic && member.DeclaredAccessibility != Accessibility.Public);
        if (field != null && (!compilation.IsSymbolAccessibleWithin(field.Type, compilation.Assembly) || HasDiagnosticAttribute(field.Type)))
        {
            return null;
        }

        string clientName = Name(client);
        var text = new StringBuilder();
        if (field != null)
        {
            text.AppendLine("[global::System.Runtime.CompilerServices.UnsafeAccessorAttribute(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticField, Name = \"serviceMetadata\")]");
            text.Append("private static extern ref ").Append(Name(field.Type)).Append(" Field").Append(suffix).Append('(').Append(clientName).AppendLine("? owner);");
        }

        text.Append("private static ").Append(Metadata).Append(" Metadata").Append(suffix).Append("() => ");
        text.AppendLine(field == null
            ? "throw new global::System.InvalidOperationException($\"Invalid service type {typeof(" + clientName + ")}\");"
            : "(" + Metadata + ")(object)Field" + suffix + "(null)!;");
        text.Append(ConfigOperation(candidates, configBase, configs, suffix));
        text.Append("private static ").Append(Runtime).Append("AmazonServiceClient? Create").Append(suffix).Append('(').Append(Runtime)
            .Append("AWSCredentials credentials, ").Append(Runtime).AppendLine("ClientConfig config) {");
        int arm = 0;
        foreach (IMethodSymbol constructor in candidates.OrderByDescending(ctor => Hierarchy((INamedTypeSymbol)ctor.Parameters[1].Type, configBase).Count()))
        {
            string variable = "config" + arm.ToString(System.Globalization.CultureInfo.InvariantCulture);
            text.Append("if (config is ").Append(Name(constructor.Parameters[1].Type)).Append(' ').Append(variable).Append(") return new ")
                .Append(clientName).Append("(credentials, ").Append(variable).AppendLine(");");
            arm++;
        }

        return text.AppendLine("return null; }").ToString();
    }

    private static string ConfigOperation(IMethodSymbol[] candidates, INamedTypeSymbol configBase, HashSet<INamedTypeSymbol> configs, string suffix)
    {
        IMethodSymbol[] selected = candidates.Where(ctor => string.Equals(ctor.Parameters[0].Name, "credentials", StringComparison.Ordinal) &&
            string.Equals(ctor.Parameters[1].Name, "clientConfig", StringComparison.Ordinal) && !Same(ctor.Parameters[1].Type, configBase)).ToArray();
        var text = new StringBuilder();
        text.Append("private static ").Append(Runtime).Append("ClientConfig Config").Append(suffix).Append("() { ");
        if (selected.Length != 1)
        {
            text.Append("throw new global::System.InvalidOperationException(\"Sequence contains ")
                .Append(selected.Length == 0 ? "no matching element" : "more than one matching element").AppendLine("\"); }");
        }
        else
        {
            var config = (INamedTypeSymbol)selected[0].Parameters[1].Type;
            configs.Add(config);
            if (config.IsAbstract || !config.InstanceConstructors.Any(ctor => ctor.DeclaredAccessibility == Accessibility.Public && ctor.Parameters.Length == 0))
            {
                text.AppendLine("throw new global::System.MissingMethodException(); }");
            }
            else
            {
                text.Append("try { return new ").Append(Name(config)).AppendLine("(); } catch (global::System.Exception e) { throw new global::System.Reflection.TargetInvocationException(e); } }");
            }
        }

        return text.ToString();
    }

    private static string? StyleOperation(Compilation compilation, INamedTypeSymbol config, INamedTypeSymbol configBase)
    {
        ISymbol[] members = Hierarchy(config, configBase).SelectMany(type => type.GetMembers("ForcePathStyle").Concat(
            type.GetMembers("this[]").Where(member => string.Equals(member.MetadataName, "ForcePathStyle", StringComparison.Ordinal)))).ToArray();
        if (members.Length == 0)
        {
            return "(c, v) => false";
        }

        if (members.Length != 1 || members[0] is not IPropertySymbol { IsStatic: false, IsIndexer: false } property ||
            property.Type.SpecialType != SpecialType.System_Boolean || property.GetMethod?.DeclaredAccessibility != Accessibility.Public ||
            property.SetMethod?.DeclaredAccessibility != Accessibility.Public || property.SetMethod.IsInitOnly || HasDiagnosticAttribute(property) ||
            !compilation.IsSymbolAccessibleWithin(property.ContainingType, compilation.Assembly))
        {
            return null;
        }

        return "(c, v) => { try { ((" + Name(config) + ")c).ForcePathStyle = v; } catch (global::System.Exception e) { throw new global::System.Reflection.TargetInvocationException(e); } return true; }";
    }

    private static ReferenceModel DiscoverReference(IAssemblySymbol assembly, INamedTypeSymbol clientBase, INamedTypeSymbol configBase, CancellationToken token)
    {
        var names = new List<string>();
        void VisitType(INamedTypeSymbol type)
        {
            token.ThrowIfCancellationRequested();
            if (type.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            if (DerivesFrom(type, clientBase) || DerivesFrom(type, configBase))
            {
                names.Add(MetadataName(type));
            }
            foreach (INamedTypeSymbol nested in type.GetTypeMembers())
            {
                VisitType(nested);
            }
        }

        void VisitNamespace(INamespaceSymbol space)
        {
            foreach (INamespaceSymbol child in space.GetNamespaceMembers())
            {
                VisitNamespace(child);
            }

            foreach (INamedTypeSymbol type in space.GetTypeMembers())
            {
                VisitType(type);
            }
        }

        VisitNamespace(assembly.GlobalNamespace);
        return new ReferenceModel(assembly.Identity.GetDisplayName(), names.ToImmutableArray());
    }

    private static bool CanUseConfig(Compilation compilation, INamedTypeSymbol config)
    {
        IMethodSymbol? constructor = config.InstanceConstructors.FirstOrDefault(ctor => ctor.DeclaredAccessibility == Accessibility.Public && ctor.Parameters.Length == 0);
        return CanName(compilation, config) && (constructor == null || CanConstruct(config, constructor));
    }

    private static bool CanConstruct(INamedTypeSymbol type, IMethodSymbol constructor) => !HasDiagnosticAttribute(constructor) &&
        (!Hierarchy(type, null).Any(current =>
            current.GetMembers().Any(member => member is IPropertySymbol { IsRequired: true } or IFieldSymbol { IsRequired: true })) ||
         constructor.GetAttributes().Any(attribute => string.Equals(attribute.AttributeClass?.ToDisplayString(),
             "System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute", StringComparison.Ordinal)));

    private static bool HasDiagnosticAttribute(ISymbol symbol)
    {
        for (ISymbol? current = symbol; current != null; current = current.ContainingType)
        {
            if (current.GetAttributes().Any(attribute => attribute.AttributeClass?.ToDisplayString() is
                "System.ObsoleteAttribute" or "System.Diagnostics.CodeAnalysis.ExperimentalAttribute"))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CanName(Compilation compilation, INamedTypeSymbol type)
    {
        if (HasDiagnosticAttribute(type))
        {
            return false;
        }

        for (INamedTypeSymbol? current = type; current != null; current = current.ContainingType)
        {
            if (current.IsGenericType || current.IsFileLocal || current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal))
            {
                return false;
            }
        }

        MetadataReference? reference = compilation.GetMetadataReference(type.ContainingAssembly);
        return compilation.IsSymbolAccessibleWithin(type, compilation.Assembly) &&
            (Same(type.ContainingAssembly, compilation.Assembly) || reference != null &&
                (reference.Properties.Aliases.IsDefaultOrEmpty || reference.Properties.Aliases.Contains("global", StringComparer.Ordinal))) &&
            HasUniqueName(compilation, type);
    }

    private static bool HasUniqueName(Compilation compilation, INamedTypeSymbol type)
    {
        if (type.ContainingType != null)
        {
            return HasUniqueName(compilation, type.ContainingType);
        }

        var names = new Stack<string>();
        for (INamespaceSymbol space = type.ContainingNamespace; !space.IsGlobalNamespace; space = space.ContainingNamespace)
        {
            names.Push(space.Name);
        }

        INamespaceSymbol current = compilation.GlobalNamespace;
        while (names.Count != 0)
        {
            INamespaceOrTypeSymbol[] members = current.GetMembers(names.Pop()).ToArray();
            if (members.Length != 1 || members[0] is not INamespaceSymbol next)
            {
                return false;
            }

            current = next;
        }

        ImmutableArray<INamedTypeSymbol> types = current.GetTypeMembers(type.Name, type.Arity);
        return types.Length == 1 && Same(types[0], type);
    }

    private static IEnumerable<INamedTypeSymbol> Hierarchy(INamedTypeSymbol type, INamedTypeSymbol? stop)
    {
        for (INamedTypeSymbol? current = type; current != null && !Same(current, stop); current = current.BaseType)
        {
            yield return current;
        }
    }

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType) =>
        Same(type, baseType) || Hierarchy(type, baseType).LastOrDefault()?.BaseType is INamedTypeSymbol last && Same(last, baseType);

    private static bool Same(ISymbol? first, ISymbol? second) => SymbolEqualityComparer.Default.Equals(first, second);

    private static string Name(ITypeSymbol type) => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private static string Identity(INamedTypeSymbol type) => type.ContainingAssembly.Identity.GetDisplayName() + ":" + MetadataName(type);

    private static string MetadataName(INamedTypeSymbol type)
    {
        string prefix = type.ContainingNamespace.IsGlobalNamespace ? "" : type.ContainingNamespace.ToDisplayString() + ".";
        return type.ContainingType != null ? MetadataName(type.ContainingType) + "+" + type.MetadataName : prefix + type.MetadataName;
    }

    private sealed class ReferenceModel(string assemblyIdentity, ImmutableArray<string> identities) : IEquatable<ReferenceModel>
    {
        public string AssemblyIdentity { get; } = assemblyIdentity;
        public ImmutableArray<string> Identities { get; } = identities;
        public bool Equals(ReferenceModel? other) => other != null && string.Equals(AssemblyIdentity, other.AssemblyIdentity, StringComparison.Ordinal) &&
            Identities.SequenceEqual(other.Identities, StringComparer.Ordinal);
        public override bool Equals(object? obj) => obj is ReferenceModel other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(AssemblyIdentity);
    }

    private const string Header = """
        // <auto-generated/>
        #nullable enable
        namespace LocalStack.Client.Generated {
        internal static class LocalStackGeneratedRegistry {
        [global::System.Runtime.CompilerServices.UnsafeAccessorAttribute(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "RegisterGeneratedClient")]
        private static extern void RegisterClient(global::LocalStack.Client.Session unused, global::System.Type type,
            global::System.Func<global::Amazon.Runtime.Internal.IServiceMetadata> metadata, global::System.Func<global::Amazon.Runtime.ClientConfig> config,
            global::System.Func<global::Amazon.Runtime.AWSCredentials, global::Amazon.Runtime.ClientConfig, global::Amazon.Runtime.AmazonServiceClient?> create);
        [global::System.Runtime.CompilerServices.UnsafeAccessorAttribute(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "RegisterGeneratedConfig")]
        private static extern void RegisterConfig(global::LocalStack.Client.Session unused, global::System.Type type,
            global::System.Func<global::Amazon.Runtime.ClientConfig, bool, bool>? style, bool plainRegion);

        """;
}
