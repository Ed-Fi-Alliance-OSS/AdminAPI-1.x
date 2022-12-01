# AdminApp.Management Legacy Support

Historically, Admin App has supported backwards or cross-compatibility between versions of the ODS/API. The release of ODS/API 6.0 includes a breaking change in the security model found in `EdFi.Security.DataAccess`. In order to continue support for older versions of the ODS/API, the Admin App library code (`EdFi.Ods.AdminApp.Management`) must in turn reference both versions of `EdFi.Security.DataAccess`.

## Referencing Both Assemblies

The current `EdFi.Security.DataAccess` assembly is referenced using NuGet, as typical for dependencies. The legacy assembly is referenced by file directly, since NuGet does not support referencing multiple versions of the same package. We can avoid _code_ collisions using aliases (see below), but the assemblies still collide since they have the same "simple name" in their assembly metadata.

### "Renaming" EdFi.Security.DataAccess 5.3.43

To get around this, we have created a "one-time" compilation which has a different name in the assembly metadata. The changes can be found in a [side-branch on GitHub](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS/tree/AdminApp-Compatibility-Compile) but are copied below for reference. The `AssemblyName`, `ProductName`, and `Versions` have been changed or hard-set before publishing. The library's code is the same as `v5.3-patch2` / `v5.3.43` but we've advanced the assembly version to differentiate this package. The compiled .dll can be found in [/edfi.suite3.security.dataaccess/5.3.43/lib/netstandard2.1](./edfi.suite3.security.dataaccess/5.3.43/lib/netstandard2.1).

```xml
<PropertyGroup>
    <PackageId>EdFi.Security.DataAccess</PackageId>
    <AssemblyName>EdFi.Security.DataAccess53</AssemblyName>
    <RootNamespace>EdFi.Security.DataAccess</RootNamespace>
    <TargetFramework>netstandard2.1</TargetFramework>
    <RestorePackages>true</RestorePackages>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Copyright>Copyright © 2020 Ed-Fi Alliance, LLC and Contributors</Copyright>
    <AllowedOutputExtensionsInPackageBuildOutputFolder>$(AllowedOutputExtensionsInPackageBuildOutputFolder);.pdb</AllowedOutputExtensionsInPackageBuildOutputFolder>
    <Product>EdFi.Security.DataAccess53</Product>
    <AssemblyVersion>5.3.43.1</AssemblyVersion>
    <FileVersion>5.3.43</FileVersion>
  </PropertyGroup>
```

### References in csproj

Referencing both versions of the library in a csproj is similar to typical scenarios, with the addition of an `Alias` directive.

For NuGet, the package can be added and updated as normal, with the `Alias` stuck on to the end. (Tooling may remove this on upgrade, but the project will fail to compile until it is re-added.) This method should be used for the latest release of the library, as **only one version per package** can be referenced this way.

```xml
<PackageReference Include="EdFi.Suite3.Security.DataAccess" Version="A.B.C" Aliases="SecurityDataAccessNew" />
```

Previous "compatibility" versions should be referenced directly by (`.dll`) file. Aliases are added by additional `XML` elements. Unlike NuGet references, any number of aliased references can exist, so long as library metadata differs (see issue above.)

```xml
<Reference Include="EdFi.Security.DataAccessOld">
  <HintPath>LegacySupport\edfi.suite3.security.dataaccess\X.Y.Z\lib\netstandard2.1\EdFi.SecurityDataAccessXYZ</HintPath>
  <Aliases>SecurityDataAccessOld</Aliases>
</Reference>
```

## Using Both Namespaces In Code

We are able to avoid namespace collisions in C# using [extern aliases](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/extern-alias).

Extern aliases are listed _before_ `using` statements in a C# code file. Namespaces from that reference can be imported when prefixed with the alias like so: `AliasName::Namespace.Nest`. All types and other symbols are imported as normal:

```csharp
extern alias SecurityDataAccessOld;

using System;
using SecurityDataAccess53::EdFi.Security.DataAccess.Models;

public class ClaimSetService
{
    public void Execute()
    {
        var claimSet = new ClaimSet();
    }
}
```

It is possible to use multiple extern aliases in the same file, but as with name overlap in other situations, ambiguous references must be qualified for clarity:

```csharp
extern alias SecurityDataAccessOld;
extern alias SecurityDataAccessNew;

using SecurityDataAccessOld::EdFi.Security.DataAccess.Models;
using SecurityDataAccessNew::EdFi.Security.DataAccess.Models;

using ClaimSet53 = SecurityDataAccessOld::EdFi.Security.DataAccess.Models.ClaimSet;

public class ClaimSetService
{
    public void Execute()
    {
        var invalid = new ClaimSet(); //this will not compile, since both namespaces contain a "ClaimSet" type

        //instead we must use the fully qualified name
        var valid1 = new SecurityDataAccessOld::EdFi.Security.DataAccess.Models.ClaimSet();
        var valid2 = new SecurityDataAccessNew::EdFi.Security.DataAccess.Models.ClaimSet();

        //type aliases are also valid, but also specify which extern alias it is from (see above)
        var valid3 = new ClaimSet53();
    }
}
```

## Avoiding Dealing with Extern Aliases

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
    private readonly ClaimSetOldService _oldService; //references extern alias SecurityDataAccessOld
    private readonly ClaimSetNewService _newService; //references extern alias SecurityDataAccessNew

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
