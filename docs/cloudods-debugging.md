# Cloud ODS Local Debugging

The Cloud ODS depends on several different Ed-Fi code repos within GitHub.

* [Ed-Fi-Alliance-OSS/Ed-Fi-ODS-AdminApp](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-AdminApp)
* [Ed-Fi-Alliance-OSS/Ed-Fi-ODS](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS)
* [Ed-Fi-Exchange-OSS/Ed-Fi-X-Ods-Deploy-Azure](https://github.com/Ed-Fi-Exchange-OSS/Ed-Fi-X-Ods-Deploy-Azure)
* [Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Implementation](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Implementation)

After cloning the above repositories locally, perform the following steps in order to
be able to debug the Cloud ODS locally:

1. [Install /
   Build](https://techdocs.ed-fi.org/display/ODSAPIS3V500/Getting+Started) the
   Ed-Fi ODS if you have not done so already
2. Deploy an instance Cloud ODS on Azure (see Readme.md in
   **Ed-Fi-Ods-Deploy-Azure** repo
3. Run the from
   **Ed-Fi-Ods-Deploy-Azure/Application/Published/ScaleDown-EdFiOds.ps1** script
   to minimize the costs of keeping the instance running. You can even delete
   the databases if the only purpose is to do local testing - that will get you
   to $0 per month
4. Edit and place the following code in
   **EdFi.Ods.AdminApp.Web/DeveloperSettings/AzureActiveDirectory.config**

    ```xml
    <appSettings>
      <add key="ida:ClientId" value="" />
      <add key="ida:TenantId" value="" />
      <add key="ida:SubscriptionId" value="" />
      <add key="ida:AADInstance" value="https://login.microsoftonline.com/" />
      <add key="ida:ClientSecret" value=""/>
    </appSettings>
    ```

    1. [TenantId](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Properties)
       (same as Directory ID) and [Subscription
       ID](https://portal.azure.com/#blade/Microsoft_Azure_Billing/SubscriptionsBlade)
       can be procured from the Azure portal
    2. ClientId is the “Application (client) ID” in [Azure Active Directory \
       App
       Registrations](https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade)
       for the Azure AD app registered when you deployed
    3. ClientSecret - In the App Registration screen for the AdminApp. Click on
       "Certificates & secrets" and add a New Client Secret. This key value is
       your ClientSecret
5. Create an account on your local SQL Server that matches the credentials you
   provided for the Azure SQL server during deployment
    1. This account must be added to the sysadmin fixed role
6. Edit **Ed-Fi-ODS-Implementation/Application/EdFi.Ods.Admin.Web/Web.config**
    1. Ensure the API code is configured to run in **SharedInstance** mode
    2. Ensure the EdFi_Ods connection string is pointing to a valid database
       name
7. [Run](https://techdocs.ed-fi.org/pages/viewpage.action?pageId=30638189) the
   Ed-Fi ODS API
8. Edit **Ed-Fi-ODS-AdminApp/Application/EdFi.Ods.AdminApp.Web\\Web.config**
    1. Ensure the “DefaultOdsInstance” value matches the “Install Friendly Name”
       you provided when the Cloud ODS was deployed
    2. Set "owin:AppStartup" to "Azure"
9. In Visual Studio with the AdminApp project open, start a Debugging Session
   and you should be able login via Azure Active Directory while the app itself
   runs locally  
