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

#if NET10_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(clientType, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return entry.Metadata();
        }

        if (ReflectionFallback.IsSupported)
        {
            return ExtractLegacyServiceMetadata(clientType);
        }
        else
        {
            throw new InvalidOperationException($"Invalid service type {clientType}");
        }
#else
        return ExtractLegacyServiceMetadata(clientType);
#endif
    }

#if NET10_0_OR_GREATER
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

#if NET10_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetClient(clientType, out GeneratedClientRegistry.ClientEntry? entry))
        {
            return entry.CreateConfig();
        }

        if (ReflectionFallback.IsSupported)
        {
            return CreateLegacyClientConfig(clientType);
        }
        else
        {
            throw new InvalidOperationException("Sequence contains no matching element");
        }
#else
        return CreateLegacyClientConfig(clientType);
#endif
    }

#if NET10_0_OR_GREATER
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

#if NET10_0_OR_GREATER
        if (amazonServiceClient.Config is ClientConfig clientConfig &&
            GeneratedClientRegistry.TryGetConfig(clientConfig.GetType(), out GeneratedClientRegistry.ConfigEntry? entry) && entry.PlainRegion)
        {
            SetPlainClientRegion(clientConfig, systemName);
            return;
        }

        if (ReflectionFallback.IsSupported)
        {
            SetLegacyClientRegion(amazonServiceClient, systemName);
        }
        else if (amazonServiceClient.Config is ClientConfig plainConfig)
        {
            SetPlainClientRegion(plainConfig, systemName);
        }
#else
        SetLegacyClientRegion(amazonServiceClient, systemName);
#endif
    }

#if NET10_0_OR_GREATER
    private static void SetPlainClientRegion(ClientConfig config, string systemName)
    {
        try
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(systemName);
        }
        catch (Exception e)
        {
            throw new TargetInvocationException(e);
        }
    }

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

#if NET10_0_OR_GREATER
        if (S3ForcePathStyle.TrySet(clientConfig, value))
        {
            return true;
        }
#endif
#if NET10_0_OR_GREATER
        if (GeneratedClientRegistry.TryGetConfig(clientConfig.GetType(), out GeneratedClientRegistry.ConfigEntry? entry) && entry.Style != null)
        {
            return entry.Style(clientConfig, value);
        }

        if (ReflectionFallback.IsSupported)
        {
            return SetLegacyForcePathStyle(clientConfig, value);
        }

        return false;
#else
        return SetLegacyForcePathStyle(clientConfig, value);
#endif
    }

#if NET10_0_OR_GREATER
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

#if NET10_0_OR_GREATER
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
