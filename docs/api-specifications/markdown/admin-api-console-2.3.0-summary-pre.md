<!-- Generator: Widdershins v4.0.1 -->

<h1 id="admin-api-documentation">Admin API Documentation vadminconsole</h1>

> Scroll down for code samples, example requests and responses. Select a language for code samples from the tabs above or the mobile navigation menu.

The Ed-Fi Admin API is a REST API-based administrative interface for managing vendors, applications, client credentials, and authorization rules for accessing an Ed-Fi API.

# Authentication

- oAuth2 authentication. 

    - Flow: clientCredentials

    - Token URL = [https://localhost/connect/token](https://localhost/connect/token)

|Scope|Scope Description|
|---|---|
|edfi_admin_api/full_access|Unrestricted access to all Admin API endpoints|

<h1 id="admin-api-documentation-information">Information</h1>

## Retrieve API informational metadata

`GET /`

> Example responses

> 200 Response

```json
{
  "version": "string",
  "build": "string"
}
```

<h3 id="retrieve-api-informational-metadata-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[informationResult](#schemainformationresult)|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|[informationResult](#schemainformationresult)|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-adminconsole">Adminconsole</h1>

## get__adminconsole_userprofile

`GET /adminconsole/userprofile`

<h3 id="get__adminconsole_userprofile-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## post__adminconsole_userprofile

`POST /adminconsole/userprofile`

> Body parameter

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}
```

<h3 id="post__adminconsole_userprofile-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[addUserProfileRequest](#schemaadduserprofilerequest)|true|none|

<h3 id="post__adminconsole_userprofile-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|FeatureConstants.BadRequestResponseDescription|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_userprofile_{tenantId}_{id}

`GET /adminconsole/userprofile/{tenantId}/{id}`

<h3 id="get__adminconsole_userprofile_{tenantid}_{id}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "tenantId": "string",
  "firstName": "string",
  "lastName": "string",
  "userName": "string",
  "email": "string",
  "preferences": [
    {
      "code": "string",
      "value": "string"
    }
  ],
  "extensions": [
    {
      "code": "string",
      "data": "string"
    }
  ],
  "tenants": [
    {
      "createdBy": "string",
      "createdDateTime": "2019-08-24T14:15:22Z",
      "domains": [
        "string"
      ],
      "isDemo": true,
      "isIdentityProviders": [
        "string"
      ],
      "lastModifiedBy": "string",
      "lastModifiedDateTime": "2019-08-24T14:15:22Z",
      "organizationIdentifier": "string",
      "organizationName": "string",
      "state": "string",
      "subscriptions": [
        null
      ],
      "subscriptionsMigrated": true,
      "tenantId": "string",
      "tenantStatus": "string",
      "tenantType": "string"
    }
  ],
  "selectedTenant": {
    "createdBy": "string",
    "createdDateTime": "2019-08-24T14:15:22Z",
    "domains": [
      "string"
    ],
    "isDemo": true,
    "isIdentityProviders": [
      "string"
    ],
    "lastModifiedBy": "string",
    "lastModifiedDateTime": "2019-08-24T14:15:22Z",
    "organizationIdentifier": "string",
    "organizationName": "string",
    "state": "string",
    "subscriptions": [
      null
    ],
    "subscriptionsMigrated": true,
    "tenantId": "string",
    "tenantStatus": "string",
    "tenantType": "string"
  },
  "tenantsTotalCount": 0
}
```

<h3 id="get__adminconsole_userprofile_{tenantid}_{id}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[userProfileModel](#schemauserprofilemodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_userprofile_{tenantId}

`GET /adminconsole/userprofile/{tenantId}`

<h3 id="get__adminconsole_userprofile_{tenantid}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "tenantId": "string",
  "firstName": "string",
  "lastName": "string",
  "userName": "string",
  "email": "string",
  "preferences": [
    {
      "code": "string",
      "value": "string"
    }
  ],
  "extensions": [
    {
      "code": "string",
      "data": "string"
    }
  ],
  "tenants": [
    {
      "createdBy": "string",
      "createdDateTime": "2019-08-24T14:15:22Z",
      "domains": [
        "string"
      ],
      "isDemo": true,
      "isIdentityProviders": [
        "string"
      ],
      "lastModifiedBy": "string",
      "lastModifiedDateTime": "2019-08-24T14:15:22Z",
      "organizationIdentifier": "string",
      "organizationName": "string",
      "state": "string",
      "subscriptions": [
        null
      ],
      "subscriptionsMigrated": true,
      "tenantId": "string",
      "tenantStatus": "string",
      "tenantType": "string"
    }
  ],
  "selectedTenant": {
    "createdBy": "string",
    "createdDateTime": "2019-08-24T14:15:22Z",
    "domains": [
      "string"
    ],
    "isDemo": true,
    "isIdentityProviders": [
      "string"
    ],
    "lastModifiedBy": "string",
    "lastModifiedDateTime": "2019-08-24T14:15:22Z",
    "organizationIdentifier": "string",
    "organizationName": "string",
    "state": "string",
    "subscriptions": [
      null
    ],
    "subscriptionsMigrated": true,
    "tenantId": "string",
    "tenantStatus": "string",
    "tenantType": "string"
  },
  "tenantsTotalCount": 0
}
```

<h3 id="get__adminconsole_userprofile_{tenantid}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[userProfileModel](#schemauserprofilemodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_tenants

`GET /adminconsole/tenants`

<h3 id="get__adminconsole_tenants-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## post__adminconsole_tenants

`POST /adminconsole/tenants`

> Body parameter

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}
```

<h3 id="post__adminconsole_tenants-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[addTenantRequest](#schemaaddtenantrequest)|true|none|

<h3 id="post__adminconsole_tenants-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|FeatureConstants.BadRequestResponseDescription|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_tenants_{tenantId}

`GET /adminconsole/tenants/{tenantId}`

<h3 id="get__adminconsole_tenants_{tenantid}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|

<h3 id="get__adminconsole_tenants_{tenantid}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_steps

`GET /adminconsole/steps`

<h3 id="get__adminconsole_steps-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## post__adminconsole_steps

`POST /adminconsole/steps`

> Body parameter

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}
```

<h3 id="post__adminconsole_steps-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[addStepRequest](#schemaaddsteprequest)|true|none|

<h3 id="post__adminconsole_steps-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|FeatureConstants.BadRequestResponseDescription|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_steps_{tenantId}_{id}

`GET /adminconsole/steps/{tenantId}/{id}`

<h3 id="get__adminconsole_steps_{tenantid}_{id}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "number": 0,
  "description": "string",
  "startedAt": "2019-08-24T14:15:22Z",
  "completedAt": "2019-08-24T14:15:22Z",
  "status": "string"
}
```

<h3 id="get__adminconsole_steps_{tenantid}_{id}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[stepModel](#schemastepmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_steps_{tenantId}

`GET /adminconsole/steps/{tenantId}`

<h3 id="get__adminconsole_steps_{tenantid}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "number": 0,
  "description": "string",
  "startedAt": "2019-08-24T14:15:22Z",
  "completedAt": "2019-08-24T14:15:22Z",
  "status": "string"
}
```

<h3 id="get__adminconsole_steps_{tenantid}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[stepModel](#schemastepmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_permissions

`GET /adminconsole/permissions`

<h3 id="get__adminconsole_permissions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## post__adminconsole_permissions

`POST /adminconsole/permissions`

> Body parameter

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}
```

<h3 id="post__adminconsole_permissions-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[addPermissionRequest](#schemaaddpermissionrequest)|true|none|

<h3 id="post__adminconsole_permissions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|FeatureConstants.BadRequestResponseDescription|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_permissions_{tenantId}_{id}

`GET /adminconsole/permissions/{tenantId}/{id}`

<h3 id="get__adminconsole_permissions_{tenantid}_{id}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "docId": 0,
  "instanceId": 0,
  "tenantId": 0,
  "edOrgId": 0,
  "document": "string"
}
```

<h3 id="get__adminconsole_permissions_{tenantid}_{id}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[permissionModel](#schemapermissionmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_permissions_{tenantId}

`GET /adminconsole/permissions/{tenantId}`

<h3 id="get__adminconsole_permissions_{tenantid}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "docId": 0,
  "instanceId": 0,
  "tenantId": 0,
  "edOrgId": 0,
  "document": "string"
}
```

<h3 id="get__adminconsole_permissions_{tenantid}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[permissionModel](#schemapermissionmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_instances

`GET /adminconsole/instances`

<h3 id="get__adminconsole_instances-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## post__adminconsole_instances

`POST /adminconsole/instances`

> Body parameter

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}
```

<h3 id="post__adminconsole_instances-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[addInstanceRequest](#schemaaddinstancerequest)|true|none|

<h3 id="post__adminconsole_instances-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|FeatureConstants.BadRequestResponseDescription|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_instances_{tenantId}_{id}

`GET /adminconsole/instances/{tenantId}/{id}`

<h3 id="get__adminconsole_instances_{tenantid}_{id}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "docId": 0,
  "instanceId": 0,
  "tenantId": 0,
  "edOrgId": 0,
  "document": "string"
}
```

<h3 id="get__adminconsole_instances_{tenantid}_{id}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[instanceModel](#schemainstancemodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_instances_{tenantId}

`GET /adminconsole/instances/{tenantId}`

<h3 id="get__adminconsole_instances_{tenantid}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "docId": 0,
  "instanceId": 0,
  "tenantId": 0,
  "edOrgId": 0,
  "document": "string"
}
```

<h3 id="get__adminconsole_instances_{tenantid}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|[instanceModel](#schemainstancemodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_healthcheck

`GET /adminconsole/healthcheck`

<h3 id="get__adminconsole_healthcheck-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## post__adminconsole_healthcheck

`POST /adminconsole/healthcheck`

> Body parameter

```json
{
  "docId": 0,
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}
```

<h3 id="post__adminconsole_healthcheck-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[addHealthCheckRequest](#schemaaddhealthcheckrequest)|true|none|

<h3 id="post__adminconsole_healthcheck-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|FeatureConstants.BadRequestResponseDescription|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## get__adminconsole_healthcheck_{tenantId}

`GET /adminconsole/healthcheck/{tenantId}`

<h3 id="get__adminconsole_healthcheck_{tenantid}-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|tenantId|path|integer(int32)|true|none|

<h3 id="get__adminconsole_healthcheck_{tenantid}-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|FeatureConstants.InternalServerErrorResponseDescription|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-connect">Connect</h1>

## Registers new client

`POST /connect/register`

Registers new client

> Body parameter

```yaml
ClientId: string
ClientSecret: string
DisplayName: string

```

<h3 id="registers-new-client-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|object|false|none|
|» ClientId|body|string|false|Client id|
|» ClientSecret|body|string|false|Client secret|
|» DisplayName|body|string|false|Client display name|

<h3 id="registers-new-client-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Application registered successfully.|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves bearer token

`POST /connect/token`

To authenticate Swagger requests, execute using "Authorize" above, not "Try It Out" here.

> Body parameter

```yaml
client_id: null
client_secret: null
grant_type: null
scope: string

```

<h3 id="retrieves-bearer-token-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|object|false|none|
|» client_id|body|string |false|none|
|» client_secret|body|string |false|none|
|» grant_type|body|string |false|none|
|» scope|body|string|false|none|

<h3 id="retrieves-bearer-token-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Sign-in successful.|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

# Schemas

<h2 id="tocS_addHealthCheckRequest">addHealthCheckRequest</h2>
<!-- backwards compatibility -->
<a id="schemaaddhealthcheckrequest"></a>
<a id="schema_addHealthCheckRequest"></a>
<a id="tocSaddhealthcheckrequest"></a>
<a id="tocsaddhealthcheckrequest"></a>

```json
{
  "docId": 0,
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}

```

AddHealthCheckRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|docId|integer(int32)|false|none|none|
|instanceId|integer(int32)|false|none|none|
|edOrgId|integer(int32)|false|none|none|
|tenantId|integer(int32)|false|none|none|
|document|string|false|none|none|

<h2 id="tocS_addInstanceRequest">addInstanceRequest</h2>
<!-- backwards compatibility -->
<a id="schemaaddinstancerequest"></a>
<a id="schema_addInstanceRequest"></a>
<a id="tocSaddinstancerequest"></a>
<a id="tocsaddinstancerequest"></a>

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|instanceId|integer(int32)|false|none|none|
|edOrgId|integer(int32)¦null|false|none|none|
|tenantId|integer(int32)|false|none|none|
|document|string|false|none|none|

<h2 id="tocS_addPermissionRequest">addPermissionRequest</h2>
<!-- backwards compatibility -->
<a id="schemaaddpermissionrequest"></a>
<a id="schema_addPermissionRequest"></a>
<a id="tocSaddpermissionrequest"></a>
<a id="tocsaddpermissionrequest"></a>

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|instanceId|integer(int32)|false|none|none|
|edOrgId|integer(int32)¦null|false|none|none|
|tenantId|integer(int32)|false|none|none|
|document|string|false|none|none|

<h2 id="tocS_addStepRequest">addStepRequest</h2>
<!-- backwards compatibility -->
<a id="schemaaddsteprequest"></a>
<a id="schema_addStepRequest"></a>
<a id="tocSaddsteprequest"></a>
<a id="tocsaddsteprequest"></a>

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|instanceId|integer(int32)|false|none|none|
|edOrgId|integer(int32)¦null|false|none|none|
|tenantId|integer(int32)|false|none|none|
|document|string|false|none|none|

<h2 id="tocS_addTenantRequest">addTenantRequest</h2>
<!-- backwards compatibility -->
<a id="schemaaddtenantrequest"></a>
<a id="schema_addTenantRequest"></a>
<a id="tocSaddtenantrequest"></a>
<a id="tocsaddtenantrequest"></a>

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|instanceId|integer(int32)|false|none|none|
|edOrgId|integer(int32)|false|none|none|
|tenantId|integer(int32)|false|none|none|
|document|string|false|none|none|

<h2 id="tocS_addUserProfileRequest">addUserProfileRequest</h2>
<!-- backwards compatibility -->
<a id="schemaadduserprofilerequest"></a>
<a id="schema_addUserProfileRequest"></a>
<a id="tocSadduserprofilerequest"></a>
<a id="tocsadduserprofilerequest"></a>

```json
{
  "instanceId": 0,
  "edOrgId": 0,
  "tenantId": 0,
  "document": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|instanceId|integer(int32)|false|none|none|
|edOrgId|integer(int32)¦null|false|none|none|
|tenantId|integer(int32)|false|none|none|
|document|string|false|none|none|

<h2 id="tocS_adminApiError">adminApiError</h2>
<!-- backwards compatibility -->
<a id="schemaadminapierror"></a>
<a id="schema_adminApiError"></a>
<a id="tocSadminapierror"></a>
<a id="tocsadminapierror"></a>

```json
{
  "title": "string",
  "errors": [
    "string"
  ]
}

```

AdminApiError

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|title|string¦null|false|read-only|none|
|errors|[string]¦null|false|read-only|none|

<h2 id="tocS_extension">extension</h2>
<!-- backwards compatibility -->
<a id="schemaextension"></a>
<a id="schema_extension"></a>
<a id="tocSextension"></a>
<a id="tocsextension"></a>

```json
{
  "code": "string",
  "data": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|code|string¦null|false|none|none|
|data|string¦null|false|none|none|

<h2 id="tocS_informationResult">informationResult</h2>
<!-- backwards compatibility -->
<a id="schemainformationresult"></a>
<a id="schema_informationResult"></a>
<a id="tocSinformationresult"></a>
<a id="tocsinformationresult"></a>

```json
{
  "version": "string",
  "build": "string"
}

```

Information

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|version|string|false|none|Application version|
|build|string|false|none|Build / release version|

<h2 id="tocS_instanceModel">instanceModel</h2>
<!-- backwards compatibility -->
<a id="schemainstancemodel"></a>
<a id="schema_instanceModel"></a>
<a id="tocSinstancemodel"></a>
<a id="tocsinstancemodel"></a>

```json
{
  "docId": 0,
  "instanceId": 0,
  "tenantId": 0,
  "edOrgId": 0,
  "document": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|docId|integer(int32)|false|none|none|
|instanceId|integer(int32)¦null|false|none|none|
|tenantId|integer(int32)¦null|false|none|none|
|edOrgId|integer(int32)¦null|false|none|none|
|document|string¦null|false|none|none|

<h2 id="tocS_permissionModel">permissionModel</h2>
<!-- backwards compatibility -->
<a id="schemapermissionmodel"></a>
<a id="schema_permissionModel"></a>
<a id="tocSpermissionmodel"></a>
<a id="tocspermissionmodel"></a>

```json
{
  "docId": 0,
  "instanceId": 0,
  "tenantId": 0,
  "edOrgId": 0,
  "document": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|docId|integer(int32)|false|none|none|
|instanceId|integer(int32)¦null|false|none|none|
|tenantId|integer(int32)¦null|false|none|none|
|edOrgId|integer(int32)¦null|false|none|none|
|document|string¦null|false|none|none|

<h2 id="tocS_preference">preference</h2>
<!-- backwards compatibility -->
<a id="schemapreference"></a>
<a id="schema_preference"></a>
<a id="tocSpreference"></a>
<a id="tocspreference"></a>

```json
{
  "code": "string",
  "value": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|code|string¦null|false|none|none|
|value|string¦null|false|none|none|

<h2 id="tocS_registerClientRequest">registerClientRequest</h2>
<!-- backwards compatibility -->
<a id="schemaregisterclientrequest"></a>
<a id="schema_registerClientRequest"></a>
<a id="tocSregisterclientrequest"></a>
<a id="tocsregisterclientrequest"></a>

```json
{
  "clientId": "string",
  "clientSecret": "string",
  "displayName": "string"
}

```

RegisterClientRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|clientId|string|false|none|Client id|
|clientSecret|string|false|none|Client secret|
|displayName|string|false|none|Client display name|

<h2 id="tocS_selectedTenant">selectedTenant</h2>
<!-- backwards compatibility -->
<a id="schemaselectedtenant"></a>
<a id="schema_selectedTenant"></a>
<a id="tocSselectedtenant"></a>
<a id="tocsselectedtenant"></a>

```json
{
  "createdBy": "string",
  "createdDateTime": "2019-08-24T14:15:22Z",
  "domains": [
    "string"
  ],
  "isDemo": true,
  "isIdentityProviders": [
    "string"
  ],
  "lastModifiedBy": "string",
  "lastModifiedDateTime": "2019-08-24T14:15:22Z",
  "organizationIdentifier": "string",
  "organizationName": "string",
  "state": "string",
  "subscriptions": [
    null
  ],
  "subscriptionsMigrated": true,
  "tenantId": "string",
  "tenantStatus": "string",
  "tenantType": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|createdBy|string¦null|false|none|none|
|createdDateTime|string(date-time)¦null|false|none|none|
|domains|[string]¦null|false|none|none|
|isDemo|boolean¦null|false|none|none|
|isIdentityProviders|[string]¦null|false|none|none|
|lastModifiedBy|string¦null|false|none|none|
|lastModifiedDateTime|string(date-time)¦null|false|none|none|
|organizationIdentifier|string¦null|false|none|none|
|organizationName|string¦null|false|none|none|
|state|string¦null|false|none|none|
|subscriptions|[any]¦null|false|none|none|
|subscriptionsMigrated|boolean¦null|false|none|none|
|tenantId|string¦null|false|none|none|
|tenantStatus|string¦null|false|none|none|
|tenantType|string¦null|false|none|none|

<h2 id="tocS_stepModel">stepModel</h2>
<!-- backwards compatibility -->
<a id="schemastepmodel"></a>
<a id="schema_stepModel"></a>
<a id="tocSstepmodel"></a>
<a id="tocsstepmodel"></a>

```json
{
  "number": 0,
  "description": "string",
  "startedAt": "2019-08-24T14:15:22Z",
  "completedAt": "2019-08-24T14:15:22Z",
  "status": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|number|integer(int32)¦null|false|none|none|
|description|string¦null|false|none|none|
|startedAt|string(date-time)¦null|false|none|none|
|completedAt|string(date-time)¦null|false|none|none|
|status|string¦null|false|none|none|

<h2 id="tocS_userProfileModel">userProfileModel</h2>
<!-- backwards compatibility -->
<a id="schemauserprofilemodel"></a>
<a id="schema_userProfileModel"></a>
<a id="tocSuserprofilemodel"></a>
<a id="tocsuserprofilemodel"></a>

```json
{
  "tenantId": "string",
  "firstName": "string",
  "lastName": "string",
  "userName": "string",
  "email": "string",
  "preferences": [
    {
      "code": "string",
      "value": "string"
    }
  ],
  "extensions": [
    {
      "code": "string",
      "data": "string"
    }
  ],
  "tenants": [
    {
      "createdBy": "string",
      "createdDateTime": "2019-08-24T14:15:22Z",
      "domains": [
        "string"
      ],
      "isDemo": true,
      "isIdentityProviders": [
        "string"
      ],
      "lastModifiedBy": "string",
      "lastModifiedDateTime": "2019-08-24T14:15:22Z",
      "organizationIdentifier": "string",
      "organizationName": "string",
      "state": "string",
      "subscriptions": [
        null
      ],
      "subscriptionsMigrated": true,
      "tenantId": "string",
      "tenantStatus": "string",
      "tenantType": "string"
    }
  ],
  "selectedTenant": {
    "createdBy": "string",
    "createdDateTime": "2019-08-24T14:15:22Z",
    "domains": [
      "string"
    ],
    "isDemo": true,
    "isIdentityProviders": [
      "string"
    ],
    "lastModifiedBy": "string",
    "lastModifiedDateTime": "2019-08-24T14:15:22Z",
    "organizationIdentifier": "string",
    "organizationName": "string",
    "state": "string",
    "subscriptions": [
      null
    ],
    "subscriptionsMigrated": true,
    "tenantId": "string",
    "tenantStatus": "string",
    "tenantType": "string"
  },
  "tenantsTotalCount": 0
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|tenantId|string¦null|false|none|none|
|firstName|string¦null|false|none|none|
|lastName|string¦null|false|none|none|
|userName|string¦null|false|none|none|
|email|string¦null|false|none|none|
|preferences|[[preference](#schemapreference)]¦null|false|none|none|
|extensions|[[extension](#schemaextension)]¦null|false|none|none|
|tenants|[[userProfileTenant](#schemauserprofiletenant)]¦null|false|none|none|
|selectedTenant|[selectedTenant](#schemaselectedtenant)|false|none|none|
|tenantsTotalCount|integer(int32)¦null|false|none|none|

<h2 id="tocS_userProfileTenant">userProfileTenant</h2>
<!-- backwards compatibility -->
<a id="schemauserprofiletenant"></a>
<a id="schema_userProfileTenant"></a>
<a id="tocSuserprofiletenant"></a>
<a id="tocsuserprofiletenant"></a>

```json
{
  "createdBy": "string",
  "createdDateTime": "2019-08-24T14:15:22Z",
  "domains": [
    "string"
  ],
  "isDemo": true,
  "isIdentityProviders": [
    "string"
  ],
  "lastModifiedBy": "string",
  "lastModifiedDateTime": "2019-08-24T14:15:22Z",
  "organizationIdentifier": "string",
  "organizationName": "string",
  "state": "string",
  "subscriptions": [
    null
  ],
  "subscriptionsMigrated": true,
  "tenantId": "string",
  "tenantStatus": "string",
  "tenantType": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|createdBy|string¦null|false|none|none|
|createdDateTime|string(date-time)¦null|false|none|none|
|domains|[string]¦null|false|none|none|
|isDemo|boolean¦null|false|none|none|
|isIdentityProviders|[string]¦null|false|none|none|
|lastModifiedBy|string¦null|false|none|none|
|lastModifiedDateTime|string(date-time)¦null|false|none|none|
|organizationIdentifier|string¦null|false|none|none|
|organizationName|string¦null|false|none|none|
|state|string¦null|false|none|none|
|subscriptions|[any]¦null|false|none|none|
|subscriptionsMigrated|boolean¦null|false|none|none|
|tenantId|string¦null|false|none|none|
|tenantStatus|string¦null|false|none|none|
|tenantType|string¦null|false|none|none|

