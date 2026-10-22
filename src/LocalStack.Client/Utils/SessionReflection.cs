#pragma warning disable S3011 // We need to use reflection to access private fields for service metadata
namespace LocalStack.Client.Utils;

public class SessionReflection : ISessionReflection
{
    public IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return ExtractServiceMetadata(clientType);
    }

    public IServiceMetadata ExtractServiceMetadata(Type clientType)
    {
        if (clientType == null)
        {
            throw new ArgumentNullException(nameof(clientType));
        }

#if NET8_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(clientType, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return entry.Metadata();
        }

        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            throw new InvalidOperationException($"Invalid service type {clientType}");
        }
#endif
        return ExtractLegacyServiceMetadata(clientType);
    }

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded client metadata requires its original private fields.")]
#endif
    private static IServiceMetadata ExtractLegacyServiceMetadata(Type clientType)
    {
        FieldInfo serviceMetadataField = clientType.GetField("serviceMetadata", BindingFlags.Static | BindingFlags.NonPublic) ??
                                         throw new InvalidOperationException($"Invalid service type {clientType}");

        #pragma warning disable CS8600,CS8603 // Not possible to get null value from this private field
        var serviceMetadata = (IServiceMetadata)serviceMetadataField.GetValue(null);

        return serviceMetadata;
    }

    public ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return CreateClientConfig(clientType);
    }

    public ClientConfig CreateClientConfig(Type clientType)
    {
        if (clientType == null)
        {
            throw new ArgumentNullException(nameof(clientType));
        }

#if NET8_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(clientType, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return entry.Config();
        }

        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            throw new InvalidOperationException("Sequence contains no matching element");
        }
#endif
        return CreateLegacyClientConfig(clientType);
    }

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded configurations require their original constructors.")]
#endif
    private static ClientConfig CreateLegacyClientConfig(Type clientType)
    {
        ConstructorInfo clientConstructorInfo = FindConstructorWithCredentialsAndClientConfig(clientType);
        ParameterInfo clientConfigParam = clientConstructorInfo.GetParameters()[1];

        return (ClientConfig)Activator.CreateInstance(clientConfigParam.ParameterType);
    }

    public void SetClientRegion(AmazonServiceClient amazonServiceClient, string systemName)
    {
        if (amazonServiceClient == null)
        {
            throw new ArgumentNullException(nameof(amazonServiceClient));
        }

#if NET8_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetConfig(amazonServiceClient.Config.GetType(), out GeneratedClientRegistry.ConfigEntry? entry))
        {
            entry.Region(amazonServiceClient.Config, systemName);
            return;
        }

        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            throw new NotSupportedException($"Configuration type {amazonServiceClient.Config.GetType()} was not present when the application was compiled.");
        }
#endif
        SetLegacyClientRegion(amazonServiceClient, systemName);
    }

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded configurations require their original properties.")]
#endif
    private static void SetLegacyClientRegion(AmazonServiceClient amazonServiceClient, string systemName)
    {
        PropertyInfo? regionEndpointProperty = amazonServiceClient.Config.GetType()
                                                                  .GetProperty(nameof(amazonServiceClient.Config.RegionEndpoint),
                                                                               BindingFlags.Public | BindingFlags.Instance);
        regionEndpointProperty?.SetValue(amazonServiceClient.Config, RegionEndpoint.GetBySystemName(systemName));
    }

    public bool SetForcePathStyle(ClientConfig clientConfig, bool value = true)
    {
        if (clientConfig == null)
        {
            throw new ArgumentNullException(nameof(clientConfig));
        }

#if NET8_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetConfig(clientConfig.GetType(), out GeneratedClientRegistry.ConfigEntry? entry))
        {
            return entry.Style(clientConfig, value);
        }

        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            throw new NotSupportedException($"Configuration type {clientConfig.GetType()} was not present when the application was compiled.");
        }
#endif
        return SetLegacyForcePathStyle(clientConfig, value);
    }

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded configurations require their original properties.")]
#endif
    private static bool SetLegacyForcePathStyle(ClientConfig clientConfig, bool value)
    {
        PropertyInfo? forcePathStyleProperty = clientConfig.GetType().GetProperty("ForcePathStyle", BindingFlags.Public | BindingFlags.Instance);

        if (forcePathStyleProperty == null)
        {
            return false;
        }

        forcePathStyleProperty.SetValue(clientConfig, value);

        return true;
    }

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("Dynamically loaded clients require their original constructors.")]
#endif
    private static ConstructorInfo FindConstructorWithCredentialsAndClientConfig(Type clientType)
    {
        return clientType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                         .Single(info =>
                         {
                             ParameterInfo[] parameterInfos = info.GetParameters();

                             if (parameterInfos.Length != 2)
                             {
                                 return false;
                             }

                             ParameterInfo credentialsParameter = parameterInfos[0];
                             ParameterInfo clientConfigParameter = parameterInfos[1];

                             return credentialsParameter.Name == "credentials" &&
                                    credentialsParameter.ParameterType == typeof(AWSCredentials) &&
                                    clientConfigParameter.Name == "clientConfig" &&
                                    clientConfigParameter.ParameterType.IsSubclassOf(typeof(ClientConfig));
                         });
    }
}
