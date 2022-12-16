# AdminApp.Management Legacy Support

Historically, Admin App has supported backwards or cross-compatibility between versions of the ODS/API. The release of ODS/API 6.0 includes a breaking change in the security model found in `EdFi.Security.DataAccess`. In order to continue support for older versions of the ODS/API, the Admin App library code (`EdFi.Ods.AdminApp.Management`) must in turn reference both versions of `EdFi.Security.DataAccess`.

## Referencing Both Assemblies

The current `EdFi.Security.DataAccess` assembly is referenced using NuGet, as typical for dependencies. 
The legacy `EdFi.SecurityCompatiblity53.DataAccess` assembly is also referenced using NuGet. 
In order to serve the `EdFi.SecurityCompatiblity53.DataAccess.dll` as a NuGet package, we created a new packaging pipeline. Given the stability of the previous release and one-off nature of this package, we copied the "legacy" library code into a new repository: [Ed-Fi-Compatibility-Libraries](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-Compatibility-Libraries). We avoid _code_ collisions using different namespaces. 

### "Renaming" Namespaces for EdFi.Security.DataAccess 5.3.43

In order to avoid collisions with the current EdFi.Security.DataAccess, we needed to rename the project and namespaces within. The copied "legacy" library code with the changed namespaces resides in a new repository: [Ed-Fi-Compatibility-Libraries](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-Compatibility-Libraries). We use a github action workflow to pack and publish the library to Azure Artifacts. The `PackageId`, `AssemblyName` and `RootNamespace` have been changed or hard-set before publishing. The package is versioned as the original. The library's code is the same as `v5.3-patch2` / `v5.3.43`.

#### EdFi.SecurityCompatiblity53.DataAccess.csproj

```xml
  <PropertyGroup>
    <PackageId>EdFi.SecurityCompatiblity53.DataAccess</PackageId>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AssemblyName>EdFi.SecurityCompatiblity53.DataAccess</AssemblyName>
    <RootNamespace>EdFi.SecurityCompatiblity53.DataAccess</RootNamespace>
    <RestorePackages>true</RestorePackages>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Copyright>Copyright © 2020 Ed-Fi Alliance, LLC and Contributors</Copyright>
    <AllowedOutputExtensionsInPackageBuildOutputFolder>$(AllowedOutputExtensionsInPackageBuildOutputFolder);.pdb</AllowedOutputExtensionsInPackageBuildOutputFolder>
  </PropertyGroup>
```

#### Directory.Build.props

```xml
<Project>
    <PropertyGroup>
        <Version>5.3.43</Version>
        <FileVersion>5.3.43</FileVersion>
        <InformationalVersion>5.3</InformationalVersion>
    </PropertyGroup>
</Project>
```

### References in csproj

Referencing both versions of the library in a csproj is similar to typical scenarios of using different NuGet packages

The latest package can be added and updated as normal
```xml
<PackageReference Include="EdFi.Suite3.Security.DataAccess" Version="A.B.C"/>
```
The legacy package can be added with the fixed version
```xml
<PackageReference Include="EdFi.SecurityCompatibility53.DataAccess" Version="5.3.43"/>
```

## Using Both Namespaces In Code

We are able to avoid namespace collisions in C# using fully qualified namespaces.

```csharp
using System;
using EdFi.SecurityCompatibility53.DataAccess.Models;

public class ClaimSetService
{
    public void Execute()
    {
        var claimSet = new ClaimSet();
    }
}
```

It is possible to use both the libraries in the same file, but as with name overlap in other situations, ambiguous references must be fully qualified for clarity:

```csharp
using EdFi.SecurityCompatibility53.DataAccess.Models;
using EdFi.Security.DataAccess.Models;

using ClaimSet53 =  EdFi.SecurityCompatibility53.DataAccess.Models.ClaimSet;

public class ClaimSetService
{
    public void Execute()
    {
        var invalid = new ClaimSet(); //this will not compile, since both namespaces contain a "ClaimSet" type

        //instead we must use the fully qualified name
        var valid1 = new EdFi.Security.DataAccess.Models.ClaimSet();
        var valid2 = new EdFi.SecurityCompatibility53.DataAccess.Models.ClaimSet();

        //type aliases are also valid, but also specify which extern alias it is from (see above)
        var valid3 = new ClaimSet53();
    }
}
```

## Avoiding Dealing with different using directives 

If at all possible, we want to minimize the need to write code like the above, which uses multiple types or methods from both versions in tandem. There are a few ways we can do this:

- limit `EdFi.Security.DataAccess` references to within the `EdFi.Ods.AdminApp.Management` project
  - (with the exception of registering `ISecurityContext` with service providers)
- use models from `EdFi.Ods.AdminApp.Management` rather than entities for parameters or return types
- avoid "chaining" `Command` or `Query` types as dependencies
  - pragmatism in applying "Don't Repeat Yourself" will go a long way

These three principals should help isolate interactions with the `Security` database and subsequently the need to reference `EdFi.Security.DataAccess`.
Once obscured, the publicly available services need only determine which versioned service to call, without care for what models are being used "under the hood:"

```csharp
namespace EdFi.Ods.AdminApp.Management.ClaimSets;

public class ClaimSetService
{
    private readonly IOdsSecurityModelVersionResolver _resolver;
    private readonly ClaimSetOldService _oldService; //references EdFi.SecurityCompatibility53.DataAccess
    private readonly ClaimSetNewService _newService; //references EdFi.Security.DataAccess

    public OverrideDefaultAuthorizationStrategyCommand() { } //initialize dependencies...

    public ClaimSetResult Execute(IClaimSetRequest request)
    {
        return _resolver.GetModelVersion() switch
        {
            EdFiOdsSecurityModelVersion.Old => _oldService.Execute(request),
            EdFiOdsSecurityModelVersion.New => _newService.Execute(request),
            _ => throw new NotImplementedException("Compatibility for version not supported"),
       };
    }
}

//use types which can be used by either version of the service, and by any outside consumer
public interface IClaimSetRequest { }
public class ClaimSetResult { }
```
