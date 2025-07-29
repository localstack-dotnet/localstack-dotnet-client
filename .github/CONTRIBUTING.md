# Contributing to LocalStack .NET Client

🎉 **Thank you for your interest in contributing to LocalStack .NET Client!**

We welcome contributions of all kinds - from bug reports and feature requests to code improvements and documentation updates. This guide will help you get started and ensure your contributions have the best chance of being accepted.

## 📋 Quick Reference

- 🐛 **Found a bug?** → [Create an Issue](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/new)
- 💡 **Have an idea?** → [Start a Discussion](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions)
- ❓ **Need help?** → [Q&A Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/categories/q-a)
- 🚨 **Security issue?** → See our [Security Policy](.github/SECURITY.md)
- 🔧 **Ready to code?** → [Submit a Pull Request](https://github.com/localstack-dotnet/localstack-dotnet-client/compare)

## 🤝 Code of Conduct

This project follows the [.NET Foundation Code of Conduct](.github/CODE_OF_CONDUCT.md). By participating, you're expected to uphold this code. Please report unacceptable behavior to [localstack.dotnet@gmail.com](mailto:localstack.dotnet@gmail.com).

## 📝 Contributor License Agreement (CLA)

**Important**: As this project is pursuing .NET Foundation membership, contributors may be required to sign a Contributor License Agreement (CLA) as part of the contribution process. This helps ensure that contributions can be used by the project and the community. By submitting a pull request, you agree to license your contribution under the MIT license.

## 🎯 Version Strategy

We maintain a **dual-track versioning strategy**:

- **v2.x (AWS SDK v4)** - Active development on [`master`](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master) branch
- **v1.x (AWS SDK v3)** - Long-term support on [`sdkv3-lts`](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/sdkv3-lts) branch (maintained until July 2026)

When contributing, please specify which version track your contribution targets.

## 🚀 Getting Started

### Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (for development)
- [Docker](https://docs.docker.com/get-docker/) (for LocalStack testing)
- [Git](https://git-scm.com/downloads)
- IDE: [Visual Studio](https://visualstudio.microsoft.com/), [Rider](https://www.jetbrains.com/rider/), or [VS Code](https://code.visualstudio.com/)

### Development Environment Setup

1. **Fork and Clone**

   ```bash
   # Fork the repository on GitHub, then clone your fork
   git clone https://github.com/YOUR-USERNAME/localstack-dotnet-client.git
   cd localstack-dotnet-client
   
   # Add upstream remote
   git remote add upstream https://github.com/localstack-dotnet/localstack-dotnet-client.git
   ```

2. **Build the Project**

   ```bash
   # Windows
   .\build.ps1
   
   # Linux/macOS  
   ./build.sh
   ```

3. **Run Tests**

   ```bash
   # All tests (requires Docker for functional tests)
   .\build.ps1 --target tests
   
   # Unit/Integration tests only
   .\build.ps1 --target tests --skipFunctionalTest true
   ```

## 🐛 Reporting Issues

### Before Creating an Issue

1. **Search existing issues** to avoid duplicates
2. **Check [Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions)** - your question might already be answered
3. **Verify the issue** occurs with LocalStack (not real AWS services)
4. **Test with latest version** when possible

### Creating a Bug Report

Use our [Issue Template](https://github.com/localstack-dotnet/localstack-dotnet-client/issues/new) which will guide you through providing:

- **Environment details** (LocalStack version, .NET version, OS)
- **Minimal reproduction** case
- **Expected vs actual** behavior
- **Configuration** and error messages

## 💡 Suggesting Features

We love new ideas! Here's how to suggest features:

1. **Check [existing discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/categories/ideas)** for similar requests
2. **Start a [Discussion](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/new?category=ideas)** to gauge community interest
3. **Create an issue** if there's positive feedback and clear requirements

## 🔧 Contributing Code

### Before You Start

1. **Discuss significant changes** in [Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions) first
2. **Check for existing work** - someone might already be working on it
3. **Create an issue** if one doesn't exist (for tracking)

### Pull Request Process

1. **Create a feature branch**

   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/issue-number-description
   ```

2. **Make your changes**
   - Follow existing code style and conventions
   - Add tests for new functionality
   - Update documentation as needed
   - Ensure all analyzers pass without warnings

3. **Test thoroughly**

   ```bash
   # Run all tests
   ./build.sh --target tests
   
   # Test specific scenarios with LocalStack
   # (see sandbox projects for examples)
   ```

4. **Commit with conventional commits**

   ```bash
   git commit -m "feat: add support for XYZ service"
   git commit -m "fix: resolve timeout issue in DynamoDB client"
   git commit -m "docs: update installation guide"
   ```

5. **Submit the Pull Request**
   - Use our [PR Template](https://github.com/localstack-dotnet/localstack-dotnet-client/compare)
   - Provide clear description of changes
   - Link related issues
   - Specify target version track (v1.x or v2.x)

### Code Quality Standards

- ✅ **Follow existing patterns** and architectural decisions
- ✅ **Write comprehensive tests** (unit, integration, functional where applicable)
- ✅ **Add XML documentation** for public APIs
- ✅ **No analyzer warnings** - we treat warnings as errors
- ✅ **Maintain backward compatibility** (unless it's a breaking change PR)
- ✅ **Performance considerations** - avoid introducing regressions

### Testing Guidelines

We have multiple test types:

- **Unit Tests** - Fast, isolated, no external dependencies
- **Integration Tests** - Test AWS SDK integration and client creation
- **Functional Tests** - Full end-to-end with LocalStack containers

When adding tests:

- Place them in the appropriate test project
- Follow existing naming conventions
- Test both success and error scenarios
- Include tests for edge cases

## 📚 Documentation

- **Code comments** - Explain the "why", not the "what"
- **XML documentation** - Required for all public APIs
- **README updates** - For feature additions or breaking changes
- **CHANGELOG** - Add entries for user-facing changes

## 🔍 Review Process

1. **Automated checks** must pass (build, tests, code analysis)
2. **Maintainer review** - we aim to review within 48 hours
3. **Community feedback** - other contributors may provide input
4. **Iterative improvements** - address feedback promptly
5. **Final approval** and merge

## ❓ Getting Help

- **Questions about usage** → [Q&A Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/categories/q-a)
- **Ideas for features** → [Ideas Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/categories/ideas)
- **General discussion** → [General Discussions](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/categories/general)
- **Show your work** → [Show and Tell](https://github.com/localstack-dotnet/localstack-dotnet-client/discussions/categories/show-and-tell)

## 🎉 Recognition

Contributors are recognized in:

- Our [Contributors](https://github.com/localstack-dotnet/localstack-dotnet-client/graphs/contributors) page
- Release notes for significant contributions
- Project documentation for major features

---

**By contributing to this project, you agree to abide by our [Code of Conduct](.github/CODE_OF_CONDUCT.md) and understand that your contributions will be licensed under the MIT License.**

Thank you for making LocalStack .NET Client better! 🚀
