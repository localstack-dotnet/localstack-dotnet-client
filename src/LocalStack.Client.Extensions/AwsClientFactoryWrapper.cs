#pragma warning disable S3011 // We need to use reflection to access private fields for service metadata
#pragma warning disable CS8600,CS8603 // Not possible to get null value from this private field
#pragma warning disable CA1802 // Deliberately readonly, not const - see the note on the fields below
namespace LocalStack.Client.Extensions;

public sealed class AwsClientFactoryWrapper : IAwsClientFactoryWrapper
{
    // These MUST stay 'readonly' rather than 'const'. AwsClientFactoryWrapperTests overwrites them by
    // reflection to simulate the AWS SDK renaming its internals, which is how the failure paths below are
    // covered. A 'const' is inlined at every use site, so overwriting the field would change nothing and
    // those tests would silently stop testing anything. CA1802/S3962 flag this; they cannot see the tests.
    private static readonly string ClientFactoryGenericTypeName = "Amazon.Extensions.NETCore.Setup.ClientFactory`1";
    private static readonly string CreateServiceClientMethodName = "CreateServiceClient";

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Creates generic ClientFactory<T> and invokes internal members via reflection"),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public AmazonServiceClient CreateServiceClient<TClient>(IServiceProvider provider, AWSOptions? awsOptions) where TClient : IAmazonService
    {
        Type? genericFactoryType = typeof(ConfigurationException).Assembly.GetType(ClientFactoryGenericTypeName);

        if (genericFactoryType == null)
        {
            throw new LocalStackClientConfigurationException($"Failed to find internal ClientFactory<T> in {ClientFactoryGenericTypeName}");
        }

        // Create ClientFactory<TClient>
        Type concreteFactoryType = genericFactoryType.MakeGenericType(typeof(TClient));
        ConstructorInfo[] candidates = concreteFactoryType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        ConstructorInfo? constructor = SelectFactoryConstructor(candidates);

        if (constructor == null)
        {
            throw new LocalStackClientConfigurationException(
                $"ClientFactory<T> has no non-public instance constructor whose first parameter is {nameof(AWSOptions)}. " +
                $"Discovered constructors: {DescribeConstructors(candidates)}. " +
                "This usually means the AWS SDK changed its internals; please open an issue at " +
                "https://github.com/localstack-dotnet/localstack-dotnet-client/issues");
        }

        object factory = constructor.Invoke(BuildConstructorArguments(constructor, awsOptions));
        MethodInfo? createMethod = factory.GetType().GetMethod(CreateServiceClientMethodName, BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(IServiceProvider) }, null);

        if (createMethod == null)
        {
            throw new LocalStackClientConfigurationException(
                $"ClientFactory<T> missing {CreateServiceClientMethodName}(IServiceProvider) method. " +
                "This usually means the AWS SDK changed its internals; please open an issue at " +
                "https://github.com/localstack-dotnet/localstack-dotnet-client/issues");
        }

        object serviceInstance = createMethod.Invoke(factory, new object[] { provider });
        return (AmazonServiceClient)serviceInstance;
    }

    /// <summary>
    /// Picks the constructor to build <c>ClientFactory&lt;T&gt;</c> with.
    /// </summary>
    /// <remarks>
    /// AWS has changed this constructor before: AWSSDK.Extensions.NETCore.Setup 4.0.4 replaced
    /// <c>ClientFactory(AWSOptions)</c> with <c>ClientFactory(AWSOptions, Action&lt;ClientConfig, IServiceProvider&gt; = null)</c>,
    /// which broke exact-signature matching (issue #52). It is source-compatible on their side - the new
    /// parameter is optional - so it ships as a patch with no signal in the release notes.
    /// <para>
    /// We therefore match on the <see cref="AWSOptions"/> parameter instead of the full signature, and prefer
    /// the smallest overload so the choice stays deterministic if AWS appends further optional parameters.
    /// Requiring <see cref="AWSOptions"/> in first position also naturally excludes the private parameterless
    /// constructor that has been present in every version.
    /// </para>
    /// </remarks>
    internal static ConstructorInfo? SelectFactoryConstructor(ConstructorInfo[] candidates)
    {
        if (candidates == null)
        {
            throw new ArgumentNullException(nameof(candidates));
        }

        ConstructorInfo? selected = null;
        var fewestParameters = int.MaxValue;

        foreach (ConstructorInfo candidate in candidates)
        {
            ParameterInfo[] parameters = candidate.GetParameters();

            if (parameters.Length == 0 || parameters[0].ParameterType != typeof(AWSOptions))
            {
                continue;
            }

            if (parameters.Length < fewestParameters)
            {
                selected = candidate;
                fewestParameters = parameters.Length;
            }
        }

        return selected;
    }

    /// <summary>
    /// Builds the argument array for the selected constructor: the caller's <see cref="AWSOptions"/> first,
    /// then each remaining parameter's own default.
    /// </summary>
    /// <remarks>
    /// Passing <c>null</c> for the trailing parameters is not a workaround - it is exactly what the AWS SDK
    /// itself passes. In 4.0.4+ the added <c>configAction</c> parameter is declared <c>= null</c> and is
    /// null-guarded before use, and <c>AWSOptions</c> may legitimately be null because the factory then
    /// resolves it from the <see cref="IServiceProvider"/> or <c>IConfiguration</c>.
    /// </remarks>
#if NET8_0_OR_GREATER
    [RequiresDynamicCode("May instantiate value-type parameter defaults via Activator.CreateInstance.")]
#endif
    internal static object?[] BuildConstructorArguments(ConstructorInfo constructor, AWSOptions? awsOptions)
    {
        if (constructor == null)
        {
            throw new ArgumentNullException(nameof(constructor));
        }

        ParameterInfo[] parameters = constructor.GetParameters();
        var arguments = new object?[parameters.Length];
        arguments[0] = awsOptions;

        for (var index = 1; index < parameters.Length; index++)
        {
            ParameterInfo parameter = parameters[index];

            if (parameter.HasDefaultValue)
            {
                arguments[index] = parameter.DefaultValue;
            }
            else if (parameter.ParameterType.IsValueType)
            {
                arguments[index] = Activator.CreateInstance(parameter.ParameterType);
            }
            else
            {
                arguments[index] = null;
            }
        }

        return arguments;
    }

    /// <summary>
    /// Renders the discovered constructor signatures so an unexpected AWS SDK change can be diagnosed
    /// straight from a bug report, instead of requiring a version bisect.
    /// </summary>
    internal static string DescribeConstructors(ConstructorInfo[] candidates)
    {
        if (candidates == null)
        {
            throw new ArgumentNullException(nameof(candidates));
        }

        if (candidates.Length == 0)
        {
            return "<none>";
        }

        var builder = new StringBuilder();

        foreach (ConstructorInfo candidate in candidates)
        {
            if (builder.Length > 0)
            {
                builder.Append("; ");
            }

            builder.Append(".ctor(");
            ParameterInfo[] parameters = candidate.GetParameters();

            for (var index = 0; index < parameters.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                AppendTypeName(builder, parameters[index].ParameterType);
            }

            builder.Append(')');
        }

        return builder.ToString();
    }

    /// <summary>
    /// Writes a readable type name, expanding generic arguments.
    /// </summary>
    /// <remarks>
    /// <see cref="MemberInfo.Name" /> renders the parameter added in 4.0.4 as <c>Action`2</c>, dropping exactly the part
    /// that identifies it. Since this text is what a bug report will quote, the arguments are spelled out so the signature
    /// reads as <c>Action&lt;ClientConfig, IServiceProvider&gt;</c>.
    /// </remarks>
    private static void AppendTypeName(StringBuilder builder, Type type)
    {
        if (!type.IsGenericType)
        {
            builder.Append(type.Name);

            return;
        }

        // Split rather than IndexOf: the StringComparison overload CA1307 asks for does not exist on
        // netstandard2.0, and this runs once, on a failure path, so the extra array is irrelevant.
        builder.Append(type.Name.Split('`')[0]);
        builder.Append('<');

        Type[] arguments = type.GetGenericArguments();

        for (var index = 0; index < arguments.Length; index++)
        {
            if (index > 0)
            {
                builder.Append(", ");
            }

            AppendTypeName(builder, arguments[index]);
        }

        builder.Append('>');
    }
}
