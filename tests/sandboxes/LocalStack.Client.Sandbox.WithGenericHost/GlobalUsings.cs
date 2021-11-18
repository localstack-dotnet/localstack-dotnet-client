global using System;
global using System.IO;
global using System.Reflection;
global using System.Threading;
global using System.Threading.Tasks;

global using Amazon.S3;
global using Amazon.S3.Model;
global using Amazon.S3.Transfer;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
 
global using LocalStack.Client.Extensions;
global using LocalStack.Client.Sandbox.WithGenericHost;
