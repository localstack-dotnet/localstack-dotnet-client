global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.Reflection;
global using System.Reflection.Emit;
global using System.Text.Json;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;

global using Amazon;
global using Amazon.Extensions.NETCore.Setup;
global using Amazon.Runtime;
global using Amazon.S3;

global using LocalStack.Client.Contracts;
global using LocalStack.Client.Extensions.Contracts;
global using LocalStack.Client.Extensions.Exceptions;
global using LocalStack.Client.Extensions.Tests.Extensions;
global using LocalStack.Client.Options;
global using LocalStack.Client.Models;
global using LocalStack.Tests.Common.Mocks.MockServiceClients;

global using Moq;
global using Xunit;
