# Ed-Fi-ODS-AdminApp.Management.Azure.Tests

Contains Integration Tests for the Azure-specific components of the Ed-Fi Cloud ODS AdminApp.

Pre-Requisites
--------------

*   A functioning local copy of the [Ed-Fi Cloud ODS](../../README.md)
*   Visual Studio 2019 or greater
*   Latest Azure SDK

Running Integration Tests
-------------------------

Many tests in this assembly make direct calls to the Azure Resource Manager API. For these tests to succeed, a few pre-requisites must be satisfied:

1.  Deploy an instance Cloud ODS on Azure (see Readme.md in **Ed-Fi-Ods-Deploy-Azure** repo)
    1.  The install friendly name should be set to "Cloud ODS Integration Tests" so that the tests will reference the correct resources. 
    (Sample powershell command for deployment:  .\Deploy-EdFiOds.ps1 -ResourceGroupLocation "South Central US" -Version 5.0.0 -InstallFriendlyName "Cloud ODS Integration Tests" -Edition "release" -DeploySwaggerUI)
    If you need to use a different name, ensure that the _resourceGroupName_ setting is appropriately changed in your **EdFi.Ods.AdminApp.Management.Azure.IntegrationTests/appsettings.json** file (see below)

2.  Run the from **Ed-Fi-Ods-Deploy-Azure/Application/Published/ScaleDown-EdFiOds.ps1** script to minimize the costs of keeping the instance running. You can even delete the databases if the only purpose is to do local testing – that will get you to $0 per month

3.  Edit the following key/value pairs in **EdFi.Ods.AdminApp.Management.Azure.IntegrationTests/appsettings.json**

    ```json
      "IdaAADInstance": "https://login.microsoftonline.com/"
      "IdaClientId": ""
      "IdaClientSecret": ""
      "IdaTenantId": ""
      "IdaSubscriptionId": ""
    ```

    DO NOT CHECK THESE VALUES INTO SOURCE CONTROL

    1.  [IdaTenantId](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Properties) (same as Directory ID) and [IdaSubscriptionId](https://portal.azure.com/#blade/Microsoft_Azure_Billing/SubscriptionsBlade) can be procured from the Azure portal
    2.  IdaClientId is the "Application (client) ID" in [Azure Active Directory \ App Registrations](https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade) for the Azure AD app registered when you deployed
    3.  IdaClientSecret – In the App Registration screen for the AdminApp. Click on "Certificates & secrets" and add a New Client Secret. This key value is your IdaClientSecret
    4.  Note: For CI servers, the above configuration items can be provided as environment variables
