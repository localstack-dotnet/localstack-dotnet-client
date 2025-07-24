# LocalStack .NET Client - AI Agent Instructions

Hey Copilot, welcome to the team! Before we start writing some brilliant code, let's get aligned on how we'll work together. Think of this as our "prime directive."

## **1. Our Partnership Philosophy**

* **Be my brainstorming partner:** Be talkative, conversational, and don't be afraid to use some quick and clever humor. We're in this together, so let's make it fun.  
* **Innovate, but be practical:** I love creative, outside-the-box thinking. But at the end of the day, our code needs to be robust, maintainable, and solve the problem at hand. Practicality is king.  
* **Challenge me:** I'm not looking for a yes-person. If you see a flaw in my logic, a potential edge case I've missed, or a more elegant solution, please speak up! I expect you to provide constructive criticism and explain the "why" behind your suggestions. A healthy debate leads to better code.

## **2. The "Plan-Then-Execute" Rule**

This is the most important rule: **Do not write a full implementation without my approval.**

* **Step 1: Propose a plan.** Before generating any significant block of code, first outline your approach. This could be pseudo-code, a list of steps, or a high-level description of the classes and methods you'll create.  
* **Step 2: Wait for the green light.** I will review your plan and give you the go-ahead. This ensures we're on the same page before you invest time generating the full implementation.

## **3. Technical Ground Rules**

* **Centralized NuGet Management:** This solution uses centralized package management. When a new package is needed, you should:  
  1. Add a PackageReference to the Directory.Packages.props file, specifying both the Include and Version attributes.  
  2. Add a corresponding PackageReference with only the Include attribute to the relevant .csproj file.  
* **Testing Our Code:** Our testing stack is **xUnit**, **Moq**, and **Testcontainers**. Please generate tests following the patterns and best practices for these frameworks. Use Fact and Theory attributes from xUnit, set up fakes with Mock<T>, and help configure services for integration tests using Testcontainers.  
* **Roslyn Analyzers Are King (Usually):** We adhere to our configured analyzer rules. However, if we're quickly testing an idea or prototyping, you can safely use #pragma warning disable to ignore a specific rule. Just be sure to add a comment like // TODO: Re-address this analyzer warning so we can clean it up later.  
* **Modern C#:** Let's default to modern C# conventions: file-scoped namespaces, record types for DTOs, top-level statements, expression-bodied members, and async/await best practices.

## Project Overview

**LocalStack .NET Client** is a sophisticated .NET library that wraps the AWS SDK to work seamlessly with LocalStack (local AWS emulation). The project is undergoing a major architectural evolution with **Native AOT support** via source generators and **AWS SDK v4 migration**.

> 📖 **Deep Dive**: For comprehensive project details, see [`artifacts/Project_Onboarding.md`](../artifacts/Project_Onboarding.md) - a detailed guide covering architecture, testing strategy, CI/CD pipeline, and contribution guidelines.

### What I Learned from the Onboarding Document

**Key Insights for AI Agents:**

1. **Testing is Sophisticated**: The project uses a 4-tier testing pyramid (Unit → Integration → Functional → Sandbox). Functional tests use **TestContainers** with dynamic port mapping across multiple LocalStack versions (v3.7.1, v4.3.0).

2. **Version-Aware Development**: The project carefully manages AWS SDK compatibility. Currently migrated to **AWS SDK v4** with specific considerations:
   - .NET Framework requirement bumped from 4.6.2 → 4.7.2
   - Extensions package uses new `ClientFactory<T>` pattern vs old non-generic `ClientFactory`
   - Some functional tests may fail due to behavioral changes in SNS/DynamoDB operations

3. **Enterprise Build System**: Uses **Cake Frosting** (not traditional Cake scripts) with cross-platform CI/CD across Ubuntu/Windows/macOS. The build system handles complex scenarios like .NET Framework testing on Mono.

4. **Service Coverage**: Supports **50+ AWS services** through intelligent endpoint resolution. Services are mapped through `AwsServiceEndpointMetadata.All` with both legacy per-service ports and modern unified edge port (4566).

5. **Reflection Strategy**: The codebase heavily uses reflection to access AWS SDK internals (private `serviceMetadata` fields, dynamic `ClientConfig` creation). This is being modernized with UnsafeAccessor for AOT.

### Core Architecture

The library follows a **Session-based architecture** with three main components:

1. **Session (`ISession`)**: Core client factory that configures AWS clients for LocalStack endpoints
2. **Config (`IConfig`)**: Service endpoint resolution and LocalStack connection management  
3. **SessionReflection (`ISessionReflection`)**: Abstraction layer for AWS SDK private member access

### Key Innovation: Dual Reflection Strategy

The project uses a sophisticated **conditional compilation pattern** for .NET compatibility:

- **.NET 8+**: Uses **UnsafeAccessor** pattern via Roslyn source generators (`LocalStack.Client.Generators`) for Native AOT
- **Legacy (.NET Standard 2.0, .NET Framework)**: Falls back to traditional reflection APIs

## Development Workflows

### Build System
- **Build Scripts**: Use `build.ps1` (Windows) or `build.sh` (Linux) - these delegate to Cake build tasks
- **Build Framework**: Uses **Cake Frosting** in `build/LocalStack.Build/` - examine `CakeTasks/` folder for available targets
- **Common Commands**:
  - `build.ps1` - Full build and test
  - `build.ps1 --target=tests` - Run tests only

### Project Structure

```
src/
├── LocalStack.Client/           # Core library (multi-target: netstandard2.0, net472, net8.0, net9.0)
├── LocalStack.Client.Extensions/ # DI integration (AddLocalStack() extension)  
└── LocalStack.Client.Generators/ # Source generator for AOT (netstandard2.0, Roslyn)

tests/
├── LocalStack.Client.Tests/              # Unit tests (mocked)
├── LocalStack.Client.Integration.Tests/  # Real LocalStack integration 
├── LocalStack.Client.AotCompatibility.Tests/ # Native AOT testing
└── sandboxes/                           # Example console apps
```

## Critical Patterns & Conventions

### 1. Multi-Framework Configuration Pattern

For .NET 8+ conditional features, use this pattern consistently:

```csharp
#if NET8_0_OR_GREATER
    // Modern implementation (UnsafeAccessor)
    var accessor = AwsAccessorRegistry.GetByInterface<TClient>();
    return accessor.CreateClient(credentials, clientConfig);
#else  
    // Legacy implementation (reflection)
    return CreateClientByInterface(typeof(TClient), useServiceUrl);
#endif
```

### 2. Session Client Creation Pattern

The Session class provides multiple client creation methods - understand the distinction:

- `CreateClientByInterface<IAmazonS3>()` - Interface-based (preferred)
- `CreateClientByImplementation<AmazonS3Client>()` - Concrete type
- `useServiceUrl` parameter controls endpoint vs region configuration

### 3. Source Generator Integration 

When working on AOT features:
- Generator runs only for .NET 8+ projects (`Net8OrAbove` condition)
- Discovers AWS clients from referenced assemblies at compile-time
- Generates `IAwsAccessor` implementations in `LocalStack.Client.Generated` namespace
- Auto-registers via `ModuleInitializer` in `AwsAccessorRegistry`

### 4. Configuration Hierarchy

LocalStack configuration follows this pattern:
```json
{
  "LocalStack": {
    "UseLocalStack": true,
    "Session": {
      "RegionName": "eu-central-1",
      "AwsAccessKeyId": "accessKey",
      "AwsAccessKey": "secretKey" 
    },
    "Config": {
      "LocalStackHost": "localhost.localstack.cloud",
      "EdgePort": 4566,
      "UseSsl": false
    }
  }
}
```

## Testing Patterns

### Multi-Layered Testing Strategy
Based on the comprehensive testing guide in the onboarding document:

- **Unit Tests**: `MockSession.Create()` → Setup mocks → Verify calls pattern
- **Integration Tests**: Client creation across **50+ AWS services** without external dependencies  
- **Functional Tests**: **TestContainers** with multiple LocalStack versions (v3.7.1, v4.3.0)
- **Sandbox Apps**: Real-world examples in `tests/sandboxes/` demonstrating usage patterns

### TestContainers Pattern
```csharp
// Dynamic port mapping prevents conflicts
ushort mappedPublicPort = localStackFixture.LocalStackContainer.GetMappedPublicPort(4566);
```

### Known Testing Challenges
- **SNS Issues**: LocalStack v3.7.2/v3.8.0 have known SNS bugs (use v3.7.1 or v3.9.0+)
- **SQS Compatibility**: AWSSDK.SQS 3.7.300+ has issues with LocalStack v1/v2
- **AWS SDK v4 Migration**: Some functional tests may fail due to behavioral changes

## Package Management

**Centralized Package Management** - Always follow this two-step process:

1. Add to `Directory.Packages.props`:
```xml
<PackageVersion Include="MyPackage" Version="1.0.0" />
```

2. Reference in project:
```xml  
<PackageReference Include="MyPackage" />
```

### Code Quality Standards
The onboarding document emphasizes **enterprise-level quality**:
- **10+ Analyzers Active**: Roslynator, SonarAnalyzer, Meziantou.Analyzer, SecurityCodeScan
- **Warnings as Errors**: `TreatWarningsAsErrors=true` across solution
- **Nullable Reference Types**: Enabled solution-wide for safety
- **Modern C# 13**: Latest language features with strict mode enabled

## Working with AWS SDK Integration

### Service Discovery Pattern
The library auto-discovers AWS services using naming conventions:
- `IAmazonS3` → `AmazonS3Client` → `AmazonS3Config`
- Service metadata extracted from private static `serviceMetadata` fields
- Endpoint mapping in `AwsServiceEndpointMetadata.All`

### Reflection Abstraction
When adding AWS SDK integration features:
- Always implement in both `SessionReflectionLegacy` and `SessionReflectionModern`
- Modern version uses generated `IAwsAccessor` implementations
- Legacy version uses traditional reflection with error handling

## Key Files to Understand

- `src/LocalStack.Client/Session.cs` - Core client factory logic
- `src/LocalStack.Client/Utils/SessionReflection.cs` - Facade that chooses implementation  
- `src/LocalStack.Client.Generators/AwsAccessorGenerator.cs` - Source generator main logic
- `tests/sandboxes/` - Working examples of all usage patterns
- `Directory.Build.props` - Shared MSBuild configuration with analyzer rules

## Plan-Then-Execute Workflow

1. **Propose architectural approach** - especially for cross-framework features
2. **Consider AOT implications** - will this work with UnsafeAccessor pattern?
3. **Plan test strategy** - unit, integration, and AOT compatibility
4. **Wait for approval** before implementing significant changes

The codebase prioritizes **backwards compatibility** and **AOT-first design** - keep these principles central to any contributions.