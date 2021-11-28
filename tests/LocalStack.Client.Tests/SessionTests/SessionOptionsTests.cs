namespace LocalStack.Client.Tests.SessionTests;

public class SessionOptionsTests
{
    [Fact]
    public void SessionOptions_Should_Created_With_Default_Parameters_If_It_Created_By_Default_Constructor()
    {
        var sessionOptions = new SessionOptions();

        Assert.Equal(Constants.AwsAccessKeyId, sessionOptions.AwsAccessKeyId);
        Assert.Equal(Constants.AwsAccessKey, sessionOptions.AwsAccessKey);
        Assert.Equal(Constants.AwsSessionToken, sessionOptions.AwsSessionToken);
        Assert.Equal(Constants.RegionName, sessionOptions.RegionName);
    }

    [Fact]
    public void SessionOptions_Should_Created_With_Default_Parameters_If_It_Created_By_Parameterized_Constructor_And_Non_Of_The_Parameters_Has_Set()
    {
        ConstructorInfo constructor = typeof(SessionOptions).GetConstructor(new[] { typeof(string), typeof(string), typeof(string), typeof(string) });

        Assert.NotNull(constructor);

        var sessionOptions = (SessionOptions)constructor.Invoke(new[] { Type.Missing, Type.Missing, Type.Missing, Type.Missing });

        Assert.Equal(Constants.AwsAccessKeyId, sessionOptions.AwsAccessKeyId);
        Assert.Equal(Constants.AwsAccessKey, sessionOptions.AwsAccessKey);
        Assert.Equal(Constants.AwsSessionToken, sessionOptions.AwsSessionToken);
        Assert.Equal(Constants.RegionName, sessionOptions.RegionName);
    }

    [Fact]
    public void AwsAccessKeyId_Property_Of_ConfigOptions_Should_Equal_To_Given_AwsAccessKeyId_Constructor_Parameter()
    {
        const string awsAccessKeyId = "myAwsAccessKeyId";

        var sessionOptions = new SessionOptions(awsAccessKeyId);

        Assert.Equal(awsAccessKeyId, sessionOptions.AwsAccessKeyId);
        Assert.Equal(Constants.AwsAccessKey, sessionOptions.AwsAccessKey);
        Assert.Equal(Constants.AwsSessionToken, sessionOptions.AwsSessionToken);
        Assert.Equal(Constants.RegionName, sessionOptions.RegionName);
    }

    [Fact]
    public void AwsAccessKey_Property_Of_ConfigOptions_Should_Equal_To_Given_AwsAccessKey_Constructor_Parameter()
    {
        const string awsAccessKey = "myAwsAccessKey";

        var sessionOptions = new SessionOptions(awsAccessKey: awsAccessKey);

        Assert.Equal(Constants.AwsAccessKeyId, sessionOptions.AwsAccessKeyId);
        Assert.Equal(awsAccessKey, sessionOptions.AwsAccessKey);
        Assert.Equal(Constants.AwsSessionToken, sessionOptions.AwsSessionToken);
        Assert.Equal(Constants.RegionName, sessionOptions.RegionName);
    }

    [Fact]
    public void AwsSessionToken_Property_Of_ConfigOptions_Should_Equal_To_Given_AwsSessionToken_Constructor_Parameter()
    {
        const string awsSessionToken = "myAwsSessionToken";

        var sessionOptions = new SessionOptions(awsSessionToken: awsSessionToken);

        Assert.Equal(Constants.AwsAccessKeyId, sessionOptions.AwsAccessKeyId);
        Assert.Equal(Constants.AwsAccessKey, sessionOptions.AwsAccessKey);
        Assert.Equal(awsSessionToken, sessionOptions.AwsSessionToken);
        Assert.Equal(Constants.RegionName, sessionOptions.RegionName);
    }

    [Fact]
    public void RegionName_Property_Of_ConfigOptions_Should_Equal_To_Given_RegionName_Constructor_Parameter()
    {
        const string regionName = "myRegionName";

        var sessionOptions = new SessionOptions(regionName: regionName);

        Assert.Equal(Constants.AwsAccessKeyId, sessionOptions.AwsAccessKeyId);
        Assert.Equal(Constants.AwsAccessKey, sessionOptions.AwsAccessKey);
        Assert.Equal(Constants.AwsSessionToken, sessionOptions.AwsSessionToken);
        Assert.Equal(regionName, sessionOptions.RegionName);
    }
}
