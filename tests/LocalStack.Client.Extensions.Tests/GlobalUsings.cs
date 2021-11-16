global using System;
global using System.Collections.Generic;
global using System.Text.Json;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;

global using Amazon.Extensions.NETCore.Setup;
global using Amazon.Runtime;

global using LocalStack.Client.Contracts;
global using LocalStack.Client.Extensions.Contracts;
global using LocalStack.Client.Extensions.Tests.Extensions;
global using LocalStack.Client.Options;
global using LocalStack.Client.Tests.Mocks.MockServiceClients;
global using LocalStack.Client.Utils;

global using Moq;
global using Xunit;