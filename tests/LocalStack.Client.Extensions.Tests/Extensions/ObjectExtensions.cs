#pragma warning disable CA1515 // Because an application's API isn't typically referenced from outside the assembly, types can be made internal

namespace LocalStack.Client.Extensions.Tests.Extensions;

public static class ObjectExtensions
{
    public static bool DeepEquals(this object obj1, object obj2)
    {
        string obj1Ser = JsonSerializer.Serialize(obj1);
        string obj2Ser = JsonSerializer.Serialize(obj2);

        return string.Equals(obj1Ser, obj2Ser, StringComparison.Ordinal);
    }
}