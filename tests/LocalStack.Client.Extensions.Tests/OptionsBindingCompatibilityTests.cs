namespace LocalStack.Client.Extensions.Tests;

public class OptionsBindingCompatibilityTests
{
    [Fact]
    public void AddLocalStack_Should_Bind_Options_Lazily_And_Keep_Nested_Options_Independent()
    {
        IConfigurationRoot configuration = Configuration(new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["localstack:uselocalstack"] = "true",
            ["LocalStack:Session:AwsAccessKeyId"] = "fixture-id",
            ["LocalStack:Session:AwsAccessKey"] = "fixture-secret",
            ["LocalStack:Session:AwsSessionToken"] = "fixture-token",
            ["LocalStack:Session:RegionName"] = "eu-west-2",
            ["LocalStack:Config:LocalStackHost"] = "fixture-host",
            ["LocalStack:Config:UseSsl"] = "true",
            ["LocalStack:Config:UseLegacyPorts"] = "true",
            ["LocalStack:Config:EdgePort"] = "0x1234",
            ["LocalStack:Ignored"] = "ignored",
        });
        var services = new ServiceCollection();
        services.AddLocalStack(configuration);
        configuration["LocalStack:Session:RegionName"] = "eu-north-1";
        using ServiceProvider provider = services.BuildServiceProvider();
        LocalStackOptions parent = provider.GetRequiredService<IOptions<LocalStackOptions>>().Value;
        SessionOptions session = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
        ConfigOptions config = provider.GetRequiredService<IOptions<ConfigOptions>>().Value;

        Assert.True(parent.UseLocalStack);
        Assert.NotSame(parent.Session, session);
        Assert.NotSame(parent.Config, config);
        foreach (SessionOptions value in new[] { parent.Session, session })
        {
            Assert.Equal("fixture-id", value.AwsAccessKeyId);
            Assert.Equal("fixture-secret", value.AwsAccessKey);
            Assert.Equal("fixture-token", value.AwsSessionToken);
            Assert.Equal("eu-north-1", value.RegionName);
        }

        foreach (ConfigOptions value in new[] { parent.Config, config })
        {
            Assert.Equal("fixture-host", value.LocalStackHost);
            Assert.True(value.UseSsl);
            Assert.True(value.UseLegacyPorts);
            Assert.Equal(4660, value.EdgePort);
        }

        configuration["LocalStack:Session:RegionName"] = "us-west-1";
        Assert.Same(session, provider.GetRequiredService<IOptions<SessionOptions>>().Value);
        Assert.Equal("eu-north-1", session.RegionName);
        using IServiceScope scope = provider.CreateScope();
        Assert.Equal("us-west-1", scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SessionOptions>>().Value.RegionName);
    }

    [Theory]
    [InlineData("0x10", 16)]
    [InlineData("&h10", 16)]
    [InlineData("#10", 16)]
    [InlineData(" +17 ", 17)]
    [InlineData("-17", -17)]
    public void AddLocalStack_Should_Convert_Integers_With_The_Invariant_Converter(string text, int expected)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            var services = new ServiceCollection();
            services.AddLocalStack(Configuration(new Dictionary<string, string?>(StringComparer.Ordinal) { ["LocalStack:Config:EdgePort"] = text }));
            using ServiceProvider provider = services.BuildServiceProvider();
            Assert.Equal(expected, provider.GetRequiredService<IOptions<ConfigOptions>>().Value.EdgePort);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Theory]
    [InlineData("LocalStack:Config:EdgePort", "", typeof(int))]
    [InlineData("LocalStack:Config:EdgePort", "2147483648", typeof(int))]
    [InlineData("LocalStack:Config:EdgePort", "1,2", typeof(int))]
    [InlineData("LocalStack:Config:UseSsl", "", typeof(bool))]
    [InlineData("LocalStack:Config:UseSsl", "yes", typeof(bool))]
    public void AddLocalStack_Should_Report_Path_Type_And_Converter_Exception_When_Conversion_Fails(string path, string text, Type type)
    {
        var services = new ServiceCollection();
        services.AddLocalStack(Configuration(new Dictionary<string, string?>(StringComparer.Ordinal) { [path] = text }));
        using ServiceProvider provider = services.BuildServiceProvider();

        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IOptions<ConfigOptions>>().Value);
        Assert.Equal($"Failed to convert configuration value at '{path}' to type '{type}'.", error.Message);
        var converter = type == typeof(int) ? (System.ComponentModel.TypeConverter)new System.ComponentModel.Int32Converter() : new System.ComponentModel.BooleanConverter();
        Exception expected = Assert.ThrowsAny<Exception>(() => converter.ConvertFromInvariantString(text));
        Assert.NotNull(error.InnerException);
        Assert.Equal(expected.GetType(), error.InnerException.GetType());
        Assert.Equal(expected.Message, error.InnerException.Message);
        Assert.Equal(expected.InnerException?.GetType(), error.InnerException.InnerException?.GetType());
    }

    [Fact]
    public void AddLocalStack_Should_Keep_Prior_Values_For_Missing_Null_Or_Child_Only_Scalars_And_Bind_Empty_Strings()
    {
        var services = new ServiceCollection();
        services.Configure<SessionOptions>(options => SetPrivate(options, nameof(SessionOptions.RegionName), "earlier"));
        services.Configure<ConfigOptions>(options => SetPrivate(options, nameof(ConfigOptions.EdgePort), 1234));
        services.AddLocalStack(Configuration(new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["LocalStack:Session:RegionName"] = null,
            ["LocalStack:Session:AwsAccessKey"] = "",
            ["LocalStack:Config:EdgePort:Child"] = "ignored",
            ["LocalStack:Config:UseSsl"] = " tRuE ",
        }));
        using ServiceProvider provider = services.BuildServiceProvider();
        SessionOptions session = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
        ConfigOptions config = provider.GetRequiredService<IOptions<ConfigOptions>>().Value;
        Assert.Equal("earlier", session.RegionName);
        Assert.Equal("", session.AwsAccessKey);
        Assert.Equal(new SessionOptions().AwsAccessKeyId, session.AwsAccessKeyId);
        Assert.Equal(1234, config.EdgePort);
        Assert.True(config.UseSsl);
    }

    [Fact]
    public void AddLocalStack_Should_Keep_Framework_Order_For_Configure_PostConfigure_Validation_And_Monitor()
    {
        var observed = new List<string>();
        var services = new ServiceCollection();
        services.Configure<SessionOptions>(options => observed.Add("before:" + options.RegionName));
        services.AddLocalStack(Configuration(new Dictionary<string, string?>(StringComparer.Ordinal) { ["LocalStack:Session:RegionName"] = "bound" }));
        services.Configure<SessionOptions>(options => observed.Add("after:" + options.RegionName));
        services.PostConfigure<SessionOptions>(options => observed.Add("post:" + options.RegionName));
        services.AddSingleton<IValidateOptions<SessionOptions>>(new RegionValidator());
        using ServiceProvider provider = services.BuildServiceProvider();
        Assert.Equal("bound", provider.GetRequiredService<IOptionsMonitor<SessionOptions>>().CurrentValue.RegionName);
        Assert.Equal(new[] { "before:" + new SessionOptions().RegionName, "after:bound", "post:bound" }, observed, StringComparer.Ordinal);
        observed.Clear();
        OptionsValidationException error = Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptionsMonitor<SessionOptions>>().Get("other"));
        Assert.Equal("region must be bound", Assert.Single(error.Failures));
        Assert.Empty(observed);
    }

    [Fact]
    public void AddLocalStack_Should_Keep_All_Defaults_When_Sections_Are_Missing()
    {
        var services = new ServiceCollection();
        services.AddLocalStack(Configuration(new Dictionary<string, string?>(StringComparer.Ordinal)));
        using ServiceProvider provider = services.BuildServiceProvider();
        LocalStackOptions parent = provider.GetRequiredService<IOptions<LocalStackOptions>>().Value;
        SessionOptions session = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
        ConfigOptions config = provider.GetRequiredService<IOptions<ConfigOptions>>().Value;
        Assert.False(parent.UseLocalStack);
        Assert.Equal(new SessionOptions().AwsAccessKeyId, session.AwsAccessKeyId);
        Assert.Equal(new SessionOptions().AwsAccessKey, session.AwsAccessKey);
        Assert.Equal(new SessionOptions().AwsSessionToken, session.AwsSessionToken);
        Assert.Equal(new SessionOptions().RegionName, session.RegionName);
        Assert.Equal(new ConfigOptions().LocalStackHost, config.LocalStackHost);
        Assert.Equal(new ConfigOptions().EdgePort, config.EdgePort);
        Assert.Equal(new ConfigOptions().UseSsl, config.UseSsl);
        Assert.Equal(new ConfigOptions().UseLegacyPorts, config.UseLegacyPorts);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AddLocalStack_Should_Create_Null_Nested_Options_Only_When_The_Section_Has_Children(bool hasChildren)
    {
        IConfigurationRoot configuration = Configuration(hasChildren
            ? new Dictionary<string, string?>(StringComparer.Ordinal) { ["LocalStack:Session:RegionName"] = "bound", ["LocalStack:Config:UseSsl"] = "true" }
            : new Dictionary<string, string?>(StringComparer.Ordinal));
        // Characterize the pinned general binder as well as the public registration seam.
        foreach (bool originalBinder in new[] { true, false })
        {
            var services = new ServiceCollection();
            services.Configure<LocalStackOptions>(options =>
            {
                SetPrivate(options, nameof(LocalStackOptions.Session), null);
                SetPrivate(options, nameof(LocalStackOptions.Config), null);
            });
            if (originalBinder)
            {
                services.Configure<LocalStackOptions>(options => configuration.GetSection("LocalStack").Bind(options, binding => binding.BindNonPublicProperties = true));
            }
            else
            {
                services.AddLocalStack(configuration);
            }

            using ServiceProvider provider = services.BuildServiceProvider();
            LocalStackOptions parent = provider.GetRequiredService<IOptions<LocalStackOptions>>().Value;
            if (hasChildren)
            {
                Assert.Equal("bound", parent.Session.RegionName);
                Assert.True(parent.Config.UseSsl);
            }
            else
            {
                Assert.Null(parent.Session);
                Assert.Null(parent.Config);
            }
        }
    }

    [Fact]
    public void AddLocalStack_Should_Not_Publish_A_Partially_Bound_Nested_Object_When_Binding_Fails()
    {
        IConfigurationRoot configuration = Configuration(new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["LocalStack:Config:UseSsl"] = "true",
            ["LocalStack:Config:EdgePort"] = "invalid",
        });
        foreach (bool originalBinder in new[] { true, false })
        {
            LocalStackOptions? captured = null;
            var services = new ServiceCollection();
            services.Configure<LocalStackOptions>(options =>
            {
                captured = options;
                SetPrivate(options, nameof(LocalStackOptions.Config), null);
            });
            if (originalBinder)
            {
                services.Configure<LocalStackOptions>(options => configuration.GetSection("LocalStack").Bind(options, binding => binding.BindNonPublicProperties = true));
            }
            else
            {
                services.AddLocalStack(configuration);
            }

            using ServiceProvider provider = services.BuildServiceProvider();
            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IOptions<LocalStackOptions>>().Value);
            Assert.NotNull(captured);
            Assert.Null(captured.Config);
        }
    }

    private static IConfigurationRoot Configuration(Dictionary<string, string?> values) => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private static void SetPrivate(object options, string property, object? value) => options.GetType().GetProperty(property)!.SetValue(options, value);

    private sealed class RegionValidator : IValidateOptions<SessionOptions>
    {
        public ValidateOptionsResult Validate(string? name, SessionOptions options) => options.RegionName == "bound"
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("region must be bound");
    }
}
