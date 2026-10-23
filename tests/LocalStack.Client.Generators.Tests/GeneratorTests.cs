using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace LocalStack.Client.Generators.Tests;

public class GeneratorTests
{
    [Theory]
    [MemberData(nameof(RejectedSourceShapes))]
    public void Generator_Should_Skip_Shapes_The_Compiler_Would_Reject(string shape, string clients, string configs, bool nullStyle)
    {
        ArgumentNullException.ThrowIfNull(clients);
        ArgumentNullException.ThrowIfNull(configs);
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture +
            "namespace Review { public class Good : Amazon.S3.AmazonS3Client { } " + shape + " }");
        Assert.Empty(output.GetDiagnostics().Where(diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning &&
            diagnostic.Location.SourceTree?.FilePath.EndsWith("LocalStackGeneratedRegistry.g.cs", StringComparison.Ordinal) == true));
        Assert.Equal(clients.Split(',', StringSplitOptions.RemoveEmptyEntries), ReviewRegistrations(result, "Client"));
        Assert.Equal(configs.Split(',', StringSplitOptions.RemoveEmptyEntries), ReviewRegistrations(result, "Config"));
        if (nullStyle)
        {
            Assert.Contains("typeof(global::Review.BadConfig), null, true)", result.GeneratedTrees.Single().ToString(), StringComparison.Ordinal);
        }
    }

    public static IEnumerable<object[]> RejectedSourceShapes()
    {
        const string config = "public class BadConfig : AbsentConfig { }";
        const string client = "public class Bad : Amazon.Runtime.AmazonServiceClient { BODY }";
        const string constructor = "public Bad(Amazon.Runtime.AWSCredentials credentials, BadConfig clientConfig) : base(credentials, clientConfig) { }";
        foreach (string modifier in new[] { "ref", "out", "in" })
        {
            string body = string.Equals(modifier, "out", StringComparison.Ordinal) ? "clientConfig = new BadConfig();" : "";
            yield return new object[] { config + client.Replace("BODY", "public Bad(Amazon.Runtime.AWSCredentials credentials, " + modifier +
                " BadConfig clientConfig) : base(credentials, new BadConfig()) { " + body + " }", StringComparison.Ordinal), "Good", "BadConfig", false };
            body = string.Equals(modifier, "out", StringComparison.Ordinal) ? "credentials = new Amazon.Runtime.AnonymousAWSCredentials();" : "";
            yield return new object[] { config + client.Replace("BODY", "public Bad(" + modifier +
                " Amazon.Runtime.AWSCredentials credentials, BadConfig clientConfig) : base(new Amazon.Runtime.AnonymousAWSCredentials(), clientConfig) { " + body + " }", StringComparison.Ordinal), "Good", "BadConfig", false };
        }

        yield return new object[] { "public class BadConfig : AbsentConfig { public bool ForcePathStyle { get; init; } }", "Good", "BadConfig", true };
        yield return new object[] { config + client.Replace("BODY", "public required string Name { get; set; } " + constructor, StringComparison.Ordinal), "Good", "BadConfig", false };
        yield return new object[] { "public class BadConfig : AbsentConfig { public required string Name { get; set; } }" +
            client.Replace("BODY", constructor, StringComparison.Ordinal), "Good", "", false };
        foreach (string attribute in new[] { "System.Obsolete", "System.Obsolete(\"old\", true)", "System.Diagnostics.CodeAnalysis.Experimental(\"REVIEW001\")" })
        {
            string marked = "[" + attribute + "] ";
            yield return new object[] { config + marked + client.Replace("BODY", constructor, StringComparison.Ordinal), "Good", "BadConfig", false };
            yield return new object[] { config + client.Replace("BODY", marked + constructor, StringComparison.Ordinal), "Good", "BadConfig", false };
            yield return new object[] { marked + config + client.Replace("BODY", constructor, StringComparison.Ordinal), "Good", "", false };
            yield return new object[] { "public class BadConfig : AbsentConfig { " + marked + "public BadConfig() { } }" +
                client.Replace("BODY", constructor, StringComparison.Ordinal), "Good", "", false };
            yield return new object[] { "public class BadConfig : AbsentConfig { " + marked + "public bool ForcePathStyle { get; set; } }", "Good", "BadConfig", true };
            yield return new object[] { marked + "public class Outer { public class Bad : Amazon.S3.AmazonS3Client { } }", "Good", "", false };
        }
    }

    private static string[] ReviewRegistrations(GeneratorDriverRunResult result, string kind) => result.GeneratedTrees.Single().ToString().Split('\n')
        .Where(line => line.Contains("Register" + kind + "(null!, typeof(global::Review.", StringComparison.Ordinal))
        .Select(line => line.Split("typeof(global::Review.", StringSplitOptions.None)[1].Split(')')[0]).ToArray();

    [Fact]
    public void Generator_Should_Register_Only_Accessible_Concrete_NonGeneric_Clients()
    {
        (GeneratorDriverRunResult result, Compilation output) = Generate("""
            namespace Selection {
                public class Zulu : Amazon.S3.AmazonS3Client { }
                internal class Alpha : Amazon.S3.AmazonS3Client { }
                public abstract class Abstract : Amazon.S3.AmazonS3Client { }
                public class Generic<T> : Amazon.S3.AmazonS3Client { }
                public class Outer<T> { public class Nested : Amazon.S3.AmazonS3Client { } }
                public class Container { private class Hidden : Amazon.S3.AmazonS3Client { } }
            }
            """);
        AssertNoCompilationErrors(output);
        string[] registrations = result.GeneratedTrees.Single().ToString().Split('\n')
            .Where(line => line.Contains("RegisterClient(null!", StringComparison.Ordinal) && line.Contains("typeof(global::Selection.", StringComparison.Ordinal))
            .Select(line => line.Split("typeof(global::Selection.", StringSplitOptions.None)[1].Split(')')[0]).ToArray();
        Assert.Collection(registrations, name => Assert.Equal("Alpha", name), name => Assert.Equal("Zulu", name));
    }

    [Theory]
    [InlineData("public Exotic(Amazon.Runtime.BasicAWSCredentials credentials, StyleConfig clientConfig) : base(credentials, clientConfig) { }")]
    [InlineData("public Exotic(object credentials, StyleConfig clientConfig) : base((Amazon.Runtime.AWSCredentials)credentials, clientConfig) { }")]
    [InlineData("public Exotic(Amazon.Runtime.AWSCredentials credentials, Amazon.Runtime.IClientConfig clientConfig) : base(credentials, (Amazon.Runtime.ClientConfig)clientConfig) { }")]
    [InlineData("public Exotic(Amazon.Runtime.AWSCredentials credentials, StyleConfig clientConfig, int extra = 0) : base(credentials, clientConfig) { }")]
    [InlineData("public Exotic(params object[] arguments) : base((Amazon.Runtime.AWSCredentials)arguments[0], (Amazon.Runtime.ClientConfig)arguments[1]) { }")]
    [InlineData("public Exotic(Amazon.Runtime.AWSCredentials credentials, Amazon.S3.AmazonS3Config clientConfig) : base(credentials, clientConfig) { }")]
    public void Generator_Should_Leave_Nonstandard_Constructor_Shapes_To_The_Fallback(string constructor)
    {
        (_, Compilation output) = Generate(EligibleFixture + """
            public class Exotic : Amazon.Runtime.AmazonServiceClient {
                public Exotic(Amazon.Runtime.AWSCredentials credentials, StyleConfig clientConfig) : base(credentials, clientConfig) { }
            """ + constructor + "}");
        AssertNoCompilationErrors(output);
        Assembly assembly = Load(output);
        System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(assembly.ManifestModule.ModuleHandle);
        Type client = assembly.GetType("Exotic", true)!;
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", true)!;
        object?[] args = { client, null };
        Assert.False((bool)registry.GetMethod("TryGetClient")!.Invoke(null, args)!);
        // A positive companion defeats a generator that skips everything.
        Assert.Equal("SQS", Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(Entry(assembly.GetType("MetadataClient", true)!, "TryGetClient"), "Metadata")().ServiceId);
    }

    [Fact]
    public void Generator_Should_Discover_Referenced_Sdk_Clients_Without_Registration_Code()
    {
        (GeneratorDriverRunResult result, Compilation output) = Generate("internal class Consumer { }");
        Assert.Empty(result.Diagnostics);
        Assert.Single(result.GeneratedTrees);
        AssertNoCompilationErrors(output);
        // Emit and inspect the generated assembly, not a marker that can pass without SDK roots.
        using var stream = new MemoryStream();
        Assert.True(output.Emit(stream).Success);
        Assembly assembly = Assembly.Load(stream.ToArray());
        Type registry = Assert.Single(assembly.GetTypes(), type => string.Equals(type.Name, "LocalStackGeneratedRegistry", StringComparison.Ordinal));
        Assert.Contains(registry.GetMethods(BindingFlags.NonPublic | BindingFlags.Static), method => method.Name.StartsWith("Metadata", StringComparison.Ordinal));
        var config = Operation<Func<ClientConfig>>(Entry(typeof(AmazonS3Client), "TryGetClient"), "CreateConfig")();
        Assert.IsType<AmazonS3Config>(config);
        Assert.Contains("global::Amazon.S3.AmazonS3Client? owner", result.GeneratedTrees.Single().ToString(), StringComparison.Ordinal);
        Assert.Equal("S3", Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(Entry(typeof(AmazonS3Client), "TryGetClient"), "Metadata")().ServiceId);
    }

    [Fact]
    public void Generator_Should_Keep_Type_Identity_When_Consumer_Declares_Global_Usings()
    {
        (_, Compilation output) = Generate("global using Collision; namespace Collision { internal class Type { } }");
        AssertNoCompilationErrors(output);
    }

    [Theory]
    [InlineData("internal class Func { } internal class Action { }")]
    [InlineData("internal delegate T Func<T>(); internal delegate T Func<T1,T>(T1 a); internal delegate T Func<T1,T2,T>(T1 a,T2 b); internal delegate void Action<T1,T2>(T1 a,T2 b);")]
    [InlineData("""
        internal sealed class ModuleInitializerAttribute : System.Attribute { }
        internal sealed class UnsafeAccessorAttribute : System.Attribute {
            public UnsafeAccessorAttribute(UnsafeAccessorKind kind) { }
            public string Name { get; set; }
        }
        internal enum UnsafeAccessorKind { StaticMethod }
        internal sealed class DynamicallyAccessedMembersAttribute : System.Attribute {
            public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes types) { }
        }
        internal enum DynamicallyAccessedMemberTypes { NonPublicFields, PublicConstructors, PublicProperties }
        """)]
    public void Generator_Should_Compile_When_Consumer_Declares_Global_Delegate_And_Attribute_Names(string declarations)
    {
        (_, Compilation output) = Generate("global using Collision; namespace Collision { " + declarations + " }");
        AssertNoCompilationErrors(output);
        using var stream = new MemoryStream();
        Assert.True(output.Emit(stream).Success);
    }

    [Fact]
    public void Generator_Should_Leave_Private_Closed_Generic_Clients_To_The_Fallback()
    {
        (_, Compilation output) = Generate(PrivateGenericFixture);
        AssertNoCompilationErrors(output);
        Assembly assembly = Load(output);
        Type fixture = assembly.GetType("Fixture", throwOnError: true)!;
        Type clientType = (Type)fixture.GetProperty("ClientType")!.GetValue(null)!;
        // Excluded shapes retain their behavior through the JIT fallback.
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", throwOnError: true)!;
        object?[] lookup = { clientType, null };
        Assert.False((bool)registry.GetMethod("TryGetClient")!.Invoke(null, lookup)!);
        Assert.Equal(0, fixture.GetField("MetadataCalls")!.GetValue(null));
        Assert.Equal(0, fixture.GetField("ConfigCalls")!.GetValue(null));
        var reflection = new Utils.SessionReflection();
        Assert.Equal("DynamoDB", reflection.ExtractServiceMetadata(clientType).ServiceId);
        Assert.Equal(1, fixture.GetField("MetadataCalls")!.GetValue(null));
        ClientConfig config = reflection.CreateClientConfig(clientType);
        Assert.Equal((Type)fixture.GetProperty("ConfigType")!.GetValue(null)!, config.GetType());
        Assert.Equal(1, fixture.GetField("ConfigCalls")!.GetValue(null));
        Assert.True(reflection.SetForcePathStyle(config));
        var session = new Session(new Options.SessionOptions("fixture-id", "fixture-secret", "fixture-token", "eu-west-2"),
            new Config(new Options.ConfigOptions(localStackHost: "fixture-host", useLegacyPorts: true)), reflection);
        using AmazonServiceClient client = session.CreateClientByImplementation(clientType);
        Assert.Equal(clientType, client.GetType());
        Assert.Equal(4569, client.Config.ProxyPort);
        Assert.Equal("fixture-host", client.Config.ProxyHost);
        Assert.Equal("eu-west-2", client.Config.RegionEndpoint.SystemName);
    }

    [Fact]
    public void Generator_Should_Not_Evaluate_Client_Or_Config_Operations_During_Registration()
    {
        (_, Compilation output) = Generate(EligibleFixture);
        Assembly assembly = Load(output);
        Type registry = assembly.GetType("LocalStack.Client.Generated.LocalStackGeneratedRegistry", throwOnError: true)!;
        MethodInfo initialize = registry.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic)!;
        initialize.Invoke(null, null);
        initialize.Invoke(null, null);
        Type fixture = assembly.GetType("Fixture", throwOnError: true)!;
        Assert.Equal(0, fixture.GetField("MetadataCalls")!.GetValue(null));
        Assert.Equal(0, fixture.GetField("ConfigCalls")!.GetValue(null));
        Type client = assembly.GetType("MetadataClient", throwOnError: true)!;
        object entry = Entry(client, "TryGetClient");
        Assert.Equal("SQS", Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(entry, "Metadata")().ServiceId);
        Assert.IsAssignableFrom<ClientConfig>(Operation<Func<ClientConfig>>(entry, "CreateConfig")());
        Assert.Equal(1, fixture.GetField("MetadataCalls")!.GetValue(null));
        Assert.Equal(1, fixture.GetField("ConfigCalls")!.GetValue(null));
    }

    [Fact]
    public void Generator_Should_Read_ServiceMetadata_Through_An_Exact_Static_Field_Accessor()
    {
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture);
        string source = result.GeneratedTrees.Single().ToString();
        Assert.Contains("extern ref global::Metadata Field", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Type.GetType", source, StringComparison.Ordinal);
        Assert.DoesNotContain("GetField", source, StringComparison.Ordinal);
        Assembly assembly = Load(output);
        Type client = assembly.GetType("MetadataClient", throwOnError: true)!;
        object entry = Entry(client, "TryGetClient");
        object expected = client.GetProperty("MetadataValue")!.GetValue(null)!;
        Assert.Same(expected, Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(entry, "Metadata")());
        Assert.Same(expected, new Utils.SessionReflection().ExtractServiceMetadata(client));
    }

    [Fact]
    public void Generator_Should_Emit_Config_Creation_For_Zero_One_And_Many_Constructor_Matches()
    {
        (_, Compilation output) = Generate(EligibleFixture + """
            public class DerivedConfig : StyleConfig { }
            public class Zero : Amazon.S3.AmazonS3Client { }
            public class Two : Amazon.Runtime.AmazonServiceClient {
                public Two(Amazon.Runtime.AWSCredentials credentials, StyleConfig clientConfig) : base(credentials, clientConfig) { }
                public Two(Amazon.Runtime.AWSCredentials credentials, DerivedConfig clientConfig) : base(credentials, clientConfig) { }
            }
            public class ThrowingConfig : StyleConfig {
                public static readonly System.Exception Failure = new System.InvalidOperationException("config");
                public ThrowingConfig() { throw Failure; }
            }
            public class ThrowingConfigClient : Amazon.Runtime.AmazonServiceClient {
                public ThrowingConfigClient(Amazon.Runtime.AWSCredentials credentials, ThrowingConfig clientConfig) : base(credentials, clientConfig) { }
            }
            public abstract class AbstractConfig : StyleConfig { }
            public class AbstractConfigClient : Amazon.Runtime.AmazonServiceClient {
                public AbstractConfigClient(Amazon.Runtime.AWSCredentials credentials, AbstractConfig clientConfig) : base(credentials, clientConfig) { }
            }
            public class NoDefaultConfig : StyleConfig { public NoDefaultConfig(int value) { } }
            public class NoDefaultClient : Amazon.Runtime.AmazonServiceClient {
                public NoDefaultClient(Amazon.Runtime.AWSCredentials credentials, NoDefaultConfig clientConfig) : base(credentials, clientConfig) { }
            }
            """);
        Assembly assembly = Load(output);
        Func<ClientConfig> Config(string name) => Operation<Func<ClientConfig>>(Entry(assembly.GetType(name, true)!, "TryGetClient"), "CreateConfig");
        Assert.Equal("Sequence contains no matching element", Assert.Throws<InvalidOperationException>(Config("Zero")).Message);
        Assert.Equal("Sequence contains more than one matching element", Assert.Throws<InvalidOperationException>(Config("Two")).Message);
        Assert.Equal(assembly.GetType("StyleConfig"), Config("MetadataClient")().GetType());
        TargetInvocationException error = Assert.Throws<TargetInvocationException>(Config("ThrowingConfigClient"));
        Assert.Same(assembly.GetType("ThrowingConfig")!.GetField("Failure")!.GetValue(null), error.InnerException);
        Assert.Throws<MissingMethodException>(Config("AbstractConfigClient"));
        Assert.Throws<MissingMethodException>(Config("NoDefaultClient"));
    }

    [Fact]
    public void Generator_Should_Select_The_Constructor_With_The_Most_Derived_Compatible_Config()
    {
        (_, Compilation output) = Generate(EligibleFixture + """
            public class DerivedConfig : StyleConfig { }
            public class DispatchClient : Amazon.Runtime.AmazonServiceClient {
                public string Selected { get; }
                public DispatchClient(Amazon.Runtime.AWSCredentials credentials, StyleConfig clientConfig) : base(credentials, clientConfig) { Selected = "base"; }
                public DispatchClient(Amazon.Runtime.AWSCredentials credentials, DerivedConfig clientConfig) : base(credentials, clientConfig) { Selected = "derived"; }
            }
            """);
        Assembly assembly = Load(output);
        object entry = Entry(assembly.GetType("DispatchClient", true)!, "TryGetClient");
        var create = Operation<Func<AWSCredentials, ClientConfig, AmazonServiceClient?>>(entry, "Create");
        var credentials = new AnonymousAWSCredentials();
        var derived = (ClientConfig)Activator.CreateInstance(assembly.GetType("DerivedConfig", true)!)!;
        using AmazonServiceClient client = create(credentials, derived)!;
        Assert.Equal("derived", client.GetType().GetProperty("Selected")!.GetValue(client));
        Assert.Same(derived, client.Config);
        var baseConfig = (ClientConfig)Activator.CreateInstance(assembly.GetType("StyleConfig", true)!)!;
        using AmazonServiceClient baseClient = create(credentials, baseConfig)!;
        Assert.Equal("base", baseClient.GetType().GetProperty("Selected")!.GetValue(baseClient));
        Assert.Null(create(credentials, new AmazonS3Config()));
    }

    [Fact]
    public void Generator_Should_Emit_ForcePathStyle_Setters_Only_For_Simple_Public_Properties()
    {
        (_, Compilation output) = Generate(EligibleFixture + """
            public class PrivateStyleConfig : AbsentConfig { public bool ForcePathStyle { get; private set; } }
            public class HiddenStyleConfig : StyleConfig { public new bool ForcePathStyle { get; set; } }
            public class WrongStyleConfig : AbsentConfig { public string ForcePathStyle { get; set; } = "initial"; }
            public class ThrowingStyleConfig : AbsentConfig {
                public static readonly System.Exception Failure = new System.InvalidOperationException("style");
                public bool ForcePathStyle { get => false; set => throw Failure; }
            }
            """);
        Assembly assembly = Load(output);
        object ConfigEntry(string name) => Entry(assembly.GetType(name, true)!, "TryGetConfig");
        Assert.Null(Operation<Delegate?>(ConfigEntry("PrivateStyleConfig"), "Style"));
        Assert.Null(Operation<Delegate?>(ConfigEntry("HiddenStyleConfig"), "Style"));
        Assert.Null(Operation<Delegate?>(ConfigEntry("WrongStyleConfig"), "Style"));
        var absent = (ClientConfig)Activator.CreateInstance(assembly.GetType("AbsentConfig")!)!;
        Assert.False(Operation<Func<ClientConfig, bool, bool>>(ConfigEntry("AbsentConfig"), "Style")(absent, true));
        var simple = (ClientConfig)Activator.CreateInstance(assembly.GetType("StyleConfig")!)!;
        Assert.True(Operation<Func<ClientConfig, bool, bool>>(ConfigEntry("StyleConfig"), "Style")(simple, true));
        Assert.True((bool)simple.GetType().GetProperty("ForcePathStyle")!.GetValue(simple)!);
        var privateStyle = (ClientConfig)Activator.CreateInstance(assembly.GetType("PrivateStyleConfig")!)!;
        Assert.True(new Utils.SessionReflection().SetForcePathStyle(privateStyle));
        Assert.True((bool)privateStyle.GetType().GetProperty("ForcePathStyle")!.GetValue(privateStyle)!);
        var throwing = (ClientConfig)Activator.CreateInstance(assembly.GetType("ThrowingStyleConfig")!)!;
        var style = Operation<Func<ClientConfig, bool, bool>>(ConfigEntry("ThrowingStyleConfig"), "Style");
        TargetInvocationException error = Assert.Throws<TargetInvocationException>(() => style(throwing, true));
        Assert.Same(throwing.GetType().GetField("Failure")!.GetValue(null), error.InnerException);
    }

    [Fact]
    public void Generator_Should_Mark_RegionEndpoint_Plain_Only_When_It_Is_Not_Shadowed()
    {
        (_, Compilation output) = Generate(EligibleFixture + """
            public class ShadowConfig : StyleConfig { public new Amazon.RegionEndpoint RegionEndpoint { get; set; } }
            """);
        Assembly assembly = Load(output);
        Type plain = assembly.GetType("StyleConfig", true)!;
        Type shadow = assembly.GetType("ShadowConfig", true)!;
        Assert.True(Operation<bool>(Entry(plain, "TryGetConfig"), "PlainRegion"));
        Assert.False(Operation<bool>(Entry(shadow, "TryGetConfig"), "PlainRegion"));
        var reflection = new Utils.SessionReflection();
        using var plainClient = new AmazonS3Client(new AnonymousAWSCredentials(), new AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.USEast1 });
        reflection.SetClientRegion(plainClient, "eu-west-2");
        Assert.Equal("eu-west-2", plainClient.Config.RegionEndpoint.SystemName);
        var config = (ClientConfig)Activator.CreateInstance(shadow)!;
        var create = Operation<Func<AWSCredentials, ClientConfig, AmazonServiceClient?>>(Entry(assembly.GetType("MetadataClient", true)!, "TryGetClient"), "Create");
        using AmazonServiceClient client = create(new AnonymousAWSCredentials(), config)!;
        reflection.SetClientRegion(client, "us-west-2");
        Assert.Equal(Amazon.RegionEndpoint.USWest2, shadow.GetProperty("RegionEndpoint")!.GetValue(config));
        Assert.Null(config.RegionEndpoint);
    }

    [Fact]
    public void Generator_Should_Emit_Code_That_Compiles_With_CSharp9()
    {
        var options = new CSharpParseOptions(LanguageVersion.CSharp9);
        CSharpCompilation compilation = CreateCompilation(EligibleFixture);
        compilation = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees.Single(), CSharpSyntaxTree.ParseText(EligibleFixture, options));
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { new AwsRegistryGenerator().AsSourceGenerator() }, parseOptions: options);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out ImmutableArray<Diagnostic> diagnostics);
        Assert.Empty(driver.GetRunResult().Diagnostics);
        Assert.Empty(diagnostics);
        AssertNoCompilationErrors(output);
        Assembly assembly = Load(output);
        Assert.Equal(assembly.GetType("StyleConfig"), Operation<Func<ClientConfig>>(Entry(assembly.GetType("MetadataClient", true)!, "TryGetClient"), "CreateConfig")().GetType());
    }

    [Fact]
    public void Generator_Should_Register_Clients_When_Consumer_Declares_Conflicting_Global_Aliases()
    {
        (_, Compilation output) = Generate("""
            global using Func = System.String;
            global using Action = System.String;
            global using UnsafeAccessorAttribute = System.ObsoleteAttribute;
            global using ModuleInitializerAttribute = System.ObsoleteAttribute;
            """ + EligibleFixture);
        AssertNoCompilationErrors(output);
        Assembly assembly = Load(output);
        Assert.Equal("SQS", Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(Entry(assembly.GetType("MetadataClient", true)!, "TryGetClient"), "Metadata")().ServiceId);
    }

    [Fact]
    public void Generator_Should_Reuse_Reference_Discovery_When_References_Are_Unchanged()
    {
        const string externalSource = "namespace Review { public class External : Amazon.S3.AmazonS3Client { } public class Ordinary { } }";
        CSharpCompilation library = CreateCompilation(externalSource);
        PortableExecutableReference reference = ImageReference(library);
        CSharpCompilation compilation = CreateCompilation(EligibleFixture, reference);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { new AwsRegistryGenerator().AsSourceGenerator() },
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));
        driver = driver.RunGenerators(compilation);
        Assert.Equal("External", Assert.Single(ReviewRegistrations(driver.GetRunResult(), "Client")));
        object cachedDiscovery = CachedReferenceDiscovery(reference);
        Assert.Equal("Review.External", Assert.Single((IEnumerable<string>)cachedDiscovery.GetType().GetProperty("Identities")!.GetValue(cachedDiscovery)!));
        string expected = driver.GetRunResult().GeneratedTrees.Single().ToString();
        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText("internal class Unrelated { }"));
        driver = driver.RunGenerators(compilation);
        GeneratorDriverRunResult result = driver.GetRunResult();
        Assert.Equal(expected, result.GeneratedTrees.Single().ToString());
        Assert.Same(cachedDiscovery, CachedReferenceDiscovery(reference));
        Assert.Equal(IncrementalStepRunReason.Unchanged, Assert.Single(result.Results.Single().TrackedSteps["ReferenceContext"]).Outputs.Single().Reason);
        Assert.All(result.Results.Single().TrackedSteps["ReferenceDiscovery"].SelectMany(step => step.Outputs),
            output => Assert.Equal(IncrementalStepRunReason.Cached, output.Reason));
        PortableExecutableReference replacement = ImageReference(library.AddSyntaxTrees(CSharpSyntaxTree.ParseText(
            "namespace Review { public class Added : Amazon.S3.AmazonS3Client { } }")));
        driver = driver.RunGenerators(compilation.ReplaceReference(reference, replacement));
        Assert.Collection(ReviewRegistrations(driver.GetRunResult(), "Client"), name => Assert.Equal("Added", name), name => Assert.Equal("External", name));
        Assert.Contains(driver.GetRunResult().Results.Single().TrackedSteps["ReferenceDiscovery"].SelectMany(step => step.Outputs),
            output => output.Reason == IncrementalStepRunReason.New || output.Reason == IncrementalStepRunReason.Modified);
    }

    [Fact]
    public void Generator_Should_Discover_Clients_In_Assemblies_That_Reference_AwsSdk_Core_Transitively()
    {
        CSharpCompilation library = CreateCompilation("namespace Review { public class MyS3 : Amazon.S3.AmazonS3Client { } }");
        Assembly libraryAssembly = Load(library);
        Assert.Contains(libraryAssembly.GetReferencedAssemblies(), name => string.Equals(name.Name, "AWSSDK.S3", StringComparison.Ordinal));
        Assert.DoesNotContain(libraryAssembly.GetReferencedAssemblies(), name => string.Equals(name.Name, "AWSSDK.Core", StringComparison.Ordinal));
        (GeneratorDriverRunResult result, Compilation output) = Generate("internal class Consumer { }", ImageReference(library));
        Assert.Equal("MyS3", Assert.Single(ReviewRegistrations(result, "Client")));
        Assembly consumer = Load(output);
        System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(consumer.ManifestModule.ModuleHandle);
        Type client = libraryAssembly.GetType("Review.MyS3", true)!;
        InvalidOperationException error = Assert.Throws<InvalidOperationException>(Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(Entry(client, "TryGetClient"), "Metadata"));
        Assert.Equal("Invalid service type Review.MyS3", error.Message);
    }

    [Fact]
    public void Generator_Should_Keep_Other_Registrations_When_A_Referenced_Assembly_Is_Missing_At_Runtime()
    {
        CSharpCompilation library = CreateCompilation("""
            namespace Review {
                public class MissingConfig : Amazon.S3.AmazonS3Config { }
                public class Missing : Amazon.S3.AmazonS3Client {
                    public Missing(Amazon.Runtime.AWSCredentials credentials, MissingConfig clientConfig) : base(credentials, clientConfig) { }
                }
            }
            """);
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture, ImageReference(library));
        Assert.Equal("Missing", Assert.Single(ReviewRegistrations(result, "Client")));
        Assert.Equal("MissingConfig", Assert.Single(ReviewRegistrations(result, "Config")));
        using var stream = new MemoryStream();
        Assert.True(output.Emit(stream).Success);
        stream.Position = 0;
        var context = new AssemblyLoadContext("missing-client", isCollectible: true);
        context.Resolving += (_, name) => throw new FileNotFoundException("Unavailable client library", name.FullName);
        try
        {
            Assembly assembly = context.LoadFromStream(stream);
            System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(assembly.ManifestModule.ModuleHandle);
            Type client = assembly.GetType("MetadataClient", true)!;
            Assert.Equal("SQS", Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(Entry(client, "TryGetClient"), "Metadata")().ServiceId);
            Assert.Equal(assembly.GetType("StyleConfig"), Operation<Func<ClientConfig>>(Entry(client, "TryGetClient"), "CreateConfig")().GetType());
        }
        finally
        {
            context.Unload();
        }
    }

    [Theory]
    [InlineData("other", "clientConfig", "StyleConfig")]
    [InlineData("credentials", "other", "StyleConfig")]
    [InlineData("credentials", "clientConfig", "Amazon.Runtime.ClientConfig")]
    public void Generator_Should_Not_Select_Constructors_With_Wrong_Names_Or_The_ClientConfig_Base(string first, string second, string configType)
    {
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture +
            "namespace Review { public class Wrong : Amazon.Runtime.AmazonServiceClient { public Wrong(Amazon.Runtime.AWSCredentials " +
            first + ", " + configType + " " + second + ") : base(" + first + ", " + second + ") { } } }");
        Assert.Equal("Wrong", Assert.Single(ReviewRegistrations(result, "Client")));
        string registration = result.GeneratedTrees.Single().ToString().Split('\n').Single(line => line.Contains("RegisterClient(null!, typeof(global::Review.Wrong)", StringComparison.Ordinal));
        string configMethod = registration.Split(", Config", StringSplitOptions.None)[1].Split(',')[0];
        Assert.Contains("ClientConfig Config" + configMethod + "() { throw new global::System.InvalidOperationException(\"Sequence contains no matching element\")",
            result.GeneratedTrees.Single().ToString(), StringComparison.Ordinal);
        Assembly assembly = Load(output);
        Assert.Equal("Sequence contains no matching element", Assert.Throws<InvalidOperationException>(
            Operation<Func<ClientConfig>>(Entry(assembly.GetType("Review.Wrong", true)!, "TryGetClient"), "CreateConfig")).Message);
        Assert.Equal(assembly.GetType("StyleConfig"), Operation<Func<ClientConfig>>(Entry(assembly.GetType("MetadataClient", true)!, "TryGetClient"), "CreateConfig")().GetType());
    }

    [Fact]
    public void Generator_Should_Allow_Required_Members_When_The_Used_Constructor_Sets_Them()
    {
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture + """
            namespace Review {
                public class RequiredConfig : AbsentConfig {
                    public required string Name { get; set; }
                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public RequiredConfig() { Name = "config"; }
                }
                public class RequiredClient : Amazon.Runtime.AmazonServiceClient {
                    public required string Name { get; set; }
                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public RequiredClient(Amazon.Runtime.AWSCredentials credentials, RequiredConfig clientConfig) : base(credentials, clientConfig) { Name = "client"; }
                }
            }
            """);
        Assert.Equal("RequiredClient", Assert.Single(ReviewRegistrations(result, "Client")));
        Assert.Equal("RequiredConfig", Assert.Single(ReviewRegistrations(result, "Config")));
        Assembly assembly = Load(output);
        object entry = Entry(assembly.GetType("Review.RequiredClient", true)!, "TryGetClient");
        ClientConfig config = Operation<Func<ClientConfig>>(entry, "CreateConfig")();
        Assert.Equal("config", config.GetType().GetProperty("Name")!.GetValue(config));
        using AmazonServiceClient client = Operation<Func<AWSCredentials, ClientConfig, AmazonServiceClient?>>(entry, "Create")(new AnonymousAWSCredentials(), config)!;
        Assert.Equal("client", client.GetType().GetProperty("Name")!.GetValue(client));
        Assert.Same(config, client.Config);
    }

    [Theory]
    [InlineData("public static bool ForcePathStyle { get; set; }")]
    [InlineData("[System.Runtime.CompilerServices.IndexerName(\"ForcePathStyle\")] public bool this[int index] { get => false; set { } }")]
    public void Generator_Should_Leave_Static_And_Indexed_ForcePathStyle_Members_To_The_Fallback(string property)
    {
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture +
            "namespace Review { public class BadConfig : AbsentConfig { " + property + " } }");
        Assert.Equal("BadConfig", Assert.Single(ReviewRegistrations(result, "Config")));
        Assembly assembly = Load(output);
        Assert.Null(Operation<Delegate?>(Entry(assembly.GetType("Review.BadConfig", true)!, "TryGetConfig"), "Style"));
        var config = (ClientConfig)Activator.CreateInstance(assembly.GetType("StyleConfig", true)!)!;
        Assert.True(Operation<Func<ClientConfig, bool, bool>>(Entry(config.GetType(), "TryGetConfig"), "Style")(config, true));
        Assert.True((bool)config.GetType().GetProperty("ForcePathStyle")!.GetValue(config)!);
    }

    [Fact]
    public void Generator_Should_Skip_Clients_With_Inaccessible_Config_Or_Field_Types()
    {
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture + """
            namespace Review {
                public class Good : Amazon.S3.AmazonS3Client { }
                public class PrivateField : Amazon.S3.AmazonS3Client {
                    private class Metadata { }
                    private static readonly Metadata serviceMetadata = new Metadata();
                }
                public class PrivateConfig : Amazon.Runtime.AmazonServiceClient {
                    private class Config : AbsentConfig { }
                    public PrivateConfig(Amazon.Runtime.AWSCredentials credentials, Config clientConfig) : base(credentials, clientConfig) { }
                }
            }
            """);
        // An incomplete consumer can expose an inaccessible config in a public signature.
        // The generator must still omit it rather than add more compiler errors.
        Assert.Equal("CS0051", Assert.Single(output.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)).Id);
        Assert.Equal("Good", Assert.Single(ReviewRegistrations(result, "Client")));
        Assert.Empty(ReviewRegistrations(result, "Config"));
        Assert.DoesNotContain(output.GetDiagnostics(), diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning &&
            diagnostic.Location.SourceTree?.FilePath.EndsWith("LocalStackGeneratedRegistry.g.cs", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void Generator_Should_Exclude_Referenced_Protected_Private_And_Generic_Clients()
    {
        CSharpCompilation library = CreateCompilation("""
            namespace Review {
                public class Good : Amazon.S3.AmazonS3Client { }
                public class Generic<T> : Amazon.S3.AmazonS3Client { }
                public class Outer {
                    protected class Protected : Amazon.S3.AmazonS3Client { }
                    private class Private : Amazon.S3.AmazonS3Client { }
                    public class Generic<T> : Amazon.S3.AmazonS3Client { }
                }
            }
            """);
        Assembly libraryAssembly = Load(library);
        (GeneratorDriverRunResult result, Compilation output) = Generate("internal class Consumer { }", ImageReference(library));
        Assert.Equal("Good", Assert.Single(ReviewRegistrations(result, "Client")));
        Assembly consumer = Load(output);
        System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(consumer.ManifestModule.ModuleHandle);
        InvalidOperationException error = Assert.Throws<InvalidOperationException>(Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(
            Entry(libraryAssembly.GetType("Review.Good", true)!, "TryGetClient"), "Metadata"));
        Assert.Equal("Invalid service type Review.Good", error.Message);
    }

    private static PortableExecutableReference ImageReference(Compilation compilation)
    {
        using var stream = new MemoryStream();
        Assert.True(compilation.Emit(stream).Success);
        return MetadataReference.CreateFromImage(stream.ToArray());
    }

    [Fact]
    public void Generator_Should_Not_Abort_Initialization_When_The_Registration_Bridge_Is_Missing()
    {
        Compilation core = CreateCompilation("namespace LocalStack.Client { public class Session { } }");
        core = core.RemoveReferences(core.References.Where(reference => string.Equals(reference.Display, typeof(Session).Assembly.Location, StringComparison.Ordinal)));
        Load(core);
        using var stream = new MemoryStream();
        Assert.True(core.Emit(stream).Success);
        CSharpCompilation compilation = CreateCompilation(EligibleFixture);
        compilation = compilation.RemoveReferences(compilation.References.Where(reference => string.Equals(reference.Display, typeof(Session).Assembly.Location, StringComparison.Ordinal)))
            .AddReferences(MetadataReference.CreateFromImage(stream.ToArray()));
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AwsRegistryGenerator().AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out _);
        Assert.Empty(driver.GetRunResult().Diagnostics);
        AssertNoCompilationErrors(output);
        Assembly assembly = Load(output);
        System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(assembly.ManifestModule.ModuleHandle);
        Assert.Equal(0, assembly.GetType("Fixture", true)!.GetField("MetadataCalls")!.GetValue(null));
        Assert.Equal(0, assembly.GetType("Fixture", true)!.GetField("ConfigCalls")!.GetValue(null));
    }

    [Fact]
    public void Generator_Should_Exclude_Referenced_Clients_With_Ambiguous_Names()
    {
        const string librarySource = "namespace Review { public class Collision : Amazon.S3.AmazonS3Client { } }";
        (GeneratorDriverRunResult result, Compilation output) = Generate(EligibleFixture,
            ImageReference(CreateCompilation(librarySource)), ImageReference(CreateCompilation(librarySource)));
        AssertNoCompilationErrors(output);
        Assert.Empty(ReviewRegistrations(result, "Client"));
        Assembly assembly = Load(output);
        Assert.Equal("SQS", Operation<Func<Amazon.Runtime.Internal.IServiceMetadata>>(
            Entry(assembly.GetType("MetadataClient", true)!, "TryGetClient"), "Metadata")().ServiceId);
    }

    private static object CachedReferenceDiscovery(MetadataReference reference)
    {
        object cache = typeof(AwsRegistryGenerator).GetField("References", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        object?[] arguments = { reference, null };
        Assert.True((bool)cache.GetType().GetMethod("TryGetValue")!.Invoke(cache, arguments)!);
        return Assert.Single(((System.Collections.IDictionary)arguments[1]!).Values.Cast<object>());
    }

    private static object Entry(Type type, string lookupName)
    {
        System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(type.Assembly.ManifestModule.ModuleHandle);
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", true)!;
        object?[] args = { type, null };
        Assert.True((bool)registry.GetMethod(lookupName)!.Invoke(null, args)!, type.FullName);
        return args[1]!;
    }

    private static T Operation<T>(object entry, string name)
    {
        PropertyInfo? property = entry.GetType().GetProperty(name);
        Assert.NotNull(property);
        return (T)property.GetValue(entry)!;
    }

    private const string EligibleFixture = """
        public static class Fixture { public static int MetadataCalls; public static int ConfigCalls; }
        public class AbsentConfig : Amazon.Runtime.ClientConfig {
            public AbsentConfig() : base(new Amazon.Runtime.Internal.DefaultConfigurationProvider(Amazon.S3.AmazonS3DefaultConfiguration.GetAllConfigurations())) { ServiceURL = "http://localhost"; }
            public override string RegionEndpointServiceName => "fixture";
            public override Amazon.Runtime.Endpoints.Endpoint DetermineServiceOperationEndpoint(Amazon.Runtime.Endpoints.ServiceOperationEndpointParameters parameters) => new Amazon.Runtime.Endpoints.Endpoint(ServiceURL);
            public override string ServiceVersion => "1";
            public override string UserAgent => "fixture";
        }
        public class StyleConfig : AbsentConfig {
            public StyleConfig() { Fixture.ConfigCalls++; }
            public bool ForcePathStyle { get; set; }
        }
        public class MetadataClient : Amazon.Runtime.AmazonServiceClient {
            private static readonly Metadata serviceMetadata = new Metadata();
            public static Amazon.Runtime.Internal.IServiceMetadata MetadataValue => serviceMetadata;
            public MetadataClient(Amazon.Runtime.AWSCredentials credentials, StyleConfig clientConfig) : base(credentials, clientConfig) { }
        }
        public class Metadata : Amazon.Runtime.Internal.IServiceMetadata {
            public Metadata() { Fixture.MetadataCalls++; }
            public string ServiceId => "SQS";
            public System.Collections.Generic.IDictionary<string, string> OperationNameMapping { get; } = new System.Collections.Generic.Dictionary<string, string>();
        }
        """;

    [Fact]
    public void Generator_Should_Leave_Same_Name_Configs_From_Different_Assemblies_To_The_Fallback()
    {
        const string configSource = "namespace Shared { public class Config : Amazon.S3.AmazonS3Config { } }";
        Compilation first = CreateCompilation(configSource);
        Compilation second = CreateCompilation(configSource);
        Assembly firstAssembly = Load(first);
        Assembly secondAssembly = Load(second);
        PortableExecutableReference Reference(Compilation compilation, string alias)
        {
            using var stream = new MemoryStream();
            Assert.True(compilation.Emit(stream).Success);
            return MetadataReference.CreateFromImage(stream.ToArray(), new MetadataReferenceProperties(aliases: ImmutableArray.Create(alias)));
        }

        (_, Compilation output) = Generate("""
            extern alias First;
            extern alias Second;
            public static class Consumer {
                public static System.Type FirstType => typeof(First::Shared.Config);
                public static System.Type SecondType => typeof(Second::Shared.Config);
            }
            """, Reference(first, "First"), Reference(second, "Second"));
        AssertNoCompilationErrors(output);
        Type consumer = Load(output).GetType("Consumer", throwOnError: true)!;
        Type firstType = (Type)consumer.GetProperty("FirstType")!.GetValue(null)!;
        Type secondType = (Type)consumer.GetProperty("SecondType")!.GetValue(null)!;
        Assert.Equal(firstAssembly, firstType.Assembly);
        Assert.Equal(secondAssembly, secondType.Assembly);
        Assert.NotEqual(firstType, secondType);
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", throwOnError: true)!;
        foreach (Type type in new[] { firstType, secondType })
        {
            object?[] lookup = { type, null };
            Assert.False((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, lookup)!);
            var config = (ClientConfig)Activator.CreateInstance(type)!;
            Assert.True(new Utils.SessionReflection().SetForcePathStyle(config));
            Assert.True(((AmazonS3Config)config).ForcePathStyle);
        }
    }

    [Fact]
    public void Generator_Should_Leave_Inaccessible_Referenced_Configs_To_The_Fallback()
    {
        Compilation library = CreateCompilation("""
            public static class Factory {
                public static Amazon.Runtime.ClientConfig Create() => new PrivateConfig();
                private sealed class PrivateConfig : Amazon.S3.AmazonS3Config { }
            }
            """);
        Load(library);
        using var stream = new MemoryStream();
        Assert.True(library.Emit(stream).Success);
        (_, Compilation output) = Generate("public static class Consumer { public static object Create() => Factory.Create(); }",
            MetadataReference.CreateFromImage(stream.ToArray()));
        AssertNoCompilationErrors(output);
        var config = (ClientConfig)Load(output).GetType("Consumer", throwOnError: true)!.GetMethod("Create")!.Invoke(null, null)!;
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", throwOnError: true)!;
        object?[] lookup = { config.GetType(), null };
        Assert.False((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, lookup)!);
        Assert.True(new Utils.SessionReflection().SetForcePathStyle(config));
        Assert.True(((AmazonS3Config)config).ForcePathStyle);
    }

    [Fact]
    public void Generator_Should_Leave_Closed_Generic_Signature_Types_To_The_Fallback()
    {
        Compilation library = CreateCompilation(ReferencedSignatureFixture);
        Assembly libraryAssembly = Load(library);
        Assert.DoesNotContain(libraryAssembly.GetReferencedAssemblies(), name => name.Name!.StartsWith("LocalStack", StringComparison.Ordinal));
        using var stream = new MemoryStream();
        Assert.True(library.Emit(stream).Success);
        (_, Compilation output) = Generate("public static class Consumer { public static object Create() => Factory.Create(); }",
            MetadataReference.CreateFromImage(stream.ToArray()));
        AssertNoCompilationErrors(output);
        Assembly consumer = Load(output);
        // Trigger the generated module initializer, not the fixture factory or its type initializers.
        System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(consumer.ManifestModule.ModuleHandle);
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", throwOnError: true)!;
        Type calls = libraryAssembly.GetType("Calls", throwOnError: true)!;
        Type clientDefinition = libraryAssembly.GetType("MyClient`1", throwOnError: true)!;
        Type configDefinition = libraryAssembly.GetType("MyConfig`1", throwOnError: true)!;
        Type[] arguments = { typeof(string), typeof(byte), typeof(int), typeof(short), typeof(long) };
        foreach (Type argument in arguments)
        {
            object?[] lookup = { clientDefinition.MakeGenericType(argument), null };
            Assert.False((bool)registry.GetMethod("TryGetClient")!.Invoke(null, lookup)!);
            object?[] configLookup = { configDefinition.MakeGenericType(argument), null };
            Assert.False((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, configLookup)!);
        }

        object?[] signatureConfigLookup = { configDefinition.MakeGenericType(typeof(decimal)), null };
        Assert.False((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, signatureConfigLookup)!);
        Assert.Equal(0, calls.GetField("Metadata")!.GetValue(null));
        Assert.Equal(0, calls.GetField("Config")!.GetValue(null));
        var reflection = new Utils.SessionReflection();
        var session = new Session(new Options.SessionOptions("fixture-id", "fixture-secret", "fixture-token", "eu-west-2"),
            new Config(new Options.ConfigOptions()), reflection);
        foreach (Type argument in arguments)
        {
            Type clientType = clientDefinition.MakeGenericType(argument);
            Assert.Equal("S3", reflection.ExtractServiceMetadata(clientType).ServiceId);
            using AmazonServiceClient client = session.CreateClientByImplementation(clientType);
            Assert.Equal(clientType, client.GetType());
            Assert.Equal(configDefinition.MakeGenericType(argument), client.Config.GetType());
            Assert.Equal("eu-west-2", client.Config.RegionEndpoint.SystemName);
            Assert.True(((AmazonS3Config)client.Config).ForcePathStyle);
        }

        Assert.Equal(arguments.Length, calls.GetField("Metadata")!.GetValue(null));
        Assert.Equal(arguments.Length, calls.GetField("Config")!.GetValue(null));
    }

    [Fact]
    public void CreateClientConfig_Should_Accept_Types_Without_A_Client_Base_Class_Or_Metadata()
    {
        (_, Compilation output) = Generate("""
            public class Ordinary {
                public Ordinary(Amazon.Runtime.AWSCredentials credentials, Amazon.S3.AmazonS3Config clientConfig) { }
            }
            public static class Consumer { public static System.Type ClientType => typeof(Ordinary); }
            """);
        AssertNoCompilationErrors(output);
        Type consumer = Load(output).GetType("Consumer", throwOnError: true)!;
        Type type = (Type)consumer.GetProperty("ClientType")!.GetValue(null)!;
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", throwOnError: true)!;
        object?[] lookup = { type, null };
        Assert.False((bool)registry.GetMethod("TryGetClient")!.Invoke(null, lookup)!);
        Assert.IsType<AmazonS3Config>(new Utils.SessionReflection().CreateClientConfig(type));
        Assert.Throws<InvalidOperationException>(() => new Utils.SessionReflection().ExtractServiceMetadata(type));
    }

    private static Assembly Load(Compilation compilation)
    {
        using var stream = new MemoryStream();
        AssertNoCompilationErrors(compilation);
        Assert.True(compilation.Emit(stream).Success);
        stream.Position = 0;
        return AssemblyLoadContext.Default.LoadFromStream(stream);
    }

    private const string ReferencedSignatureFixture = """
        using System;
        using System.Collections.Generic;
        using Amazon.Runtime;
        using Amazon.Runtime.Internal;
        public static class Calls { public static int Metadata; public static int Config; }
        public class MyConfig<T> : Amazon.S3.AmazonS3Config {
            public MyConfig() { Calls.Config++; }
        }
        public class MyClient<T> : AmazonServiceClient {
            private static readonly IServiceMetadata serviceMetadata = new Metadata();
            public static IServiceMetadata MetadataValue => serviceMetadata;
            public MyClient(AWSCredentials credentials, MyConfig<T> clientConfig) : base(credentials, clientConfig) { }
        }
        public static class Factory {
            public static MyClient<string> Create() => throw new NotImplementedException();
            public static void Accept(MyClient<byte> value) { }
            public static List<MyClient<int>[]> Property => throw new NotImplementedException();
            public static MyClient<short>[] Field;
            public static List<MyConfig<decimal>[]> Configs => throw new NotImplementedException();
        }
        public sealed class FromBase : MyClient<long> {
            public FromBase(AWSCredentials credentials, MyConfig<long> clientConfig) : base(credentials, clientConfig) { }
        }
        internal sealed class Metadata : IServiceMetadata {
            public Metadata() { Calls.Metadata++; }
            public string ServiceId => "S3";
            public IDictionary<string,string> OperationNameMapping { get; } = new Dictionary<string,string>();
        }
        """;

    private const string PrivateGenericFixture = """
        using System;
        using System.Collections.Generic;
        using Amazon.Runtime;
        using Amazon.Runtime.Internal;
        using Amazon.S3;
        public static class Fixture {
            public static int MetadataCalls;
            public static int ConfigCalls;
            public static Type ClientType => typeof(Client<int[]>);
            public static Type ConfigType => typeof(Config<int[]>);
            private sealed class Config<T> : AmazonS3Config {
                public Config() { ConfigCalls++; }
            }
            private sealed class Client<T> : AmazonServiceClient {
                private static readonly IServiceMetadata serviceMetadata = new Metadata();
                public static IServiceMetadata MetadataValue => serviceMetadata;
                public Client(AWSCredentials credentials, Config<T> clientConfig) : base(credentials, clientConfig) { }
            }
            private sealed class Metadata : IServiceMetadata {
                public Metadata() { MetadataCalls++; }
                public string ServiceId => "DynamoDB";
                public IDictionary<string,string> OperationNameMapping { get; } = new Dictionary<string,string>(StringComparer.Ordinal);
            }
        }
        """;

    private static (GeneratorDriverRunResult Result, Compilation Output) Generate(string source, params PortableExecutableReference[] additionalReferences)
    {
        CSharpCompilation compilation = CreateCompilation(source, additionalReferences);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new AwsRegistryGenerator().AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out ImmutableArray<Diagnostic> diagnostics);
        Assert.Empty(diagnostics);
        return (driver.GetRunResult(), output);
    }

    private static CSharpCompilation CreateCompilation(string source, params PortableExecutableReference[] additionalReferences)
    {
        var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator).ToHashSet(StringComparer.OrdinalIgnoreCase);
        paths.Add(typeof(Session).Assembly.Location);
        paths.Add(typeof(AmazonServiceClient).Assembly.Location);
        paths.Add(typeof(AmazonS3Client).Assembly.Location);
        IEnumerable<PortableExecutableReference> references = paths.Select(path => MetadataReference.CreateFromFile(path));
        return CSharpCompilation.Create("GeneratedConsumer" + Guid.NewGuid().ToString("N"),
            new[] { CSharpSyntaxTree.ParseText(source) }, references.Concat(additionalReferences),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static void AssertNoCompilationErrors(Compilation compilation) => Assert.Empty(compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
}
