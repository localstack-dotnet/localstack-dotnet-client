global using System;
global using System.Collections.Generic;
global using System.Dynamic;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Threading.Tasks;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Amazon;
global using Amazon.DynamoDBv2;
global using Amazon.DynamoDBv2.DataModel;
global using Amazon.DynamoDBv2.DocumentModel;
global using Amazon.DynamoDBv2.Model;
global using Amazon.S3;
global using Amazon.S3.Model;
global using Amazon.S3.Transfer;
global using Amazon.SQS;
global using Amazon.SQS.Model;
global using Amazon.SimpleNotificationService;
global using Amazon.SimpleNotificationService.Model;

global using AutoFixture;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Converters;

global using LocalStack.Client.Extensions;
global using LocalStack.Client.Enums;
global using LocalStack.Client.Contracts;
global using LocalStack.Client.Extensions.Tests.Extensions;
global using LocalStack.Client.Functional.Tests.Fixtures;
global using LocalStack.Client.Functional.Tests.Scenarios.DynamoDb.Entities;
global using LocalStack.Client.Functional.Tests.Scenarios.SQS.Models;
global using LocalStack.Client.Functional.Tests.Scenarios.SNS.Models;

global using Testcontainers.LocalStack;

global using Xunit;


#if NETCOREAPP
namespace System.Runtime.CompilerServices
{
    using System.ComponentModel;
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}
#endif