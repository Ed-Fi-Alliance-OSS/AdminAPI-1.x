# Test Pre-requisites

Many tests in this assembly make direct calls to the Azure Resource Manager API.  For these tests to succeed, a few pre-requisites must be satisfied:

1. An Azure account must be established
2. Within this account, a Cloud ODS instance must be deployed
    - The install friendly name should be set to "Cloud ODS Integration Tests" so that the tests will reference the correct resources.
      If you need to use a different name, ensure that the resource group name is set in your app.config
        ```xml
        <appSettings>
          ...
          <add key="resourceGroupName" value="your_resource_group_name" />
        </appSettings>
        ```

    - The installation needs only a production API website
    - Web hosting plans for the integration test sites can be set to "Free"
    - The AdminApp is not actually a requirement for the integration tests to run and therefore may be deleted after the deployment has completed.
    - SQL server authentication must be enabled when using a local sql server.  Ensure a "IntegrationTests" user account has been created and given the sysadmin role
    - Set the password in your app.config IntegrationTests connection string.
      Example:
        ```xml
          <connectionStrings>
            ...
            <add name="IntegrationTests" connectionString="Server=.;Database=master;User Id='IntegrationTests';Password='IntegrationTests_User_Password_Here'" />
          </connectionStrings>
        ```
    - Deploy script example:
        ```
        .\Deploy-EdFiOds.ps1 -InstallFriendlyName "Cloud ODS Integration Tests" -UseMyOwnSqlServer
         ```
3. Azure account configuration gathered in the previous steps must be made available to the unit test assembly.
    - For local development, settings may be placed in DeveloperSettings\AzureActiveDirectory.config.  
    **DO NOT CHECK THESE VALUES INTO SOURCE CONTROL**:
        ```xml
        <appSettings>
          <add key="ida:ClientId" value="YOURCLIENTID" />
          <add key="ida:TenantId" value="YOURTENANTID" />
          <add key="ida:ClientSecret" value="YOURCLIENTSECRET"/>
          <add key="ida:SubscriptionId" value="YOURSUBSCRIPTIONID" />
        </appSettings>
        ```
    - For CI servers, the above configuration items ("ida:ClientId", "ida:TenantId", etc.) can be provided as environment variables.
