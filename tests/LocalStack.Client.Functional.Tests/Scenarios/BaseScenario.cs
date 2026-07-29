namespace LocalStack.Client.Functional.Tests.Scenarios;

/// <summary>
/// Base for every functional scenario. Owns the service provider and, crucially, the cleanup of any
/// AWS resource a test creates.
/// </summary>
/// <remarks>
/// All scenario classes in a collection share a single LocalStack container, so a resource left behind by
/// one test is visible to every test that runs after it - including in other classes. That makes assertions
/// about container state (for example <c>Assert.Single</c> over <c>ListTopics</c>) order dependent, and an
/// order-dependent test is not isolated.
/// <para>
/// xUnit creates a new instance of the test class for every test method and awaits <see cref="DisposeAsync" />
/// afterwards whether the test passed or failed. Registering cleanup here therefore gives per-test teardown
/// that still runs when an assertion throws - which the previous "delete at the end of the test body" pattern
/// could not do.
/// </para>
/// </remarks>
public abstract class BaseScenario : IAsyncLifetime
{
    private readonly List<CleanupRegistration> _cleanups = [];

    protected BaseScenario(TestFixture testFixture, ILocalStackFixture localStackFixture, string configFile = TestConstants.LocalStackConfig, bool useServiceUrl = false)
    {
        ArgumentNullException.ThrowIfNull(testFixture);
        ArgumentNullException.ThrowIfNull(localStackFixture);

        ushort mappedPublicPort = localStackFixture.LocalStackContainer.GetMappedPublicPort(4566);
        ConfigurationBuilder configurationBuilder = testFixture.CreateConfigureAppConfiguration(configFile, mappedPublicPort);
        Configuration = configurationBuilder.Build();

        IServiceCollection serviceCollection = testFixture.CreateServiceCollection(Configuration);

        serviceCollection.AddAwsService<IAmazonS3>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonDynamoDB>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonSQS>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonSimpleNotificationService>(useServiceUrl: useServiceUrl)
                         .AddAwsService<IAmazonCloudFormation>(useServiceUrl: useServiceUrl);

        serviceCollection.AddLogging();

        ServiceProvider = serviceCollection.BuildServiceProvider();
        LocalStackFixture = localStackFixture;
    }

    protected ILocalStackFixture LocalStackFixture { get; set; }

    protected IConfiguration Configuration { get; set; }

    protected ServiceProvider ServiceProvider { get; private set; }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes everything the test registered, most recent first, then fails the test if anything survived.
    /// </summary>
    /// <remarks>
    /// Reverse order matters: a queue subscribed to a topic has to go before the topic it depends on.
    /// Cleanup failures are reported rather than swallowed - a leaked resource is a defect, and staying silent
    /// about it is what let the shared container drift in the first place.
    /// </remarks>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types",
                     Justification = "Teardown must attempt every registered resource and report all failures together; narrowing the catch would let one AWS service's error abandon the rest.")]
    public virtual async Task DisposeAsync()
    {
        List<Exception>? failures = null;

        for (int index = _cleanups.Count - 1; index >= 0; index--)
        {
            CleanupRegistration registration = _cleanups[index];

            try
            {
                await registration.Cleanup().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                (failures ??= []).Add(new System.InvalidOperationException($"Failed to clean up '{registration.ResourceId}'.", exception));
            }
        }

        _cleanups.Clear();
        await ServiceProvider.DisposeAsync().ConfigureAwait(false);

        if (failures is not null)
        {
            throw new AggregateException(
                "Test resources were left behind. Scenario classes share one LocalStack container per collection, so a leak makes later tests order dependent.",
                failures);
        }
    }

    /// <summary>
    /// Registers a resource to be removed after the current test, whatever its outcome.
    /// </summary>
    /// <param name="resourceId">Identifier used both for de-registration and for cleanup failure messages.</param>
    /// <param name="cleanup">The removal call.</param>
    protected void TrackForCleanup(string resourceId, Func<Task> cleanup)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);
        ArgumentNullException.ThrowIfNull(cleanup);

        _cleanups.Add(new CleanupRegistration(resourceId, cleanup));
    }

    /// <summary>
    /// Drops a registration because the test removed the resource itself - deletion being the behaviour under
    /// test - so teardown does not try to delete it twice.
    /// </summary>
    protected void UntrackCleanup(string resourceId)
    {
        _cleanups.RemoveAll(registration => string.Equals(registration.ResourceId, resourceId, StringComparison.Ordinal));
    }

    private sealed record CleanupRegistration(string ResourceId, Func<Task> Cleanup);
}
