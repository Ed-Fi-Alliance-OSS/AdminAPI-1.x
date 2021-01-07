# Admin App Developer Instructions

## Development Pre-Requisites

* A functioning local copy of an [Ed-Fi ODS for Suite
  3](https://techdocs.ed-fi.org/display/ETKB/Ed-Fi+Operational+Data+Store+and+API)
  * Currently supported versions: 3.4, 5.0.0
* Visual Studio 2019 or greater
* Latest Azure SDK
* Support for file names longer than 256 characters:
  1. Start the registry editor (regedit.exe)
  2. Navigate to
     `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem`
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

## Local Debugging with the ODS/API

### From Source Code, using SQL Server

1. Get started with the ODS/API until the point where you are ready to have the
   ODS/API running in Visual Studio.
2. Change the ODS/API web.config to run in SharedInstance mode and change the
   ODS connection string to point to the `EdFi_Ods_Populated_Template` database.
3. Install the AdminApp database support by running the following command in a
   PowerShell window:

   ```powershell
   # From AdminApp clone directory
   .\eng\run-dbup-migrations.ps1
   ```

4. Adjust AdminApp's `appsettings.SqlServer-SharedInstance.json` by setting the
   `ProductionOds` connection string to point to the
   `EdFi_Ods_Populated_Template` database.\
   :warning: _Make sure you don't check in these changes._
5. Run Admin App from Visual Studio.

#### Resetting from Scratch

Instructions for resetting your ODS/API to its initial state in order to test
the Admin App first time setup process:

1. If the API is running in Visual Studio, stop it.
2. Reset the Ed-Fi databases:
   1. Either re-run `initdev`, or
   2. Run the following commands for a quicker reset:

      ```powershell
      # From Ed-Fi-ODS-Implementation clone directory
      # Only need to run the following line if this is a new PowerShell session
      # .\Initialize-PowershellForDevelopment.ps1 
      Reset-AdminDatabase
      Reset-SecurityDatabase
      Reset-PopulatedTemplate
      ```

3. Re-run `run-dbup-migrations.ps1` in the AdminApp clone directory to
   re-install the Admin App database support.
4. Re-start the API.
5. Start debugging AdminApp.

### Docker

These instructions are for running AdminApp from Visual Studio, connecting to
the ODS/API running in Docker:

* Generate a 256 bit AES encryption key and paste it into the
  `appSettings.Docker-SharedInstance.json` file. :warning: _do not commit your
  modified file into source control_. You can generate a key using a PowerShell
  script in the `eng` directory:

  ```powershell
  # From AdminApp clone directory
  Import-Module ./eng/key-management.psm1
  New-AESKey
  ```

* In the connection strings, adjust the default Host, Port, Username, and
  Password as required for your Docker configuration.
* In the Docker files directory, create an appropriate `.env` file.
* Start the ODS/API in Docker using the Docker Compose "headless" file:

  ```powershell
  # From the Docker clone directory
  docker-compose -f compose-shared-instance-for-local-debug.yml up -d
  ```

### Admin app on Docker

These instructions are for updating the latest files from local, to an existing AdminApp container
for quick local development testing:

* Specify all the required appsettings values on BuildDockerDevelopment.ps1 on AdminApp clone directory
* Run following command for quick build and deploy/ copy over the latest files to an existing docker container
  
  ```powershell
  # From AdminApp clone directory
  .\BuildDockerDevelopment.ps1
  ```
  