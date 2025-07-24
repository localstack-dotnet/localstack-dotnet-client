global using System;
global using System.Reflection;
global using System.Runtime.Serialization;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;

global using Amazon.Extensions.NETCore.Setup;
global using Amazon.Runtime;

global using LocalStack.Client.Contracts;
global using LocalStack.Client.Extensions.Contracts;
global using LocalStack.Client.Options;
global using LocalStack.Client.Extensions.Exceptions;

#if NETSTANDARD || NET472
global using LocalStack.Client.Utils;
#endif