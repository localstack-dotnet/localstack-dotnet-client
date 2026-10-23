# Native AOT support preserves existing consumer code

Native AOT support preserves existing consumer calls, including `AddLocalStack(configuration)`, `AddAwsService<IAmazonS3>()`, and Core `Session` APIs. The generic interface path uses AWS SDK v4 static factories; a slim build-time generator supplies direct operations for the `Type` APIs, and a guarded reflection fallback preserves JIT behavior for shapes the compiler cannot see. This keeps upgrades source-compatible while removing runtime reflection from LocalStack.NET's qualified Native AOT paths.

Rooting the existing reflection with generated code was AOT-compatible, but retained reflection and imposed heavy consumer build cost. Marking the `Type` APIs AOT-unsupported would break the goal that existing calls work. Removing the fallback would break existing JIT consumers such as F#/VB projects using `Type` APIs and runtime-loaded clients, so trimming/Native AOT removes it while JIT retains it.

The [platform boundary](../architecture/platform-and-packaging.md#native-aot-and-trimming) owns exact shape/compiler limits and the Native AOT test lane that qualifies unchanged calls by publishing and running them; the [implementation](../../src/LocalStack.Client/Session.cs) and [generator](../../src/LocalStack.Client.Generators/AwsRegistryGenerator.cs) define the two paths.
