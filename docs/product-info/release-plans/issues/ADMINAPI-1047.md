# ADMINAPI-1047 - When you create an Application with Admin API it will not show in Admin App

**Type**: Bug

**Status**: Done

## Description
Admin App has a requirement that Application.OdsInstance\_OdsInstanceId in Admin DB be populated to show an application within Admin App


![image-20240827-201115.png](/rest/api/3/attachment/content/21306)


![image-20240827-201151.png](/rest/api/3/attachment/content/21307)


Admin API Application POST does not allow you to include an instance id, meaning that any applications created will not show up in Admin App.



