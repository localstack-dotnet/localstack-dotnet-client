namespace LocalStack.Client.Tests.ConfigTests;

public class ConfigOptionsTests
{
    [Fact]
    public void ConfigOptions_Should_Created_With_Default_Parameters_If_It_Created_By_Default_Constructor()
    {
        var configOptions = new ConfigOptions();

        Assert.Equal(Constants.LocalStackHost, configOptions.LocalStackHost);
        Assert.Equal(Constants.UseSsl, configOptions.UseSsl);
        Assert.Equal(Constants.UseLegacyPorts, configOptions.UseLegacyPorts);
        Assert.Equal(Constants.EdgePort, configOptions.EdgePort);
    }

    [Fact]
    public void ConfigOptions_Should_Created_With_Default_Parameters_If_It_Created_By_Parameterized_Constructor_And_Non_Of_The_Parameters_Has_Set()
    {
        ConstructorInfo constructor = typeof(ConfigOptions).GetConstructor(new[] { typeof(string), typeof(bool), typeof(bool), typeof(int) });

        Assert.NotNull(constructor);

        var configOptions = (ConfigOptions)constructor.Invoke(new[] { Type.Missing, Type.Missing, Type.Missing, Type.Missing });

        Assert.Equal(Constants.LocalStackHost, configOptions.LocalStackHost);
        Assert.Equal(Constants.UseSsl, configOptions.UseSsl);
        Assert.Equal(Constants.UseLegacyPorts, configOptions.UseLegacyPorts);
        Assert.Equal(Constants.EdgePort, configOptions.EdgePort);
    }

    [Fact]
    public void LocalStackHost_Property_Of_ConfigOptions_Should_Equal_To_Given_LocalStackHost_Constructor_Parameter()
    {
        const string localStackHost = "myhost";

        var configOptions = new ConfigOptions(localStackHost);

        Assert.Equal(localStackHost, configOptions.LocalStackHost);
        Assert.Equal(Constants.UseSsl, configOptions.UseSsl);
        Assert.Equal(Constants.UseLegacyPorts, configOptions.UseLegacyPorts);
        Assert.Equal(Constants.EdgePort, configOptions.EdgePort);
    }

    [Fact]
    public void UseSsl_Property_Of_ConfigOptions_Should_Equal_To_Given_UseSsl_Constructor_Parameter()
    {
        const bool useSsl = true;

        var configOptions = new ConfigOptions(useSsl: useSsl);

        Assert.Equal(Constants.LocalStackHost, configOptions.LocalStackHost);
        Assert.Equal(useSsl, configOptions.UseSsl);
        Assert.Equal(Constants.UseLegacyPorts, configOptions.UseLegacyPorts);
        Assert.Equal(Constants.EdgePort, configOptions.EdgePort);
    }

    [Fact]
    public void UseLegacyPorts_Property_Of_ConfigOptions_Should_Equal_To_Given_UseLegacyPorts_Constructor_Parameter()
    {
        const bool useLegacyPorts = true;

        var configOptions = new ConfigOptions(useLegacyPorts: useLegacyPorts);

        Assert.Equal(Constants.LocalStackHost, configOptions.LocalStackHost);
        Assert.Equal(Constants.UseSsl, configOptions.UseSsl);
        Assert.Equal(useLegacyPorts, configOptions.UseLegacyPorts);
        Assert.Equal(Constants.EdgePort, configOptions.EdgePort);
    }

    [Fact]
    public void EdgePort_Property_Of_ConfigOptions_Should_Equal_To_Given_EdgePort_Constructor_Parameter()
    {
        const int edgePort = 4212;

        var configOptions = new ConfigOptions(edgePort: edgePort);

        Assert.Equal(Constants.LocalStackHost, configOptions.LocalStackHost);
        Assert.Equal(Constants.UseSsl, configOptions.UseSsl);
        Assert.Equal(Constants.UseLegacyPorts, configOptions.UseLegacyPorts);
        Assert.Equal(edgePort, configOptions.EdgePort);
    }
}
