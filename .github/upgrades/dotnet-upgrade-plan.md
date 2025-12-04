# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade `iFredApps.Lib\iFredApps.Lib.csproj`
4. Upgrade `iFredApps.TimeTracker\iFredApps.TimeTracker.Core\iFredApps.TimeTracker.Core.csproj`
5. Upgrade `iFredApps.TimeTracker\iFredApps.TimeTracker.Data\iFredApps.TimeTracker.Data.csproj`
6. Upgrade `iFredApps.TimeTracker\iFredApps.TimeTracker.SL\iFredApps.TimeTracker.SL.csproj`
7. Convert and upgrade `iFredApps.Lib.WPF\iFredApps.Lib.WPF.csproj` (convert to SDK-style and change target framework)
8. Upgrade `iFredApps.TimeTracker\iFredApps.TimeTracker.Tests\iFredApps.TimeTracker.Tests.csproj`
9. Upgrade `iFredApps.TimeTracker\iFredApps.TimeTracker.WebApi\iFredApps.TimeTracker.WebApi.csproj`
10. Upgrade `iFredApps.TimeTracker\iFredApps.TimeTracker.UI\iFredApps.TimeTracker.UI.csproj`

## Settings

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                                | Current Version                 | New Version   | Description                                   |
|:------------------------------------------------------------|:-------------------------------:|:-------------:|:----------------------------------------------|
| MahApps.Metro.IconPacks                                      | 5.0.0                           | 4.11.0        | Incompatible with .NET 10; recommended downgrade to 4.11.0 |
| Microsoft.AspNetCore.Authentication.JwtBearer                | 8.0.7                           | 10.0.0        | Recommended for .NET 10                        |
| Microsoft.EntityFrameworkCore                                | 8.0.7                           | 10.0.0        | Recommended for .NET 10                        |
| Microsoft.EntityFrameworkCore.Design                         | 8.0.7                           | 10.0.0        | Recommended for .NET 10                        |
| Microsoft.EntityFrameworkCore.InMemory                       | 8.0.10                          | 10.0.0        | Recommended for .NET 10 (test in-memory provider) |
| Microsoft.EntityFrameworkCore.Tools                          | 8.0.7                           | 10.0.0        | Recommended for .NET 10                        |
| Microsoft.Extensions.Options                                 | 9.0.0-preview.1.24080.9         | 10.0.0        | Replace preview package with stable 10.0.0     |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets        | 1.20.1                          |               | No supported version found for .NET 10 (incompatible) |
| Newtonsoft.Json                                               | 13.0.3                          | 13.0.4        | Security/compatibility update                  |
| System.Security.Cryptography.ProtectedData                   | 9.0.0                           | 10.0.0        | Recommended for .NET 10                        |

### Project upgrade details

#### iFredApps.Lib\iFredApps.Lib.csproj

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - `Newtonsoft.Json` update from `13.0.3` to `13.0.4`
  - `System.Security.Cryptography.ProtectedData` update from `9.0.0` to `10.0.0`

Other changes:
  - Ensure code references are compatible with .NET 10 APIs.

#### iFredApps.TimeTracker\iFredApps.TimeTracker.Core\iFredApps.TimeTracker.Core.csproj

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.Extensions.Options` update from `9.0.0-preview.1.24080.9` to `10.0.0`

Other changes:
  - Review any usages of preview APIs and replace them with stable equivalents.

#### iFredApps.TimeTracker\iFredApps.TimeTracker.Data\iFredApps.TimeTracker.Data.csproj

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.EntityFrameworkCore` update from `8.0.7` to `10.0.0`
  - `Microsoft.EntityFrameworkCore.Design` update from `8.0.7` to `10.0.0`
  - `Microsoft.EntityFrameworkCore.Tools` update from `8.0.7` to `10.0.0`

Other changes:
  - Validate EF Core breaking changes and update DbContext/EF usage if needed.

#### iFredApps.TimeTracker\iFredApps.TimeTracker.SL\iFredApps.TimeTracker.SL.csproj

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

Other changes:
  - No NuGet package changes detected specifically for this project in analysis.

#### iFredApps.Lib.WPF\iFredApps.Lib.WPF.csproj

Project properties changes:
  - Convert project file to SDK-style project.
  - Change target framework from `net48` (or `.NETFramework,Version=v4.7.2`) to `net10.0-windows` and set appropriate `UseWPF` and `TargetPlatform` if needed.

Other changes:
  - Ensure WPF-specific package and API compatibility for .NET 10 windows target. This project conversion may require manual edits to resource and assembly references.

#### iFredApps.TimeTracker\iFredApps.TimeTracker.Tests\iFredApps.TimeTracker.Tests.csproj

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.EntityFrameworkCore.InMemory` update from `8.0.10` to `10.0.0`

Other changes:
  - Update test SDK and test runner packages if required for .NET 10.

#### iFredApps.TimeTracker\iFredApps.TimeTracker.WebApi\iFredApps.TimeTracker.WebApi.csproj

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.AspNetCore.Authentication.JwtBearer` update from `8.0.7` to `10.0.0`
  - `Microsoft.EntityFrameworkCore.Design` update from `8.0.7` to `10.0.0`
  - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (version `1.20.1`) is incompatible with .NET 10 and no supported version was found. Consider removing or replacing this package/tooling.

Other changes:
  - Validate Web API hosting and container tooling compatibility.

#### iFredApps.TimeTracker\iFredApps.TimeTracker.UI\iFredApps.TimeTracker.UI.csproj

Project properties changes:
  - Target framework should be changed from `net8.0-windows7.0` to `net10.0-windows`

NuGet packages changes:
  - `MahApps.Metro.IconPacks` downgrade from `5.0.0` to `4.11.0` (incompatibility)
  - `Newtonsoft.Json` update from `13.0.3` to `13.0.4`

Other changes:
  - Verify UI libraries compatibility with new windows target.

