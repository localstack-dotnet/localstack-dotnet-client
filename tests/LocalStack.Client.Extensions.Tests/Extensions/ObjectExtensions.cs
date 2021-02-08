using System.Text.Json;

namespace LocalStack.Client.Extensions.Tests.Extensions
{
    public static class ObjectExtensions
    {
        public static bool DeepEquals(this object obj1, object obj2)
        {
            string obj1Ser = JsonSerializer.Serialize(obj1);
            string obj2Ser = JsonSerializer.Serialize(obj2);

            return obj1Ser == obj2Ser;
        }
    }
}
