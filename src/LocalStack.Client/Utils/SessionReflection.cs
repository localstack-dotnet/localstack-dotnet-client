#pragma warning disable S3011 // We need to use reflection to access private fields for service metadata
namespace LocalStack.Client.Utils;

public class SessionReflection : ISessionReflection
{
    public IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return ExtractServiceMetadata(clientType);
    }

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Accesses private field 'serviceMetadata' with reflection; not safe for Native AOT."),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public IServiceMetadata ExtractServiceMetadata(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
#endif
        Type clientType
        )
    {
        if (clientType == null)
        {
            throw new ArgumentNullException(nameof(clientType));
        }

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

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Uses Activator.CreateInstance on derived ClientConfig types; not safe for Native AOT."),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public ClientConfig CreateClientConfig(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        Type clientType

        )
    {
        if (clientType == null)
        {
            throw new ArgumentNullException(nameof(clientType));
        }

        ConstructorInfo clientConstructorInfo = FindConstructorWithCredentialsAndClientConfig(clientType);
        ParameterInfo clientConfigParam = clientConstructorInfo.GetParameters()[1];

        return (ClientConfig)Activator.CreateInstance(clientConfigParam.ParameterType);
    }

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Reflects over Config.RegionEndpoint property; not safe for Native AOT."),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public void SetClientRegion(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
        AmazonServiceClient amazonServiceClient,
        string systemName)
    {
        if (amazonServiceClient == null)
        {
            throw new ArgumentNullException(nameof(amazonServiceClient));
        }

        PropertyInfo? regionEndpointProperty = amazonServiceClient.Config.GetType()
                                                                  .GetProperty(nameof(amazonServiceClient.Config.RegionEndpoint),
                                                                               BindingFlags.Public | BindingFlags.Instance);
        regionEndpointProperty?.SetValue(amazonServiceClient.Config, RegionEndpoint.GetBySystemName(systemName));
    }

#if NET8_0_OR_GREATER
    [RequiresDynamicCode("Reflects over ForcePathStyle property; not safe for Native AOT."),
     RequiresUnreferencedCode("Reflection may break when IL trimming removes private members. We’re migrating to a source‑generated path in vNext.")]
#endif
    public bool SetForcePathStyle(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
        ClientConfig clientConfig,
        bool value = true)
    {
        if (clientConfig == null)
        {
            throw new ArgumentNullException(nameof(clientConfig));
        }

        PropertyInfo? forcePathStyleProperty = clientConfig.GetType().GetProperty("ForcePathStyle", BindingFlags.Public | BindingFlags.Instance);

        if (forcePathStyleProperty == null)
        {
            return false;
        }

        forcePathStyleProperty.SetValue(clientConfig, value);

        return true;
    }

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