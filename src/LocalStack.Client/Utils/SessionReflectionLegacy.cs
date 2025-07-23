#if NETSTANDARD2_0 || NET472
#pragma warning disable S3011 // We need to use reflection to access private fields for service metadata
using System.Reflection;

namespace LocalStack.Client.Utils;

/// <summary>
/// Legacy reflection-based implementation for .NET Framework and .NET Standard 2.0
/// Uses traditional reflection to access private AWS SDK members.
/// This class is excluded from .NET 8+ builds to ensure zero reflection code in AOT scenarios.
/// </summary>
internal sealed class SessionReflectionLegacy : ISessionReflection
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

        ConstructorInfo clientConstructorInfo = FindConstructorWithCredentialsAndClientConfig(clientType);
        ParameterInfo clientConfigParam = clientConstructorInfo.GetParameters()[1];

        return (ClientConfig)Activator.CreateInstance(clientConfigParam.ParameterType)!;
    }

    public void SetClientRegion(AmazonServiceClient amazonServiceClient, string systemName)
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

    public bool SetForcePathStyle(ClientConfig clientConfig, bool value = true)
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
#endif
