// The nested *Factory types below are reflection-only stand-ins for AWS SDK ClientFactory<T> shapes.
// They are never constructed with `new` - only through ConstructorInfo.Invoke - so the analyzers
// cannot see the usage and report them as dead code.
#pragma warning disable CA1812 // Internal class is apparently never instantiated
#pragma warning disable S1144 // Remove unused constructor of private type

namespace LocalStack.Client.Extensions.Tests;

/// <summary>
/// Guards the constructor-resolution logic behind <see cref="AwsClientFactoryWrapper" />.
/// </summary>
/// <remarks>
/// AWSSDK.Extensions.NETCore.Setup 4.0.4 changed the internal <c>ClientFactory&lt;T&gt;</c> constructor from
/// <c>(AWSOptions)</c> to <c>(AWSOptions, Action&lt;ClientConfig, IServiceProvider&gt; = null)</c> and broke us
/// (issue #52). We pin a floor, but a NuGet floor is a minimum - consumers still float upwards, so the next
/// such change reaches them before it reaches our pinned build. These tests therefore cover shapes AWS has
/// <em>not</em> shipped yet, using local stand-ins, because a test against the real SDK can only ever see
/// whichever version is currently pinned.
/// </remarks>
public class AwsClientFactoryWrapperResolutionTests
{
    [Fact]
    public void SelectFactoryConstructor_Should_Select_Ctor_On_The_Current_Shape()
    {
        ConstructorInfo? selected = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<CurrentShapeFactory>());

        Assert.NotNull(selected);
        Assert.Equal(2, selected.GetParameters().Length);
        Assert.Equal(typeof(AWSOptions), selected.GetParameters()[0].ParameterType);
    }

    [Fact]
    public void SelectFactoryConstructor_Should_Select_Ctor_On_Unknown_Future_Shape()
    {
        ConstructorInfo? selected = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<FutureShapeFactory>());

        Assert.NotNull(selected);
        Assert.Equal(3, selected.GetParameters().Length);
        Assert.Equal(typeof(AWSOptions), selected.GetParameters()[0].ParameterType);
    }

    [Fact]
    public void SelectFactoryConstructor_Should_Ignore_The_Parameterless_Ctor()
    {
        ConstructorInfo? selected = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<CurrentShapeFactory>());

        Assert.NotNull(selected);
        Assert.NotEmpty(selected.GetParameters());
    }

    [Fact]
    public void SelectFactoryConstructor_Should_Prefer_The_Fewest_Parameter_Overload()
    {
        ConstructorInfo? selected = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<MultipleOverloadFactory>());

        Assert.NotNull(selected);
        Assert.Single(selected.GetParameters());
    }

    [Fact]
    public void SelectFactoryConstructor_Should_Return_Null_When_No_Ctor_Takes_AwsOptions_First()
    {
        ConstructorInfo? selected = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<UnrelatedFactory>());

        Assert.Null(selected);
    }

    [Fact]
    public void BuildConstructorArguments_Should_Default_The_Trailing_Parameters()
    {
        ConstructorInfo constructor = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<CurrentShapeFactory>())!;
        var awsOptions = new AWSOptions();

        object?[] arguments = AwsClientFactoryWrapper.BuildConstructorArguments(constructor, awsOptions);

        Assert.Equal(2, arguments.Length);
        Assert.Same(awsOptions, arguments[0]);
        Assert.Null(arguments[1]);
    }

    [Fact]
    public void BuildConstructorArguments_Should_Default_Value_Type_Parameters_Without_Defaults()
    {
        ConstructorInfo constructor = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<FutureShapeFactory>())!;

        object?[] arguments = AwsClientFactoryWrapper.BuildConstructorArguments(constructor, new AWSOptions());

        Assert.Equal(3, arguments.Length);
        Assert.Null(arguments[1]);
        Assert.Equal(0, arguments[2]);
    }

    [Fact]
    public void BuildConstructorArguments_Should_Allow_Null_AwsOptions()
    {
        // AddAwsService<T>() without explicit options passes null, and the AWS factory then resolves
        // AWSOptions from the IServiceProvider or IConfiguration. Null must survive the call.
        ConstructorInfo constructor = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<CurrentShapeFactory>())!;

        object?[] arguments = AwsClientFactoryWrapper.BuildConstructorArguments(constructor, awsOptions: null);

        Assert.Null(arguments[0]);
    }

    [Fact]
    public void BuildConstructorArguments_Should_Produce_Invokable_Arguments()
    {
        ConstructorInfo constructor = AwsClientFactoryWrapper.SelectFactoryConstructor(CtorsOf<CurrentShapeFactory>())!;
        var awsOptions = new AWSOptions();

        var created = (CurrentShapeFactory)constructor.Invoke(AwsClientFactoryWrapper.BuildConstructorArguments(constructor, awsOptions));

        Assert.Same(awsOptions, created.AwsOptions);
        Assert.Null(created.ConfigAction);
    }

    [Fact]
    public void DescribeConstructors_Should_Render_Discovered_Signatures()
    {
        string described = AwsClientFactoryWrapper.DescribeConstructors(CtorsOf<CurrentShapeFactory>());

        Assert.Contains("AWSOptions", described, StringComparison.Ordinal);
        Assert.Contains(".ctor(", described, StringComparison.Ordinal);
    }

    [Fact]
    public void DescribeConstructors_Should_Report_None_When_Empty()
    {
        Assert.Equal("<none>", AwsClientFactoryWrapper.DescribeConstructors([]));
    }

    [Fact]
    public void SelectFactoryConstructor_Should_Resolve_Against_The_Real_Aws_Sdk()
    {
        // Deliberately shape-agnostic: this also runs on the floating canary track, where it must go red
        // only when we genuinely cannot resolve - not merely because AWS appended another parameter.
        Type factoryType = typeof(ConfigurationException).Assembly.GetType("Amazon.Extensions.NETCore.Setup.ClientFactory`1")!
                                                         .MakeGenericType(typeof(IAmazonS3));

        ConstructorInfo[] candidates = factoryType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        ConstructorInfo? selected = AwsClientFactoryWrapper.SelectFactoryConstructor(candidates);

        Assert.NotNull(selected);

        ParameterInfo[] parameters = selected.GetParameters();
        Assert.NotEmpty(parameters);
        Assert.Equal(typeof(AWSOptions), parameters[0].ParameterType);
    }

    private static ConstructorInfo[] CtorsOf<T>()
    {
        return typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
    }

    /// <summary>Mirrors AWSSDK.Extensions.NETCore.Setup &gt;= 4.0.4, the shape we pin against.</summary>
    private sealed class CurrentShapeFactory
    {
        private CurrentShapeFactory()
        {
        }

        internal CurrentShapeFactory(AWSOptions awsOptions, Action<ClientConfig, IServiceProvider>? configAction = null)
        {
            AwsOptions = awsOptions;
            ConfigAction = configAction;
        }

        public AWSOptions? AwsOptions { get; }

        public Action<ClientConfig, IServiceProvider>? ConfigAction { get; }
    }

    /// <summary>A shape AWS has not shipped, including a required value-type parameter.</summary>
    private sealed class FutureShapeFactory
    {
        internal FutureShapeFactory(AWSOptions awsOptions, Action<ClientConfig, IServiceProvider>? configAction, int retryCount)
        {
            AwsOptions = awsOptions;
            ConfigAction = configAction;
            RetryCount = retryCount;
        }

        public AWSOptions AwsOptions { get; }

        public Action<ClientConfig, IServiceProvider>? ConfigAction { get; }

        public int RetryCount { get; }
    }

    private sealed class MultipleOverloadFactory
    {
        internal MultipleOverloadFactory(AWSOptions awsOptions, Action<ClientConfig, IServiceProvider>? configAction, bool flag)
        {
            _ = awsOptions;
            _ = configAction;
            _ = flag;
        }

        internal MultipleOverloadFactory(AWSOptions awsOptions)
        {
            _ = awsOptions;
        }
    }

    private sealed class UnrelatedFactory
    {
        internal UnrelatedFactory(string name)
        {
            _ = name;
        }

        internal UnrelatedFactory(IServiceProvider provider, AWSOptions awsOptions)
        {
            _ = provider;
            _ = awsOptions;
        }
    }
}
