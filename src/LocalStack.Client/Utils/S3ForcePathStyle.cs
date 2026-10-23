#if NET10_0_OR_GREATER
using System.Runtime.CompilerServices;

namespace LocalStack.Client.Utils;

internal static class S3ForcePathStyle
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_ForcePathStyle")]
    private static extern void SetForcePathStyle([UnsafeAccessorType("Amazon.S3.AmazonS3Config, AWSSDK.S3")] object config, bool value);

    internal static bool TrySet(ClientConfig clientConfig, bool value)
    {
        if (string.Equals(clientConfig.ServiceId, AwsServiceEndpointMetadata.S3.ServiceId, StringComparison.Ordinal))
        {
            SetForcePathStyle(clientConfig, value);
            return true;
        }

        return false;
    }
}
#endif
