using System.ComponentModel;
using System.Linq;

namespace LocalStack.Client.Extensions;

// This is the fixed LocalStack schema, not a replacement for the general configuration binder.
// Keep evaluation inside Configure callbacks so the framework owns order, validation and caching.
internal static class LocalStackOptionsBinder
{
    public static void Bind(IConfiguration section, LocalStackOptions options)
    {
        options.ApplyConfiguration(ReadBoolean(section.GetSection(nameof(LocalStackOptions.UseLocalStack))));
        IConfigurationSection sessionSection = section.GetSection(nameof(LocalStackOptions.Session));
        SessionOptions? session = options.Session;
        if (session == null && sessionSection.GetChildren().Any())
        {
            session = new SessionOptions();
        }

        if (session != null)
        {
            Bind(sessionSection, session);
            options.ApplyConfiguration(session: session);
        }

        IConfigurationSection configSection = section.GetSection(nameof(LocalStackOptions.Config));
        ConfigOptions? config = options.Config;
        if (config == null && configSection.GetChildren().Any())
        {
            config = new ConfigOptions();
        }

        if (config != null)
        {
            Bind(configSection, config);
            options.ApplyConfiguration(config: config);
        }
    }

    public static void Bind(IConfiguration section, SessionOptions options)
    {
        options.ApplyConfiguration(awsAccessKeyId: section.GetSection(nameof(SessionOptions.AwsAccessKeyId)).Value);
        options.ApplyConfiguration(awsAccessKey: section.GetSection(nameof(SessionOptions.AwsAccessKey)).Value);
        options.ApplyConfiguration(awsSessionToken: section.GetSection(nameof(SessionOptions.AwsSessionToken)).Value);
        options.ApplyConfiguration(regionName: section.GetSection(nameof(SessionOptions.RegionName)).Value);
    }

    public static void Bind(IConfiguration section, ConfigOptions options)
    {
        options.ApplyConfiguration(localStackHost: section.GetSection(nameof(ConfigOptions.LocalStackHost)).Value);
        options.ApplyConfiguration(useSsl: ReadBoolean(section.GetSection(nameof(ConfigOptions.UseSsl))));
        options.ApplyConfiguration(useLegacyPorts: ReadBoolean(section.GetSection(nameof(ConfigOptions.UseLegacyPorts))));
        options.ApplyConfiguration(edgePort: ReadInt32(section.GetSection(nameof(ConfigOptions.EdgePort))));
    }

    private static bool? ReadBoolean(IConfigurationSection section)
    {
        string? value = section.Value;
        if (value == null)
        {
            return null;
        }

        try
        {
            return (bool)new BooleanConverter().ConvertFromInvariantString(value)!;
        }
        catch (Exception exception)
        {
            throw ConversionError(section.Path, typeof(bool), exception);
        }
    }

    private static int? ReadInt32(IConfigurationSection section)
    {
        string? value = section.Value;
        if (value == null)
        {
            return null;
        }

        try
        {
            // Int32Converter also accepts 0x, &h and # hexadecimal forms, unlike int.Parse.
            return (int)new Int32Converter().ConvertFromInvariantString(value)!;
        }
        catch (Exception exception)
        {
            // The desktop converter throws System.Exception; restrict the catch to conversion,
            // not configuration access or option mutation, just as ConfigurationBinder does.
            throw ConversionError(section.Path, typeof(int), exception);
        }
    }

    private static InvalidOperationException ConversionError(string path, Type type, Exception exception) =>
        new($"Failed to convert configuration value at '{path}' to type '{type}'.", exception);
}
