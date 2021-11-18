global using System;
global using System.IO;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Linq;
global using System.Text;
global using System.Text.RegularExpressions;

global using Cake.Common;
global using Cake.Common.IO;
global using Cake.Common.IO.Paths;
global using Cake.Common.Tools.NuGet;
global using Cake.Common.Tools.NuGet.Install;
global using Cake.Common.Diagnostics;
global using Cake.Common.Tools.DotNetCore;
global using Cake.Common.Tools.DotNetCore.Build;
global using Cake.Common.Tools.DotNetCore.MSBuild;
global using Cake.Common.Tools.DotNetCore.Pack;
global using Cake.Common.Tools.DotNetCore.Test;
global using Cake.Common.Tools.NuGet.List;
global using Cake.Core;
global using Cake.Core.IO;
global using Cake.Frosting;

global using LocalStack.Build;
global using LocalStack.Build.Models;