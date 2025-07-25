﻿namespace LocalStack.Client.Extensions;

public static class ServiceCollectionExtensions
{
    private const string LocalStackSectionName = "LocalStack";

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("ConfigurationBinder.Bind() sets options via reflection; keep non‑public setters or suppress in AOT build.")]
#endif
    public static IServiceCollection AddLocalStack(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.Configure<LocalStackOptions>(options => configuration.GetSection(LocalStackSectionName).Bind(options, c => c.BindNonPublicProperties = true));

        collection.Configure<SessionOptions>(options => configuration.GetSection(LocalStackSectionName)
                                                                     .GetSection(nameof(LocalStackOptions.Session))
                                                                     .Bind(options, c => c.BindNonPublicProperties = true));

        collection.Configure<ConfigOptions>(options => configuration.GetSection(LocalStackSectionName)
                                                                    .GetSection(nameof(LocalStackOptions.Config))
                                                                    .Bind(options, c => c.BindNonPublicProperties = true));

        return collection.AddLocalStackServices();
    }

    /// <summary>
    /// Adds the AWSOptions object to the dependency injection framework providing information
    /// that will be used to construct Amazon service clients.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="options">The default AWS options used to construct AWS service clients with.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddDefaultAwsOptions(this IServiceCollection collection, AWSOptions options)
    {
        collection.AddDefaultAWSOptions(options);

        return collection;
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework. The Amazon service client is not
    /// created until it is requested. If the ServiceLifetime property is set to Singleton, the default, then the same
    /// instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddAwsService<TService>(this IServiceCollection collection, ServiceLifetime lifetime = ServiceLifetime.Singleton,
                                                             bool useServiceUrl = false) where TService : IAmazonService
    {
        return AddAwsService<TService>(collection, null, lifetime, useServiceUrl);
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework. The Amazon service client is not
    /// created until it is requested. If the ServiceLifetime property is set to Singleton, the default, then the same
    /// instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="options">The AWS options used to create the service client overriding the default AWS options added using AddDefaultAWSOptions.</param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddAwsService<TService>(this IServiceCollection collection, AWSOptions? options,
                                                             ServiceLifetime lifetime = ServiceLifetime.Singleton, bool useServiceUrl = false)
        where TService : IAmazonService
    {
        ServiceDescriptor descriptor = GetServiceFactoryDescriptor<TService>(options, lifetime, useServiceUrl);

        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        collection.Add(descriptor);

        return collection;
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework. The Amazon service client is not
    /// created until it is requested. If the ServiceLifetime property is set to Singleton, the default, then the same
    /// instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddAWSServiceLocalStack<TService>(this IServiceCollection collection, ServiceLifetime lifetime = ServiceLifetime.Singleton,
                                                                       bool useServiceUrl = false) where TService : IAmazonService
    {
        return AddAWSServiceLocalStack<TService>(collection, null, lifetime, useServiceUrl);
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework. The Amazon service client is not
    /// created until it is requested. If the ServiceLifetime property is set to Singleton, the default, then the same
    /// instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="options">The AWS options used to create the service client overriding the default AWS options added using AddDefaultAWSOptions.</param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection AddAWSServiceLocalStack<TService>(this IServiceCollection collection, AWSOptions? options,
                                                                       ServiceLifetime lifetime = ServiceLifetime.Singleton, bool useServiceUrl = false)
        where TService : IAmazonService
    {
        return AddAwsService<TService>(collection, options, lifetime, useServiceUrl);
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework if the service type hasn't already been registered.
    /// The Amazon service client is not created until it is requested. If the ServiceLifetime property is set to Singleton,
    /// the default, then the same instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection TryAddAwsService<TService>(this IServiceCollection collection, ServiceLifetime lifetime = ServiceLifetime.Singleton,
                                                                bool useServiceUrl = false) where TService : IAmazonService
    {
        return TryAddAwsService<TService>(collection, null, lifetime);
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework if the service type hasn't already been registered.
    /// The Amazon service client is not created until it is requested. If the ServiceLifetime property is set to Singleton,
    /// the default, then the same instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="options">The AWS options used to create the service client overriding the default AWS options added using AddDefaultAWSOptions.</param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection TryAddAwsService<TService>(this IServiceCollection collection, AWSOptions? options,
                                                                ServiceLifetime lifetime = ServiceLifetime.Singleton, bool useServiceUrl = false)
        where TService : IAmazonService
    {
        ServiceDescriptor descriptor = GetServiceFactoryDescriptor<TService>(options, lifetime, useServiceUrl);
        collection.TryAdd(descriptor);

        return collection;
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework if the service type hasn't already been registered.
    /// The Amazon service client is not created until it is requested. If the ServiceLifetime property is set to Singleton,
    /// the default, then the same instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection TryAddAWSServiceLocalStack<TService>(this IServiceCollection collection, ServiceLifetime lifetime = ServiceLifetime.Singleton,
                                                                          bool useServiceUrl = false) where TService : IAmazonService
    {
        return TryAddAWSServiceLocalStack<TService>(collection, null, lifetime, useServiceUrl);
    }

    /// <summary>
    /// Adds the Amazon service client to the dependency injection framework if the service type hasn't already been registered.
    /// The Amazon service client is not created until it is requested. If the ServiceLifetime property is set to Singleton,
    /// the default, then the same instance will be reused for the lifetime of the process and the object should not be disposed.
    /// </summary>
    /// <typeparam name="TService">The AWS service interface, like IAmazonS3.</typeparam>
    /// <param name="collection"></param>
    /// <param name="options">The AWS options used to create the service client overriding the default AWS options added using AddDefaultAWSOptions.</param>
    /// <param name="lifetime">The lifetime of the service client created. The default is Singleton.</param>
    /// <param name="useServiceUrl">(LocalStack) Determines whether the service client will use RegionEndpoint or ServiceUrl. The default is false.</param>
    /// <returns>Returns back the IServiceCollection to continue the fluent system of IServiceCollection.</returns>
    public static IServiceCollection TryAddAWSServiceLocalStack<TService>(this IServiceCollection collection, AWSOptions? options,
                                                                          ServiceLifetime lifetime = ServiceLifetime.Singleton, bool useServiceUrl = false)
        where TService : IAmazonService
    {
        return TryAddAwsService<TService>(collection, options, lifetime, useServiceUrl);
    }

    private static IServiceCollection AddLocalStackServices(this IServiceCollection services)
    {
        services.AddTransient<IConfig, Config>(provider =>
                {
                    ConfigOptions options = provider.GetRequiredService<IOptions<ConfigOptions>>().Value;

                    return new Config(options);
                })
                .AddSingleton<ISessionReflection, SessionReflection>()
                .AddSingleton<ISession, Session>(provider =>
                {
                    SessionOptions sessionOptions = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
                    var config = provider.GetRequiredService<IConfig>();
                    var sessionReflection = provider.GetRequiredService<ISessionReflection>();

                    return new Session(sessionOptions, config, sessionReflection);
                })
                .AddSingleton<IAwsClientFactoryWrapper, AwsClientFactoryWrapper>();

        return services;
    }

    private static ServiceDescriptor GetServiceFactoryDescriptor<TService>(AWSOptions? options, ServiceLifetime lifetime, bool useServiceUrl = false)
        where TService : IAmazonService
    {
        var descriptor = new ServiceDescriptor(typeof(TService), provider =>
        {
            LocalStackOptions localStackOptions = provider.GetRequiredService<IOptions<LocalStackOptions>>().Value;

            AmazonServiceClient serviceInstance;

            if (localStackOptions.UseLocalStack)
            {
                var session = provider.GetRequiredService<ISession>();
                serviceInstance = session.CreateClientByInterface<TService>(useServiceUrl);
            }
            else
            {
                var clientFactory = provider.GetRequiredService<IAwsClientFactoryWrapper>();
                serviceInstance = clientFactory.CreateServiceClient<TService>(provider, options!);
            }

            return serviceInstance;
        }, lifetime);

        return descriptor;
    }
}