global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.Serialization;

global using Amazon;
global using Amazon.Runtime;
global using Amazon.Runtime.Internal;

global using LocalStack.Client.Contracts;
global using LocalStack.Client.Enums;
global using LocalStack.Client.Exceptions;
global using LocalStack.Client.Models;
global using LocalStack.Client.Options;
global using LocalStack.Client.Utils;

#pragma warning disable MA0048 // File name must match type name
#if NETSTANDARD || NET472
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