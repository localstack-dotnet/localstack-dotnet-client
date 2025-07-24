global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Reflection;
global using System.Linq;

#if NETSTANDARD || NET472
global using LocalStack.Client.Utils;
#endif

global using Amazon;
global using Amazon.Runtime;

#if NETSTANDARD || NET472
global using Amazon.Runtime.Internal;
#endif

global using LocalStack.Client.Enums;
global using LocalStack.Client.Exceptions;
global using LocalStack.Client.Models;
global using LocalStack.Client.Options;
global using LocalStack.Tests.Common.Mocks;
global using LocalStack.Tests.Common.Mocks.MockServiceClients;

global using Moq;

global using Xunit;