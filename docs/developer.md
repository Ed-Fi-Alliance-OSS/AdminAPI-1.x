# Admin App Developer Instructions

## Contents

- [Admin App Developer Instructions](#admin-app-developer-instructions)
  - [Contents](#contents)
  - [Development Pre-Requisites](#development-pre-requisites)
  - [Build Script](#build-script)
  - [TeamCity Automation](#teamcity-automation)
  - [Running on Localhost](#running-on-localhost)
    - [Shared Instance](#shared-instance)
    - [Year-Specific Mode](#year-specific-mode)
    - [District-Specific Mode](#district-specific-mode)
  - [Running in Docker](#running-in-docker)
    - [Running Only the API in Docker](#running-only-the-api-in-docker)
    - [Running a Local Build in Docker](#running-a-local-build-in-docker)

For debugging on Azure, see [CloudODS Debugging](cloudods-debugging.md)

## Development Pre-Requisites

* [.NET Core 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
* Either:
  * [Visual Studio 2022](https://visualstudio.microsoft.com/downloads), or
  * [Visual Studio 2022 Build
    Tools](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022)
    (install the ".NET Build Tools" component)
* Clone [this
  repository](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-AdminApp) locally
* To work with the official Ed-Fi Docker solution, also clone the [Docker
  repository](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Docker).

:warning: some situations may require changing Windows to handle file names
longer than 256 characters:

  1. Start the registry editor (regedit.exe)
  2. Navigate to
     `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem`
  3. Double click LongPathsEnabled
  4. Set to 1 and click OK

## Build Script

The PowerShell script `build.ps1` in the root directory contains functions for
running standard build operations at the command line . This script assumes that
.NET 6.0 SDK or newer is installed. Other dependencies tools are downloaded
as needed (nuget, nunit).

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
* `.\build.ps1 run` launches the Admin App from the build script. The LaunchProfile
  parameter is required for running Admin App. Valid values include 'mssql-district'
  , 'mssql-shared', 'mssql-year', 'pg-district', 'pg-shared' and 'pg-year'

Review the parameters at the top of `build.ps1` for additional command line
arguments.

## TeamCity Automation

See [.teamcity/readme.md](../.teamcity/readme.md) for information on how to use
the TeamCity build configurations.

## Running on Localhost

To run the ODS/API locally (not in Docker), follow the Getting Started
instructions for the desired version of the [Operational Data Store (ODS) and
API](https://techdocs.ed-fi.org/display/ETKB/Ed-Fi+Operational+Data+Store+and+API).
_NOTE_: the current Admin App code supports ODS/API for Tech Suite 3, version
3.4, 5.0.1, and 5.1.0

Initially, the ODS/API will be set to run in Sandbox Mode, in which case Admin
App is not needed. Admin App supports the three "production" operation modes:
Shared Instance, Year-Specific, and District-Specific. The instructions below
will aid in running in each of these three modes.

Admin App works with both SQL Server and PostgreSQL. The non-Docker notes
describe working with SQL Server, whereas the [Docker
instructions](#running-on-docker) use PostgreSQL.

### Shared Instance

1. Get started with the ODS/API until the point where you are ready to have the
   ODS/API running in Visual Studio.
2. Change the ODS/API `web.config` (v3.x) or `appsettings.json` (v5.x) to run in
   "SharedInstance" mode and ensure that the ODS connection string points to
   database `EdFi_{0}`. At runtime, the software will substitute "Ods" for
   the replacement token `{0}`.
3. Install the AdminApp database support by running the following command in a
   PowerShell window:

   ```powershell
   # From AdminApp clone directory
   cd eng
   .\run-dbup-migrations.ps1
   ```

   :warning: you may wish to review the default configuration at the top of this
   script to ensure that it is appropriate for your situation.

4. Run the build script and exercise tests to verify your setup:

    ```powershell
    .\build.ps1 buildandtest
    ```

5. Run Admin App from Visual Studio, choosing either the "Shared Instance (SQL
   Server)" profile (uses IIS Express) or "mssql-shared" profile (runs the Kestrel
   built-in web server).

To reset the databases so that you start from a clean slate, re-run `initdev`
and return to step 3 above.

### Year-Specific Mode

1. Stop the ODS/API and/or Admin App if running in Visual Studio.
2. Change the ODS/API `web.config` (v3.x) or `appsettings.json` (v5.x) to run in
   "YearSpecific" mode and ensure that the ODS connection string points to
   database `EdFi_{0}`. At runtime, the software will substitute "Ods_yyyy" for
   the replacement token `{0}`, where "yyyy" is the four digit year of a given
   year instance database.
3. Re-run `initdev` with the following command, changing the years as desired:

   ```powershell
   initdev -InstallType YearSpecific -OdsTokens '2020;2021'
   ```

4. Install the AdminApp database support by running the following command in a
   PowerShell window:

   ```powershell
   # From AdminApp clone directory
   cd eng
   .\run-dbup-migrations.ps1
   ```

   :warning: you may wish to review the default configuration at the top of this
   script to ensure that it is appropriate for your situation.

5. Run the build script and exercise tests to verify your setup:

    ```powershell
    .\build.ps1 buildandtest
    ```

6. Run Admin App from Visual Studio, choosing either the "Year Specific (SQL
   Server)" profile (uses IIS Express) or "mssql-year" profile (runs the Kestrel
   built-in web server).

To reset the databases so that you start from a clean slate, return to step 3
above.

### District-Specific Mode

1. Stop the ODS/API and/or Admin App if running in Visual Studio.
2. Change the ODS/API `web.config` (v3.x) or `appsettings.json` (v5.x) to run in
   "DistrictSpecific" mode and ensure that the ODS connection string points to
   database `EdFi_{0}`. At runtime, the software will substitute "Ods_ddddd" for
   the replacement token `{0}`, where "ddddd" is the numeric identifier of a given
   district instance database.

3. Re-run `initdev` with the following command:

   ```powershell
   initdev -InstallType DistrictSpecific -OdsTokens '255901;255902;255903;255904;255905'
   ```

4. Install the AdminApp database support by running the following command in a
   PowerShell window:

   ```powershell
   # From AdminApp clone directory
   cd eng
   .\run-dbup-migrations.ps1
   ```

   :warning: you may wish to review the default configuration at the top of this
   script to ensure that it is appropriate for your situation.

5. Run the build script and exercise tests to verify your setup:

    ```powershell
    .\build.ps1 buildandtest
    ```

6. Run Admin App from Visual Studio, choosing either the "District Specific (SQL
   Server)" profile (uses IIS Express) or "mssql-district" profile (runs the
   Kestrel built-in web server).

To reset the databases so that you start from a clean slate, return to step 3
above.

## Running in Docker

### Running Only the API in Docker

These instructions are for running AdminApp _from Visual Studio_, connecting to
the ODS/API running in Docker:

* Follow the [README
  instructions](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Docker/blob/main/README.md)
  for configuring and running the ODS/API in Docker, except you should run the
  "local debug" compose file instead of the default one, so that you are not
  starting Admin App itself inside of Docker.

  ```powershell
  # From the Docker clone directory
  docker-compose -f compose-shared-instance-for-local-debug.yml up -d
  ```

* Generate a 256 bit AES encryption key and paste it into the
  `appSettings.Docker-SharedInstance.json` file. :warning: _do not commit your
  modified file into source control_. You can generate a key using a PowerShell
  script in the `eng` directory:

  ```powershell
  # From AdminApp clone directory
  Import-Module ./eng/key-management.psm1
  New-AESKey
  ```

* In the connection strings for this same file, adjust the default Host, Port,
  Username, and Password as required for your Docker configuration.
* Run Admin App, selecting the "Shared Instance (Docker-Postgres)" profile.

### Running a Local Build in Docker

Whereas the instructions above allow you to run Admin App from Visual Studio,
pointing to the ODS/API in Docker, the instructions below are for _injecting_
the locally-built Admin App into a Docker container so that the freshly-compiled
DLLs are what runs inside of Docker.

* Follow the [README
  instructions](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Docker/blob/main/README.md)
  for configuring and running the ODS/API in Docker. This time, you will want to
  run Docker compose using `compose-shared-instance-env-build.yml`:

  ```bash
  docker-compose -f compose-shared-instance-env-build.yml up -d
  ```

* Open a PowerShell prompt and change to the AdminApp clone directory.
* Review and edit, if necessary, the parameters in file
  `BuildDockerDevelopment.ps1`.
* Run following command for quick build and deploy/ copy over the latest files
  to an existing docker container:
  
  ```powershell
  # From AdminApp clone directory
  .\BuildDockerDevelopment.ps1
  ```
  