global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Text.Json;
global using System.Threading.Tasks;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Amazon.DynamoDBv2;
global using Amazon.DynamoDBv2.DataModel;
global using Amazon.DynamoDBv2.DocumentModel;
global using Amazon.DynamoDBv2.Model;
global using Amazon.S3;
global using Amazon.S3.Model;
global using Amazon.S3.Transfer;
global using Amazon.SQS;
global using Amazon.SQS.Model;

global using AutoFixture;

global using DotNet.Testcontainers.Containers.Builders;
global using DotNet.Testcontainers.Containers.Modules;

global using LocalStack.Client.Extensions;
global using LocalStack.Client.Extensions.Tests.Extensions;
global using LocalStack.Client.Functional.Tests.Fixtures;
global using LocalStack.Client.Functional.Tests.Scenarios.DynamoDb.Entities;
global using LocalStack.Client.Functional.Tests.Scenarios.SQS.Models;

global using Xunit;
