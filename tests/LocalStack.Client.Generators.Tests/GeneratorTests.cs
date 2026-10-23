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
    public void Generator_Should_Register_Private_Closed_Generic_Clients_Lazily_With_Their_Own_Metadata_And_Config()
    {
        (_, Compilation output) = Generate(PrivateGenericFixture);
        AssertNoCompilationErrors(output);
        Assembly assembly = Load(output);
        Type fixture = assembly.GetType("Fixture", throwOnError: true)!;
        Type clientType = (Type)fixture.GetProperty("ClientType")!.GetValue(null)!;
        // The behavior assertions below must not pass via the ordinary JIT fallback.
        Type registry = typeof(Session).Assembly.GetType("LocalStack.Client.Utils.GeneratedClientRegistry", throwOnError: true)!;
        object?[] lookup = { clientType, null };
        Assert.True((bool)registry.GetMethod("TryGetClient")!.Invoke(null, lookup)!);
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
        (_, Compilation output) = Generate(PrivateGenericFixture);
        Assembly assembly = Load(output);
        Type registry = assembly.GetType("LocalStack.Client.Generated.LocalStackGeneratedRegistry", throwOnError: true)!;
        MethodInfo initialize = registry.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic)!;
        initialize.Invoke(null, null);
        initialize.Invoke(null, null);
        Type fixture = assembly.GetType("Fixture", throwOnError: true)!;
        Assert.Equal(0, fixture.GetField("MetadataCalls")!.GetValue(null));
        Assert.Equal(0, fixture.GetField("ConfigCalls")!.GetValue(null));
    }

    [Fact]
    public void Generator_Should_Keep_Assembly_Identity_For_Referenced_Configs_With_The_Same_Full_Name()
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
            Assert.True((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, lookup)!);
            var config = (ClientConfig)Activator.CreateInstance(type)!;
            Assert.True(new Utils.SessionReflection().SetForcePathStyle(config));
            Assert.True(((AmazonS3Config)config).ForcePathStyle);
        }
    }

    [Fact]
    public void Generator_Should_Discover_Inaccessible_Referenced_Configs_Without_Consumer_Syntax()
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
        Assert.True((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, lookup)!);
        Assert.True(new Utils.SessionReflection().SetForcePathStyle(config));
        Assert.True(((AmazonS3Config)config).ForcePathStyle);
    }

    [Fact]
    public void Generator_Should_Register_Closed_Signature_Types_Lazily_Without_Consumer_Generic_Syntax()
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
            Assert.True((bool)registry.GetMethod("TryGetClient")!.Invoke(null, lookup)!);
            object?[] configLookup = { configDefinition.MakeGenericType(argument), null };
            Assert.True((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, configLookup)!);
        }

        object?[] signatureConfigLookup = { configDefinition.MakeGenericType(typeof(decimal)), null };
        Assert.True((bool)registry.GetMethod("TryGetConfig")!.Invoke(null, signatureConfigLookup)!);
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
        Assert.True((bool)registry.GetMethod("TryGetClient")!.Invoke(null, lookup)!);
        Assert.IsType<AmazonS3Config>(new Utils.SessionReflection().CreateClientConfig(type));
        Assert.Throws<InvalidOperationException>(() => new Utils.SessionReflection().ExtractServiceMetadata(type));
    }

    private static Assembly Load(Compilation compilation)
    {
        using var stream = new MemoryStream();
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
