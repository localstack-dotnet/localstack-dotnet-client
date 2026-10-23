namespace LocalStack.Client.Utils;

internal static class ReflectionFallback
{
#if NET10_0_OR_GREATER
    [FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))]
#endif
    internal static bool IsSupported { get; } =
        !AppContext.TryGetSwitch("LocalStack.Client.ReflectionFallback.IsSupported", out bool enabled) || enabled;
}
