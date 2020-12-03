# Admin App Developer Instructions

## Development Pre-Requisites

* A functioning local copy of an [Ed-Fi
  ODS for Suite 3](https://techdocs.ed-fi.org/display/ETKB/Ed-Fi+Operational+Data+Store+and+API)
  * Currently supported versions: 3.4, 5.0.0
* Visual Studio 2019 or greater
* Latest Azure SDK
* Support for file names longer than 256 characters:
  1. Start the registry editor (regedit.exe)
  2. Navigate to `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem`
  3. Double click LongPathsEnabled
  4. Set to 1 and click OK

## Build Script

The PowerShell script `build.ps1` in the root directory contains several options
to assist in running standard processes from the command line. This script
assumes that Visual Studio 2019 or newer is installed, providing MSBuild 15 or
MSBuild 16. Other dependencies tools are downloaded as needed (nuget, nunit).

Available commands:

* `.\build.ps1 clean` runs the MSBuild `clean` task
* `.\build.ps1 build` runs the MSBuild `build` task with several implicit steps,
  including NuGet restore and temporary injection of version numbers into the
  AssemblyInfo.cs files.
* `.\build.ps1 unittest` executes NUnit tests in projects named `*.UnitTest`,
  which do not connect to a database.
* `.\build.ps1 integrationtest` executes NUnit tests in projects named `*.Test`,
  which connect to a database (does not include the Azure integration test
  project).
* `.\build.ps1 buildandtest` executes the Build, UnitTest, and IntegrationTest
  commands.
* `.\build.ps1 package` builds pre-release and release NuGet packages for the
  Admin App web application.
* `.\build.ps1 push` uploads a NuGet package to the NuGet feed.

Review the parameters at the top of `build.ps1` for additional command line
arguments.

## TeamCity Automation

See [.teamcity/readme.md](../.teamcity/readme.md) for information on how to use
the TeamCity build configurations.
