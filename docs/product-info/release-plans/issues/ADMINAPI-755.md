# ADMINAPI-755 - An error is thrown in the response instead of a warning message when registration is disabled

**Type**: Bug

**Status**: Done

## Description
**Affected versions**


* AdminAPI: 1\.x and 2\.x


**Step to Reproduce.**  

1\. Open the appsettings.json file from AdminAPI  

2\. Change the configuration 'AllowRegistration: false'  

3\. Save all changes and restart IIS  

4\. Go to '<https://localhost/adminapi/swagger>  

5\. Run the endpoint 'POST /connect/register'


**Actual Result.**


* An error is thrown in the response instead of a warning message


**Expected Result \& Acceptance Criteria** 


* Only for first\-time configuration
* A warning message should be return in the response instead of the error
* Try\-catch to capture the error
* As seen from Mozilla's HTTP Status Code article, "503 Service Unavailable" appears to be best to represent this server\-side error.
* Include message "This server is not available.  Please contact your Ed\-Fi system administrator for more information."


Unable to render embedded object: File (image\-2024\-04\-18\-08\-34\-58\-552\.png) not found.![](/rest/api/3/attachment/content/19347)



