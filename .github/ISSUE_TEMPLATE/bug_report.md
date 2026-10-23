---
name: Bug report
about: Create a report to help us improve
title: ''
labels: bug
assignees: ''

---

## 🐛 Bug Description

**What happened?**
A clear and concise description of the bug.

**What did you expect to happen?**
A clear and concise description of what you expected to happen.

## 🔄 Steps to Reproduce

1.
2.
3.
4.

**Minimal code example:**

```csharp
// Please provide a minimal code example that reproduces the issue
```

## 📋 Environment Information

**Package Versions:**

- LocalStack.Client version:
- LocalStack.Client.Extensions version (if used):

Fixes target the supported 3.x line. See the [security policy](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/.github/SECURITY.md).

**LocalStack Information:**

- LocalStack Version:
- LocalStack image tag: (e.g., `localstack/localstack:4.14.0` or `localstack/localstack:2026.10.2`)
- LocalStack Services Used: (e.g., S3, DynamoDB, SNS)

**.NET Information:**

- .NET Version: (e.g., .NET 8, .NET Framework 4.7.2)
- Target framework (TFM): (e.g., `net10.0`, `net11.0`, `netstandard2.0`, `net472`)
- Native AOT used: (yes/no)
- Trimming used: (yes/no)
- Operating System: (e.g., Windows 11, Ubuntu 22.04, macOS 14)
- IDE/Editor: (e.g., Visual Studio 2022, Rider, VS Code)

**AWS SDK Information:**

- AWS SDK Version: (e.g., AWSSDK.S3 4.0.4.2)
- Services: (e.g., S3, DynamoDB, SNS)

## 🔍 Additional Context

**Configuration (Optional):**

```json
{
  "LocalStack": {
    "UseLocalStack": true
  }
}
```

**Error Messages/Stack Traces:**

```
Paste any error messages or stack traces here
```

**Screenshots:**
If applicable, add screenshots to help explain your problem.

**Additional Information:**
Add any other context about the problem here.

## ✅ Checklist

- [ ] I have searched existing issues to ensure this is not a duplicate
- [ ] I have provided all the requested information above
- [ ] I have tested this with the latest version of LocalStack.Client
- [ ] I have verified this issue occurs with LocalStack (not with real AWS services)
