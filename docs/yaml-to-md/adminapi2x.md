---
title: Admin API Documentation v2
language_tabs:
  - http: HTTP
  - python: PYTHON
  - csharp: CSHARP
language_clients:
  - http: ""
  - python: ""
  - csharp: ""
toc_footers: []
includes: []
search: false
highlight_theme: darkula
headingLevel: 2

---

<!-- Generator: Widdershins v4.0.1 -->

<h1 id="admin-api-documentation">Admin API Documentation v2</h1>

> Scroll down for code samples, example requests and responses. Select a language for code samples from the tabs above or the mobile navigation menu.

# Authentication

- oAuth2 authentication. 

    - Flow: clientCredentials

    - Token URL = [http://localhost/connect/token](http://localhost/connect/token)

|Scope|Scope Description|
|---|---|
|edfi_admin_api/full_access|Unrestricted access to all Admin API endpoints|

<h1 id="admin-api-documentation-resourceclaims">ResourceClaims</h1>

## Retrieves all resourceClaims.

> Code samples

```http
GET /v2/resourceClaims HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/resourceClaims', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/resourceClaims";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/resourceClaims`

<h3 id="retrieves-all-resourceclaims.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|
|orderBy|query|string|false|Indicates the property name by which the results will be sorted.|
|direction|query|string|false|Indicates whether the result should be sorted in descending order (DESC) or ascending order (ASC).|
|id|query|integer(int32)|false|Resource Claim Id|
|name|query|string|false|Resource Claim Name|

#### Enumerated Values

|Parameter|Value|
|---|---|
|direction|Ascending|
|direction|Descending|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "name": "string",
    "parentId": 0,
    "parentName": "string",
    "children": [
      {}
    ]
  }
]
```

<h3 id="retrieves-all-resourceclaims.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-resourceclaims.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.resourceclaimmodel)]|false|none|none|
|» ResourceClaimModel|[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.resourceclaimmodel)|false|none|none|
|»» id|integer(int32)|true|none|none|
|»» name|string¦null|true|none|none|
|»» parentId|integer(int32)¦null|true|none|none|
|»» parentName|string¦null|true|none|none|
|»» children|[[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.resourceclaimmodel)]¦null|true|none|Children are collection of SimpleResourceClaimModel|
|»»» ResourceClaimModel|[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.resourceclaimmodel)|false|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves a specific resourceClaim based on the identifier.

> Code samples

```http
GET /v2/resourceClaims/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/resourceClaims/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/resourceClaims/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/resourceClaims/{id}`

<h3 id="retrieves-a-specific-resourceclaim-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "name": "string",
  "parentId": 0,
  "parentName": "string",
  "children": [
    {
      "id": 0,
      "name": "string",
      "parentId": 0,
      "parentName": "string",
      "children": []
    }
  ]
}
```

<h3 id="retrieves-a-specific-resourceclaim-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.resourceclaimmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-vendors">Vendors</h1>

## Retrieves all vendors.

> Code samples

```http
GET /v2/vendors HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/vendors', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/vendors";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/vendors`

<h3 id="retrieves-all-vendors.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|
|orderBy|query|string|false|Indicates the property name by which the results will be sorted.|
|direction|query|string|false|Indicates whether the result should be sorted in descending order (DESC) or ascending order (ASC).|
|id|query|integer(int32)|false|Vendor/ company id|
|company|query|string|false|Vendor/ company name|
|namespacePrefixes|query|string|false|Namespace prefix for the vendor. Multiple namespace prefixes can be provided as comma separated list if required.|
|contactName|query|string|false|Vendor contact name|
|contactEmailAddress|query|string|false|Vendor contact email id|

#### Enumerated Values

|Parameter|Value|
|---|---|
|direction|Ascending|
|direction|Descending|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "company": "string",
    "namespacePrefixes": "string",
    "contactName": "string",
    "contactEmailAddress": "string"
  }
]
```

<h3 id="retrieves-all-vendors.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-vendors.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.Vendors.VendorModel](#schemaedfi.ods.adminapi.features.vendors.vendormodel)]|false|none|none|
|» Vendor|[EdFi.Ods.AdminApi.Features.Vendors.VendorModel](#schemaedfi.ods.adminapi.features.vendors.vendormodel)|false|none|none|
|»» id|integer(int32)¦null|true|none|none|
|»» company|string¦null|true|none|none|
|»» namespacePrefixes|string¦null|true|none|none|
|»» contactName|string¦null|true|none|none|
|»» contactEmailAddress|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Creates vendor based on the supplied values.

> Code samples

```http
POST /v2/vendors HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/v2/vendors', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/v2/vendors";
      
      string json = @"{
  ""company"": ""string"",
  ""namespacePrefixes"": ""string"",
  ""contactName"": ""string"",
  ""contactEmailAddress"": ""string""
}";
      EdFi.Ods.AdminApi.Features.Vendors.AddVendor.Request content = JsonConvert.DeserializeObject(json);
      await PostAsync(content, url);
      
      
    }

    /// Performs a POST Request
    public async Task PostAsync(EdFi.Ods.AdminApi.Features.Vendors.AddVendor.Request content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.Vendors.AddVendor.Request content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`POST /v2/vendors`

> Body parameter

```json
{
  "company": "string",
  "namespacePrefixes": "string",
  "contactName": "string",
  "contactEmailAddress": "string"
}
```

<h3 id="creates-vendor-based-on-the-supplied-values.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[EdFi.Ods.AdminApi.Features.Vendors.AddVendor.Request](#schemaedfi.ods.adminapi.features.vendors.addvendor.request)|true|none|

<h3 id="creates-vendor-based-on-the-supplied-values.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves a specific vendor based on the identifier.

> Code samples

```http
GET /v2/vendors/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/vendors/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/vendors/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/vendors/{id}`

<h3 id="retrieves-a-specific-vendor-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "company": "string",
  "namespacePrefixes": "string",
  "contactName": "string",
  "contactEmailAddress": "string"
}
```

<h3 id="retrieves-a-specific-vendor-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.Vendors.VendorModel](#schemaedfi.ods.adminapi.features.vendors.vendormodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Updates vendor based on the resource identifier.

> Code samples

```http
PUT /v2/vendors/{id} HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/vendors/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/vendors/{id}";

      
      string json = @"{
  ""company"": ""string"",
  ""namespacePrefixes"": ""string"",
  ""contactName"": ""string"",
  ""contactEmailAddress"": ""string""
}";
      EdFi.Ods.AdminApi.Features.Vendors.EditVendor.Request content = JsonConvert.DeserializeObject(json);
      var result = await PutAsync(id, content, url);
      
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, EdFi.Ods.AdminApi.Features.Vendors.EditVendor.Request content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.Vendors.EditVendor.Request content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/vendors/{id}`

> Body parameter

```json
{
  "company": "string",
  "namespacePrefixes": "string",
  "contactName": "string",
  "contactEmailAddress": "string"
}
```

<h3 id="updates-vendor-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.Vendors.EditVendor.Request](#schemaedfi.ods.adminapi.features.vendors.editvendor.request)|true|none|

<h3 id="updates-vendor-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Deletes an existing vendor using the resource identifier.

> Code samples

```http
DELETE /v2/vendors/{id} HTTP/1.1

```

```python
import requests
headers = {
  'Authorization': 'Bearer {access-token}'
}

r = requests.delete('/v2/vendors/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    
    /// Make a dummy request
    public async Task MakeDeleteRequest()
    {
      int id = 1;
      string url = "/v2/vendors/{id}";

      await DeleteAsync(id, url);
    }

    /// Performs a DELETE Request
    public async Task DeleteAsync(int id, string url)
    {
        //Execute DELETE request
        HttpResponseMessage response = await Client.DeleteAsync(url + $"/{id}");

        //Return response
        await DeserializeObject(response);
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`DELETE /v2/vendors/{id}`

<h3 id="deletes-an-existing-vendor-using-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

<h3 id="deletes-an-existing-vendor-using-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Resource was successfully deleted.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves applications assigned to a specific vendor based on the resource identifier.

> Code samples

```http
GET /v2/vendors/{id}/applications HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/vendors/{id}/applications', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/vendors/{id}/applications";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/vendors/{id}/applications`

<h3 id="retrieves-applications-assigned-to-a-specific-vendor-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "applicationName": "string",
    "claimSetName": "string",
    "educationOrganizationIds": [
      0
    ],
    "vendorId": 0,
    "profileIds": [
      0
    ],
    "odsInstanceIds": [
      0
    ]
  }
]
```

<h3 id="retrieves-applications-assigned-to-a-specific-vendor-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-applications-assigned-to-a-specific-vendor-based-on-the-resource-identifier.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.Applications.ApplicationModel](#schemaedfi.ods.adminapi.features.applications.applicationmodel)]|false|none|none|
|» Application|[EdFi.Ods.AdminApi.Features.Applications.ApplicationModel](#schemaedfi.ods.adminapi.features.applications.applicationmodel)|false|none|none|
|»» id|integer(int32)|true|none|none|
|»» applicationName|string¦null|true|none|none|
|»» claimSetName|string¦null|true|none|none|
|»» educationOrganizationIds|[integer]¦null|true|none|none|
|»» vendorId|integer(int32)¦null|true|none|none|
|»» profileIds|[integer]¦null|true|none|none|
|»» odsInstanceIds|[integer]¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-profiles">Profiles</h1>

## Retrieves all profiles.

> Code samples

```http
GET /v2/profiles HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/profiles', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/profiles";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/profiles`

<h3 id="retrieves-all-profiles.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|
|orderBy|query|string|false|Indicates the property name by which the results will be sorted.|
|direction|query|string|false|Indicates whether the result should be sorted in descending order (DESC) or ascending order (ASC).|
|id|query|integer(int32)|false|Profile id|
|name|query|string|false|Profile name|

#### Enumerated Values

|Parameter|Value|
|---|---|
|direction|Ascending|
|direction|Descending|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "name": "string"
  }
]
```

<h3 id="retrieves-all-profiles.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-profiles.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.Profiles.ProfileModel](#schemaedfi.ods.adminapi.features.profiles.profilemodel)]|false|none|none|
|» Profile|[EdFi.Ods.AdminApi.Features.Profiles.ProfileModel](#schemaedfi.ods.adminapi.features.profiles.profilemodel)|false|none|none|
|»» id|integer(int32)¦null|true|none|none|
|»» name|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Creates profile based on the supplied values.

> Code samples

```http
POST /v2/profiles HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/v2/profiles', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/v2/profiles";
      
      string json = @"{
  ""name"": ""Test-Profile"",
  ""definition"": ""<Profile name=\""Test-Profile\""><Resource name=\""Resource1\""><ReadContentType memberSelection=\""IncludeOnly\""><Collection name=\""Collection1\"" memberSelection=\""IncludeOnly\""><Property name=\""Property1\"" /><Property name=\""Property2\"" /></Collection></ReadContentType><WriteContentType memberSelection=\""IncludeOnly\""><Collection name=\""Collection2\"" memberSelection=\""IncludeOnly\""><Property name=\""Property1\"" /><Property name=\""Property2\"" /></Collection></WriteContentType></Resource></Profile>""
}";
      EdFi.Ods.AdminApi.Features.Profiles.AddProfile.AddProfileRequest content = JsonConvert.DeserializeObject(json);
      await PostAsync(content, url);
      
      
    }

    /// Performs a POST Request
    public async Task PostAsync(EdFi.Ods.AdminApi.Features.Profiles.AddProfile.AddProfileRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.Profiles.AddProfile.AddProfileRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`POST /v2/profiles`

> Body parameter

```json
"{\r\n  \"name\": \"Test-Profile\",\r\n  \"definition\": \"<Profile name=\\\"Test-Profile\\\"><Resource name=\\\"Resource1\\\"><ReadContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection1\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></ReadContentType><WriteContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection2\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></WriteContentType></Resource></Profile>\"\r\n}"
```

<h3 id="creates-profile-based-on-the-supplied-values.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[EdFi.Ods.AdminApi.Features.Profiles.AddProfile.AddProfileRequest](#schemaedfi.ods.adminapi.features.profiles.addprofile.addprofilerequest)|true|none|

<h3 id="creates-profile-based-on-the-supplied-values.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves a specific profile based on the identifier.

> Code samples

```http
GET /v2/profiles/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/profiles/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/profiles/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/profiles/{id}`

<h3 id="retrieves-a-specific-profile-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "name": "string",
  "definition": "string"
}
```

<h3 id="retrieves-a-specific-profile-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.Profiles.ProfileDetailsModel](#schemaedfi.ods.adminapi.features.profiles.profiledetailsmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Updates profile based on the resource identifier.

> Code samples

```http
PUT /v2/profiles/{id} HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/profiles/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/profiles/{id}";

      
      string json = @"{
  ""name"": ""Test-Profile"",
  ""definition"": ""<Profile name=\""Test-Profile\""><Resource name=\""Resource1\""><ReadContentType memberSelection=\""IncludeOnly\""><Collection name=\""Collection1\"" memberSelection=\""IncludeOnly\""><Property name=\""Property1\"" /><Property name=\""Property2\"" /></Collection></ReadContentType><WriteContentType memberSelection=\""IncludeOnly\""><Collection name=\""Collection2\"" memberSelection=\""IncludeOnly\""><Property name=\""Property1\"" /><Property name=\""Property2\"" /></Collection></WriteContentType></Resource></Profile>""
}";
      EdFi.Ods.AdminApi.Features.Profiles.EditProfile.EditProfileRequest content = JsonConvert.DeserializeObject(json);
      var result = await PutAsync(id, content, url);
      
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, EdFi.Ods.AdminApi.Features.Profiles.EditProfile.EditProfileRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.Profiles.EditProfile.EditProfileRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/profiles/{id}`

> Body parameter

```json
"{\r\n  \"name\": \"Test-Profile\",\r\n  \"definition\": \"<Profile name=\\\"Test-Profile\\\"><Resource name=\\\"Resource1\\\"><ReadContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection1\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></ReadContentType><WriteContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection2\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></WriteContentType></Resource></Profile>\"\r\n}"
```

<h3 id="updates-profile-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.Profiles.EditProfile.EditProfileRequest](#schemaedfi.ods.adminapi.features.profiles.editprofile.editprofilerequest)|true|none|

<h3 id="updates-profile-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Deletes an existing profile using the resource identifier.

> Code samples

```http
DELETE /v2/profiles/{id} HTTP/1.1

```

```python
import requests
headers = {
  'Authorization': 'Bearer {access-token}'
}

r = requests.delete('/v2/profiles/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    
    /// Make a dummy request
    public async Task MakeDeleteRequest()
    {
      int id = 1;
      string url = "/v2/profiles/{id}";

      await DeleteAsync(id, url);
    }

    /// Performs a DELETE Request
    public async Task DeleteAsync(int id, string url)
    {
        //Execute DELETE request
        HttpResponseMessage response = await Client.DeleteAsync(url + $"/{id}");

        //Return response
        await DeserializeObject(response);
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`DELETE /v2/profiles/{id}`

<h3 id="deletes-an-existing-profile-using-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

<h3 id="deletes-an-existing-profile-using-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Resource was successfully deleted.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-odsinstances">OdsInstances</h1>

## Retrieves all odsInstances.

> Code samples

```http
GET /v2/odsInstances HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/odsInstances', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/odsInstances";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/odsInstances`

<h3 id="retrieves-all-odsinstances.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|
|orderBy|query|string|false|Indicates the property name by which the results will be sorted.|
|direction|query|string|false|Indicates whether the result should be sorted in descending order (DESC) or ascending order (ASC).|
|id|query|integer(int32)|false|List of ODS instance id|
|name|query|string|false|Ods Instance name|

#### Enumerated Values

|Parameter|Value|
|---|---|
|direction|Ascending|
|direction|Descending|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "name": "string",
    "instanceType": "string"
  }
]
```

<h3 id="retrieves-all-odsinstances.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-odsinstances.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceModel](#schemaedfi.ods.adminapi.features.odsinstances.odsinstancemodel)]|false|none|none|
|» OdsInstance|[EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceModel](#schemaedfi.ods.adminapi.features.odsinstances.odsinstancemodel)|false|none|none|
|»» id|integer(int32)|false|none|none|
|»» name|string¦null|true|none|none|
|»» instanceType|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Creates odsInstance based on the supplied values.

> Code samples

```http
POST /v2/odsInstances HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/v2/odsInstances', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/v2/odsInstances";
      
      string json = @"{
  ""name"": ""string"",
  ""instanceType"": ""string"",
  ""connectionString"": ""string""
}";
      EdFi.Ods.AdminApi.Features.OdsInstances.AddOdsInstance.AddOdsInstanceRequest content = JsonConvert.DeserializeObject(json);
      await PostAsync(content, url);
      
      
    }

    /// Performs a POST Request
    public async Task PostAsync(EdFi.Ods.AdminApi.Features.OdsInstances.AddOdsInstance.AddOdsInstanceRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.OdsInstances.AddOdsInstance.AddOdsInstanceRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`POST /v2/odsInstances`

> Body parameter

```json
{
  "name": "string",
  "instanceType": "string",
  "connectionString": "string"
}
```

<h3 id="creates-odsinstance-based-on-the-supplied-values.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[EdFi.Ods.AdminApi.Features.OdsInstances.AddOdsInstance.AddOdsInstanceRequest](#schemaedfi.ods.adminapi.features.odsinstances.addodsinstance.addodsinstancerequest)|true|none|

<h3 id="creates-odsinstance-based-on-the-supplied-values.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves a specific odsInstance based on the identifier.

> Code samples

```http
GET /v2/odsInstances/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/odsInstances/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/odsInstances/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/odsInstances/{id}`

<h3 id="retrieves-a-specific-odsinstance-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "name": "string",
  "instanceType": "string",
  "odsInstanceContexts": [
    {
      "id": 0,
      "odsInstanceId": 0,
      "contextKey": "string",
      "contextValue": "string"
    }
  ],
  "odsInstanceDerivatives": [
    {
      "id": 0,
      "odsInstanceId": 0,
      "derivativeType": "string"
    }
  ]
}
```

<h3 id="retrieves-a-specific-odsinstance-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceDetailModel](#schemaedfi.ods.adminapi.features.odsinstances.odsinstancedetailmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Updates odsInstance based on the resource identifier.

> Code samples

```http
PUT /v2/odsInstances/{id} HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/odsInstances/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/odsInstances/{id}";

      
      string json = @"{
  ""name"": ""string"",
  ""instanceType"": ""string"",
  ""connectionString"": ""string""
}";
      EdFi.Ods.AdminApi.Features.OdsInstances.EditOdsInstance.EditOdsInstanceRequest content = JsonConvert.DeserializeObject(json);
      var result = await PutAsync(id, content, url);
      
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, EdFi.Ods.AdminApi.Features.OdsInstances.EditOdsInstance.EditOdsInstanceRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.OdsInstances.EditOdsInstance.EditOdsInstanceRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/odsInstances/{id}`

> Body parameter

```json
{
  "name": "string",
  "instanceType": "string",
  "connectionString": "string"
}
```

<h3 id="updates-odsinstance-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.OdsInstances.EditOdsInstance.EditOdsInstanceRequest](#schemaedfi.ods.adminapi.features.odsinstances.editodsinstance.editodsinstancerequest)|true|none|

<h3 id="updates-odsinstance-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Deletes an existing odsInstance using the resource identifier.

> Code samples

```http
DELETE /v2/odsInstances/{id} HTTP/1.1

```

```python
import requests
headers = {
  'Authorization': 'Bearer {access-token}'
}

r = requests.delete('/v2/odsInstances/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    
    /// Make a dummy request
    public async Task MakeDeleteRequest()
    {
      int id = 1;
      string url = "/v2/odsInstances/{id}";

      await DeleteAsync(id, url);
    }

    /// Performs a DELETE Request
    public async Task DeleteAsync(int id, string url)
    {
        //Execute DELETE request
        HttpResponseMessage response = await Client.DeleteAsync(url + $"/{id}");

        //Return response
        await DeserializeObject(response);
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`DELETE /v2/odsInstances/{id}`

<h3 id="deletes-an-existing-odsinstance-using-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

<h3 id="deletes-an-existing-odsinstance-using-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Resource was successfully deleted.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves applications assigned to a specific ODS instance based on the resource identifier.

> Code samples

```http
GET /v2/odsInstances/{id}/applications HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/odsInstances/{id}/applications', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/odsInstances/{id}/applications";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/odsInstances/{id}/applications`

<h3 id="retrieves-applications-assigned-to-a-specific-ods-instance-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "applicationName": "string",
    "claimSetName": "string",
    "educationOrganizationIds": [
      0
    ],
    "vendorId": 0,
    "profileIds": [
      0
    ],
    "odsInstanceIds": [
      0
    ]
  }
]
```

<h3 id="retrieves-applications-assigned-to-a-specific-ods-instance-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-applications-assigned-to-a-specific-ods-instance-based-on-the-resource-identifier.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.Applications.ApplicationModel](#schemaedfi.ods.adminapi.features.applications.applicationmodel)]|false|none|none|
|» Application|[EdFi.Ods.AdminApi.Features.Applications.ApplicationModel](#schemaedfi.ods.adminapi.features.applications.applicationmodel)|false|none|none|
|»» id|integer(int32)|true|none|none|
|»» applicationName|string¦null|true|none|none|
|»» claimSetName|string¦null|true|none|none|
|»» educationOrganizationIds|[integer]¦null|true|none|none|
|»» vendorId|integer(int32)¦null|true|none|none|
|»» profileIds|[integer]¦null|true|none|none|
|»» odsInstanceIds|[integer]¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-odsinstancederivatives">OdsInstanceDerivatives</h1>

## Retrieves all odsInstanceDerivatives.

> Code samples

```http
GET /v2/odsInstanceDerivatives HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/odsInstanceDerivatives', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/odsInstanceDerivatives";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/odsInstanceDerivatives`

<h3 id="retrieves-all-odsinstancederivatives.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "odsInstanceId": 0,
    "derivativeType": "string"
  }
]
```

<h3 id="retrieves-all-odsinstancederivatives.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-odsinstancederivatives.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.OdsInstanceDerivativeModel](#schemaedfi.ods.adminapi.features.odsinstancederivative.odsinstancederivativemodel)]|false|none|none|
|» OdsInstanceDerivative|[EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.OdsInstanceDerivativeModel](#schemaedfi.ods.adminapi.features.odsinstancederivative.odsinstancederivativemodel)|false|none|none|
|»» id|integer(int32)|true|none|none|
|»» odsInstanceId|integer(int32)¦null|true|none|none|
|»» derivativeType|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Creates odsInstanceDerivative based on the supplied values.

> Code samples

```http
POST /v2/odsInstanceDerivatives HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/v2/odsInstanceDerivatives', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/v2/odsInstanceDerivatives";
      
      string json = @"{
  ""odsInstanceId"": 0,
  ""derivativeType"": ""string"",
  ""connectionString"": ""string""
}";
      EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.AddOdsInstanceDerivative.AddOdsInstanceDerivativeRequest content = JsonConvert.DeserializeObject(json);
      await PostAsync(content, url);
      
      
    }

    /// Performs a POST Request
    public async Task PostAsync(EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.AddOdsInstanceDerivative.AddOdsInstanceDerivativeRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.AddOdsInstanceDerivative.AddOdsInstanceDerivativeRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`POST /v2/odsInstanceDerivatives`

> Body parameter

```json
{
  "odsInstanceId": 0,
  "derivativeType": "string",
  "connectionString": "string"
}
```

<h3 id="creates-odsinstancederivative-based-on-the-supplied-values.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.AddOdsInstanceDerivative.AddOdsInstanceDerivativeRequest](#schemaedfi.ods.adminapi.features.odsinstancederivative.addodsinstancederivative.addodsinstancederivativerequest)|true|none|

<h3 id="creates-odsinstancederivative-based-on-the-supplied-values.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves a specific odsInstanceDerivative based on the identifier.

> Code samples

```http
GET /v2/odsInstanceDerivatives/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/odsInstanceDerivatives/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/odsInstanceDerivatives/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/odsInstanceDerivatives/{id}`

<h3 id="retrieves-a-specific-odsinstancederivative-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "odsInstanceId": 0,
  "derivativeType": "string"
}
```

<h3 id="retrieves-a-specific-odsinstancederivative-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.OdsInstanceDerivativeModel](#schemaedfi.ods.adminapi.features.odsinstancederivative.odsinstancederivativemodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Updates odsInstanceDerivative based on the resource identifier.

> Code samples

```http
PUT /v2/odsInstanceDerivatives/{id} HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/odsInstanceDerivatives/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/odsInstanceDerivatives/{id}";

      
      string json = @"{
  ""odsInstanceId"": 0,
  ""derivativeType"": ""string"",
  ""connectionString"": ""string""
}";
      EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.EditOdsInstanceDerivative.EditOdsInstanceDerivativeRequest content = JsonConvert.DeserializeObject(json);
      var result = await PutAsync(id, content, url);
      
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.EditOdsInstanceDerivative.EditOdsInstanceDerivativeRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.EditOdsInstanceDerivative.EditOdsInstanceDerivativeRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/odsInstanceDerivatives/{id}`

> Body parameter

```json
{
  "odsInstanceId": 0,
  "derivativeType": "string",
  "connectionString": "string"
}
```

<h3 id="updates-odsinstancederivative-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.EditOdsInstanceDerivative.EditOdsInstanceDerivativeRequest](#schemaedfi.ods.adminapi.features.odsinstancederivative.editodsinstancederivative.editodsinstancederivativerequest)|true|none|

<h3 id="updates-odsinstancederivative-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Deletes an existing odsInstanceDerivative using the resource identifier.

> Code samples

```http
DELETE /v2/odsInstanceDerivatives/{id} HTTP/1.1

```

```python
import requests
headers = {
  'Authorization': 'Bearer {access-token}'
}

r = requests.delete('/v2/odsInstanceDerivatives/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    
    /// Make a dummy request
    public async Task MakeDeleteRequest()
    {
      int id = 1;
      string url = "/v2/odsInstanceDerivatives/{id}";

      await DeleteAsync(id, url);
    }

    /// Performs a DELETE Request
    public async Task DeleteAsync(int id, string url)
    {
        //Execute DELETE request
        HttpResponseMessage response = await Client.DeleteAsync(url + $"/{id}");

        //Return response
        await DeserializeObject(response);
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`DELETE /v2/odsInstanceDerivatives/{id}`

<h3 id="deletes-an-existing-odsinstancederivative-using-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

<h3 id="deletes-an-existing-odsinstancederivative-using-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Resource was successfully deleted.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-odsinstancecontexts">OdsInstanceContexts</h1>

## Retrieves all odsInstanceContexts.

> Code samples

```http
GET /v2/odsInstanceContexts HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/odsInstanceContexts', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/odsInstanceContexts";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/odsInstanceContexts`

<h3 id="retrieves-all-odsinstancecontexts.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "odsInstanceId": 0,
    "contextKey": "string",
    "contextValue": "string"
  }
]
```

<h3 id="retrieves-all-odsinstancecontexts.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-odsinstancecontexts.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.OdsInstanceContext.OdsInstanceContextModel](#schemaedfi.ods.adminapi.features.odsinstancecontext.odsinstancecontextmodel)]|false|none|none|
|» OdsInstanceContext|[EdFi.Ods.AdminApi.Features.OdsInstanceContext.OdsInstanceContextModel](#schemaedfi.ods.adminapi.features.odsinstancecontext.odsinstancecontextmodel)|false|none|none|
|»» id|integer(int32)|false|none|none|
|»» odsInstanceId|integer(int32)|true|none|none|
|»» contextKey|string¦null|true|none|none|
|»» contextValue|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Creates odsInstanceContext based on the supplied values.

> Code samples

```http
POST /v2/odsInstanceContexts HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/v2/odsInstanceContexts', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/v2/odsInstanceContexts";
      
      string json = @"{
  ""odsInstanceId"": 0,
  ""contextKey"": ""string"",
  ""contextValue"": ""string""
}";
      EdFi.Ods.AdminApi.Features.OdsInstanceContext.AddOdsInstanceContext.AddOdsInstanceContextRequest content = JsonConvert.DeserializeObject(json);
      await PostAsync(content, url);
      
      
    }

    /// Performs a POST Request
    public async Task PostAsync(EdFi.Ods.AdminApi.Features.OdsInstanceContext.AddOdsInstanceContext.AddOdsInstanceContextRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.OdsInstanceContext.AddOdsInstanceContext.AddOdsInstanceContextRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`POST /v2/odsInstanceContexts`

> Body parameter

```json
{
  "odsInstanceId": 0,
  "contextKey": "string",
  "contextValue": "string"
}
```

<h3 id="creates-odsinstancecontext-based-on-the-supplied-values.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[EdFi.Ods.AdminApi.Features.OdsInstanceContext.AddOdsInstanceContext.AddOdsInstanceContextRequest](#schemaedfi.ods.adminapi.features.odsinstancecontext.addodsinstancecontext.addodsinstancecontextrequest)|true|none|

<h3 id="creates-odsinstancecontext-based-on-the-supplied-values.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves a specific odsInstanceContext based on the identifier.

> Code samples

```http
GET /v2/odsInstanceContexts/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/odsInstanceContexts/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/odsInstanceContexts/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/odsInstanceContexts/{id}`

<h3 id="retrieves-a-specific-odsinstancecontext-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "odsInstanceId": 0,
  "contextKey": "string",
  "contextValue": "string"
}
```

<h3 id="retrieves-a-specific-odsinstancecontext-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.OdsInstanceContext.OdsInstanceContextModel](#schemaedfi.ods.adminapi.features.odsinstancecontext.odsinstancecontextmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Updates odsInstanceContext based on the resource identifier.

> Code samples

```http
PUT /v2/odsInstanceContexts/{id} HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/odsInstanceContexts/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/odsInstanceContexts/{id}";

      
      string json = @"{
  ""odsInstanceId"": 0,
  ""contextKey"": ""string"",
  ""contextValue"": ""string""
}";
      EdFi.Ods.AdminApi.Features.OdsInstanceContext.EditOdsInstanceContext.EditOdsInstanceContextRequest content = JsonConvert.DeserializeObject(json);
      var result = await PutAsync(id, content, url);
      
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, EdFi.Ods.AdminApi.Features.OdsInstanceContext.EditOdsInstanceContext.EditOdsInstanceContextRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.OdsInstanceContext.EditOdsInstanceContext.EditOdsInstanceContextRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/odsInstanceContexts/{id}`

> Body parameter

```json
{
  "odsInstanceId": 0,
  "contextKey": "string",
  "contextValue": "string"
}
```

<h3 id="updates-odsinstancecontext-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.OdsInstanceContext.EditOdsInstanceContext.EditOdsInstanceContextRequest](#schemaedfi.ods.adminapi.features.odsinstancecontext.editodsinstancecontext.editodsinstancecontextrequest)|true|none|

<h3 id="updates-odsinstancecontext-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Deletes an existing odsInstanceContext using the resource identifier.

> Code samples

```http
DELETE /v2/odsInstanceContexts/{id} HTTP/1.1

```

```python
import requests
headers = {
  'Authorization': 'Bearer {access-token}'
}

r = requests.delete('/v2/odsInstanceContexts/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    
    /// Make a dummy request
    public async Task MakeDeleteRequest()
    {
      int id = 1;
      string url = "/v2/odsInstanceContexts/{id}";

      await DeleteAsync(id, url);
    }

    /// Performs a DELETE Request
    public async Task DeleteAsync(int id, string url)
    {
        //Execute DELETE request
        HttpResponseMessage response = await Client.DeleteAsync(url + $"/{id}");

        //Return response
        await DeserializeObject(response);
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`DELETE /v2/odsInstanceContexts/{id}`

<h3 id="deletes-an-existing-odsinstancecontext-using-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

<h3 id="deletes-an-existing-odsinstancecontext-using-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Resource was successfully deleted.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-claimsets">ClaimSets</h1>

## Retrieves a specific claimSet based on the identifier.

> Code samples

```http
GET /v2/claimSets/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/claimSets/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/claimSets/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/claimSets/{id}`

<h3 id="retrieves-a-specific-claimset-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "name": "string",
  "_isSystemReserved": true,
  "_applications": [
    {
      "applicationName": "string"
    }
  ],
  "resourceClaims": [
    {
      "id": 0,
      "name": "string",
      "actions": [
        {
          "name": "string",
          "enabled": true
        }
      ],
      "_defaultAuthorizationStrategiesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "authorizationStrategyOverridesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "children": [
        {}
      ]
    }
  ]
}
```

<h3 id="retrieves-a-specific-claimset-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetDetailsModel](#schemaedfi.ods.adminapi.features.claimsets.claimsetdetailsmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves all claimSets.

> Code samples

```http
GET /v2/claimSets HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/claimSets', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/claimSets";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/claimSets`

<h3 id="retrieves-all-claimsets.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|
|orderBy|query|string|false|Indicates the property name by which the results will be sorted.|
|direction|query|string|false|Indicates whether the result should be sorted in descending order (DESC) or ascending order (ASC).|
|id|query|integer(int32)|false|Claim set id|
|name|query|string|false|Claim set name|

#### Enumerated Values

|Parameter|Value|
|---|---|
|direction|Ascending|
|direction|Descending|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "name": "string",
    "_isSystemReserved": true,
    "_applications": [
      {
        "applicationName": "string"
      }
    ]
  }
]
```

<h3 id="retrieves-all-claimsets.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-claimsets.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetModel](#schemaedfi.ods.adminapi.features.claimsets.claimsetmodel)]|false|none|none|
|» ClaimSet|[EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetModel](#schemaedfi.ods.adminapi.features.claimsets.claimsetmodel)|false|none|none|
|»» id|integer(int32)|true|none|none|
|»» name|string¦null|true|none|none|
|»» _isSystemReserved|boolean|false|read-only|none|
|»» _applications|[[EdFi.Ods.AdminApi.Features.Applications.SimpleApplicationModel](#schemaedfi.ods.adminapi.features.applications.simpleapplicationmodel)]¦null|false|read-only|none|
|»»» Application|[EdFi.Ods.AdminApi.Features.Applications.SimpleApplicationModel](#schemaedfi.ods.adminapi.features.applications.simpleapplicationmodel)|false|none|none|
|»»»» applicationName|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Creates claimSet based on the supplied values.

> Code samples

```http
POST /v2/claimSets/{claimSetId}/resourceClaimActions HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/v2/claimSets/{claimSetId}/resourceClaimActions', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/v2/claimSets/{claimSetId}/resourceClaimActions";
      
      string json = @"{
  ""resourceClaimId"": 0,
  ""resourceClaimActions"": [
    {
      ""name"": ""string"",
      ""enabled"": true
    }
  ]
}";
      EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.AddResourceClaimOnClaimSetRequest content = JsonConvert.DeserializeObject(json);
      await PostAsync(content, url);
      
      
    }

    /// Performs a POST Request
    public async Task PostAsync(EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.AddResourceClaimOnClaimSetRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.AddResourceClaimOnClaimSetRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`POST /v2/claimSets/{claimSetId}/resourceClaimActions`

> Body parameter

```json
{
  "resourceClaimId": 0,
  "resourceClaimActions": [
    {
      "name": "string",
      "enabled": true
    }
  ]
}
```

<h3 id="creates-claimset-based-on-the-supplied-values.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|claimSetId|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.AddResourceClaimOnClaimSetRequest](#schemaedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.addresourceclaimonclaimsetrequest)|true|none|

<h3 id="creates-claimset-based-on-the-supplied-values.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Updates claimSet based on the resource identifier.

> Code samples

```http
PUT /v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId} HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}";

      
      string json = @"{
  ""resourceClaimActions"": [
    {
      ""name"": ""string"",
      ""enabled"": true
    }
  ]
}";
      EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.EditResourceClaimOnClaimSetRequest content = JsonConvert.DeserializeObject(json);
      var result = await PutAsync(id, content, url);
      
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.EditResourceClaimOnClaimSetRequest content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.EditResourceClaimOnClaimSetRequest content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}`

> Body parameter

```json
{
  "resourceClaimActions": [
    {
      "name": "string",
      "enabled": true
    }
  ]
}
```

<h3 id="updates-claimset-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|claimSetId|path|integer(int32)|true|none|
|resourceClaimId|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.EditResourceClaimOnClaimSetRequest](#schemaedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.editresourceclaimonclaimsetrequest)|true|none|

<h3 id="updates-claimset-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Deletes an existing claimSet using the resource identifier.

> Code samples

```http
DELETE /v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId} HTTP/1.1

```

```python
import requests
headers = {
  'Authorization': 'Bearer {access-token}'
}

r = requests.delete('/v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    
    /// Make a dummy request
    public async Task MakeDeleteRequest()
    {
      int id = 1;
      string url = "/v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}";

      await DeleteAsync(id, url);
    }

    /// Performs a DELETE Request
    public async Task DeleteAsync(int id, string url)
    {
        //Execute DELETE request
        HttpResponseMessage response = await Client.DeleteAsync(url + $"/{id}");

        //Return response
        await DeserializeObject(response);
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`DELETE /v2/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}`

<h3 id="deletes-an-existing-claimset-using-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|claimSetId|path|integer(int32)|true|none|
|resourceClaimId|path|integer(int32)|true|none|

<h3 id="deletes-an-existing-claimset-using-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-authorizationstrategies">AuthorizationStrategies</h1>

## Retrieves all authorizationStrategies.

> Code samples

```http
GET /v2/authorizationStrategies HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/authorizationStrategies', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/authorizationStrategies";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/authorizationStrategies`

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "name": "string",
    "displayName": "string"
  }
]
```

<h3 id="retrieves-all-authorizationstrategies.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-authorizationstrategies.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.AuthorizationStrategies.AuthorizationStrategyModel](#schemaedfi.ods.adminapi.features.authorizationstrategies.authorizationstrategymodel)]|false|none|none|
|» AuthorizationStrategy|[EdFi.Ods.AdminApi.Features.AuthorizationStrategies.AuthorizationStrategyModel](#schemaedfi.ods.adminapi.features.authorizationstrategies.authorizationstrategymodel)|false|none|none|
|»» id|integer(int32)|false|none|none|
|»» name|string¦null|false|none|none|
|»» displayName|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-applications">Applications</h1>

## Retrieves all applications.

> Code samples

```http
GET /v2/applications?offset=0&limit=25 HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/applications', params={
  'offset': '0',  'limit': '25'
}, headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/applications";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/applications`

<h3 id="retrieves-all-applications.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|true|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|true|Indicates the maximum number of items that should be returned in the results.|
|orderBy|query|string|false|Indicates the property name by which the results will be sorted.|
|direction|query|string|false|Indicates whether the result should be sorted in descending order (DESC) or ascending order (ASC).|
|id|query|integer(int32)|false|Application id|
|applicationName|query|string|false|Application name|
|claimsetName|query|string|false|Claim set name|

#### Enumerated Values

|Parameter|Value|
|---|---|
|direction|Ascending|
|direction|Descending|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "applicationName": "string",
    "claimSetName": "string",
    "educationOrganizationIds": [
      0
    ],
    "vendorId": 0,
    "profileIds": [
      0
    ],
    "odsInstanceIds": [
      0
    ]
  }
]
```

<h3 id="retrieves-all-applications.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-applications.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.Applications.ApplicationModel](#schemaedfi.ods.adminapi.features.applications.applicationmodel)]|false|none|none|
|» Application|[EdFi.Ods.AdminApi.Features.Applications.ApplicationModel](#schemaedfi.ods.adminapi.features.applications.applicationmodel)|false|none|none|
|»» id|integer(int32)|true|none|none|
|»» applicationName|string¦null|true|none|none|
|»» claimSetName|string¦null|true|none|none|
|»» educationOrganizationIds|[integer]¦null|true|none|none|
|»» vendorId|integer(int32)¦null|true|none|none|
|»» profileIds|[integer]¦null|true|none|none|
|»» odsInstanceIds|[integer]¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Creates application based on the supplied values.

> Code samples

```http
POST /v2/applications HTTP/1.1

Content-Type: application/json
Accept: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/v2/applications', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/v2/applications";
      
      string json = @"{
  ""applicationName"": ""string"",
  ""vendorId"": 0,
  ""claimSetName"": ""string"",
  ""profileIds"": [
    0
  ],
  ""educationOrganizationIds"": [
    0
  ],
  ""odsInstanceIds"": [
    0
  ]
}";
      EdFi.Ods.AdminApi.Features.Applications.AddApplication.Request content = JsonConvert.DeserializeObject(json);
      await PostAsync(content, url);
      
      
    }

    /// Performs a POST Request
    public async Task PostAsync(EdFi.Ods.AdminApi.Features.Applications.AddApplication.Request content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.Applications.AddApplication.Request content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`POST /v2/applications`

> Body parameter

```json
{
  "applicationName": "string",
  "vendorId": 0,
  "claimSetName": "string",
  "profileIds": [
    0
  ],
  "educationOrganizationIds": [
    0
  ],
  "odsInstanceIds": [
    0
  ]
}
```

<h3 id="creates-application-based-on-the-supplied-values.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[EdFi.Ods.AdminApi.Features.Applications.AddApplication.Request](#schemaedfi.ods.adminapi.features.applications.addapplication.request)|true|none|

> Example responses

> 201 Response

```json
{
  "id": 0,
  "key": "string",
  "secret": "string"
}
```

<h3 id="creates-application-based-on-the-supplied-values.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|201|[Created](https://tools.ietf.org/html/rfc7231#section-6.3.2)|Created|[EdFi.Ods.AdminApi.Features.Applications.ApplicationResult](#schemaedfi.ods.adminapi.features.applications.applicationresult)|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Retrieves a specific application based on the identifier.

> Code samples

```http
GET /v2/applications/{id} HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/applications/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/applications/{id}";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/applications/{id}`

<h3 id="retrieves-a-specific-application-based-on-the-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "applicationName": "string",
  "claimSetName": "string",
  "educationOrganizationIds": [
    0
  ],
  "vendorId": 0,
  "profileIds": [
    0
  ],
  "odsInstanceIds": [
    0
  ]
}
```

<h3 id="retrieves-a-specific-application-based-on-the-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.Applications.ApplicationModel](#schemaedfi.ods.adminapi.features.applications.applicationmodel)|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Updates application based on the resource identifier.

> Code samples

```http
PUT /v2/applications/{id} HTTP/1.1

Content-Type: application/json

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/applications/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/applications/{id}";

      
      string json = @"{
  ""applicationName"": ""string"",
  ""vendorId"": 0,
  ""claimSetName"": ""string"",
  ""profileIds"": [
    0
  ],
  ""educationOrganizationIds"": [
    0
  ],
  ""odsInstanceIds"": [
    0
  ]
}";
      EdFi.Ods.AdminApi.Features.Applications.EditApplication.Request content = JsonConvert.DeserializeObject(json);
      var result = await PutAsync(id, content, url);
      
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, EdFi.Ods.AdminApi.Features.Applications.EditApplication.Request content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(EdFi.Ods.AdminApi.Features.Applications.EditApplication.Request content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/applications/{id}`

> Body parameter

```json
{
  "applicationName": "string",
  "vendorId": 0,
  "claimSetName": "string",
  "profileIds": [
    0
  ],
  "educationOrganizationIds": [
    0
  ],
  "odsInstanceIds": [
    0
  ]
}
```

<h3 id="updates-application-based-on-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[EdFi.Ods.AdminApi.Features.Applications.EditApplication.Request](#schemaedfi.ods.adminapi.features.applications.editapplication.request)|true|none|

<h3 id="updates-application-based-on-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|None|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Deletes an existing application using the resource identifier.

> Code samples

```http
DELETE /v2/applications/{id} HTTP/1.1

```

```python
import requests
headers = {
  'Authorization': 'Bearer {access-token}'
}

r = requests.delete('/v2/applications/{id}', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    
    /// Make a dummy request
    public async Task MakeDeleteRequest()
    {
      int id = 1;
      string url = "/v2/applications/{id}";

      await DeleteAsync(id, url);
    }

    /// Performs a DELETE Request
    public async Task DeleteAsync(int id, string url)
    {
        //Execute DELETE request
        HttpResponseMessage response = await Client.DeleteAsync(url + $"/{id}");

        //Return response
        await DeserializeObject(response);
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`DELETE /v2/applications/{id}`

<h3 id="deletes-an-existing-application-using-the-resource-identifier.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

<h3 id="deletes-an-existing-application-using-the-resource-identifier.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Resource was successfully deleted.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

## Reset application credentials. Returns new key and secret.

> Code samples

```http
PUT /v2/applications/{id}/reset-credential HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.put('/v2/applications/{id}/reset-credential', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    
    /// Make a dummy request
    public async Task MakePutRequest()
    {
      int id = 1;
      string url = "/v2/applications/{id}/reset-credential";

      
      
      var result = await PutAsync(id, null, url);
          
    }

    /// Performs a PUT Request
    public async Task PutAsync(int id, undefined content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute PUT request
        HttpResponseMessage response = await Client.PutAsync(url + $"/{id}", jsonContent);

        //Return response
        return await DeserializeObject(response);
    }
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(undefined content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`PUT /v2/applications/{id}/reset-credential`

<h3 id="reset-application-credentials.-returns-new-key-and-secret.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{
  "id": 0,
  "key": "string",
  "secret": "string"
}
```

<h3 id="reset-application-credentials.-returns-new-key-and-secret.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.Applications.ApplicationResult](#schemaedfi.ods.adminapi.features.applications.applicationresult)|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request. The request was invalid and cannot be completed. See the response body for details.|None|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not found. A resource with given identifier could not be found.|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-actions">Actions</h1>

## Retrieves all actions.

> Code samples

```http
GET /v2/actions HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/v2/actions', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/v2/actions";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

`GET /v2/actions`

<h3 id="retrieves-all-actions.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|offset|query|integer(int32)|false|Indicates how many items should be skipped before returning results.|
|limit|query|integer(int32)|false|Indicates the maximum number of items that should be returned in the results.|
|orderBy|query|string|false|Indicates the property name by which the results will be sorted.|
|direction|query|string|false|Indicates whether the result should be sorted in descending order (DESC) or ascending order (ASC).|
|id|query|integer(int32)|false|Action id|
|name|query|string|false|Action name|

#### Enumerated Values

|Parameter|Value|
|---|---|
|direction|Ascending|
|direction|Descending|

> Example responses

> 200 Response

```json
[
  {
    "id": 0,
    "name": "string",
    "uri": "string"
  }
]
```

<h3 id="retrieves-all-actions.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|Inline|
|401|[Unauthorized](https://tools.ietf.org/html/rfc7235#section-3.1)|Unauthorized. The request requires authentication|None|
|403|[Forbidden](https://tools.ietf.org/html/rfc7231#section-6.5.3)|Forbidden. The request is authenticated, but not authorized to access this resource|None|
|409|[Conflict](https://tools.ietf.org/html/rfc7231#section-6.5.8)|Conflict. The request is authenticated, but it has a conflict with an existing element|None|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|None|

<h3 id="retrieves-all-actions.-responseschema">Response Schema</h3>

Status Code **200**

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|[[EdFi.Ods.AdminApi.Features.Actions.ActionModel](#schemaedfi.ods.adminapi.features.actions.actionmodel)]|false|none|none|
|» Action|[EdFi.Ods.AdminApi.Features.Actions.ActionModel](#schemaedfi.ods.adminapi.features.actions.actionmodel)|false|none|none|
|»» id|integer(int32)|true|none|none|
|»» name|string¦null|true|none|none|
|»» uri|string¦null|true|none|none|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-information">Information</h1>

## Retrieve API informational metadata

> Code samples

```http
GET / HTTP/1.1

Accept: application/json

```

```python
import requests
headers = {
  'Accept': 'application/json',
  'Authorization': 'Bearer {access-token}'
}

r = requests.get('/', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    /// Make a dummy request
    public async Task MakeGetRequest()
    {
      string url = "/";
      var result = await GetAsync(url);
    }

    /// Performs a GET Request
    public async Task GetAsync(string url)
    {
        //Start the request
        HttpResponseMessage response = await Client.GetAsync(url);

        //Validate result
        response.EnsureSuccessStatusCode();

    }
    
    
    
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

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
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|Success|[EdFi.Ods.AdminApi.Features.Information.InformationResult](#schemaedfi.ods.adminapi.features.information.informationresult)|
|500|[Internal Server Error](https://tools.ietf.org/html/rfc7231#section-6.6.1)|Internal server error. An unhandled error occurred on the server. See the response body for details.|[EdFi.Ods.AdminApi.Features.Information.InformationResult](#schemaedfi.ods.adminapi.features.information.informationresult)|

<aside class="warning">
To perform this operation, you must be authenticated by means of one of the following methods:
oauth ( Scopes: api )
</aside>

<h1 id="admin-api-documentation-connect">Connect</h1>

## Registers new client

> Code samples

```http
POST /connect/register HTTP/1.1

Content-Type: application/x-www-form-urlencoded

```

```python
import requests
headers = {
  'Content-Type': 'application/x-www-form-urlencoded',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/connect/register', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/connect/register";
      
      
      await PostAsync(null, url);
      
    }

    /// Performs a POST Request
    public async Task PostAsync(undefined content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(undefined content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

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

> Code samples

```http
POST /connect/token HTTP/1.1

Content-Type: application/x-www-form-urlencoded

```

```python
import requests
headers = {
  'Content-Type': 'application/x-www-form-urlencoded',
  'Authorization': 'Bearer {access-token}'
}

r = requests.post('/connect/token', headers = headers)

print(r.json())

```

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <<summary>>
/// Example of Http Client
/// <</summary>>
public class HttpExample
{
    private HttpClient Client { get; set; }

    /// <<summary>>
    /// Setup http client
    /// <</summary>>
    public HttpExample()
    {
      Client = new HttpClient();
    }
    
    
    /// Make a dummy request
    public async Task MakePostRequest()
    {
      string url = "/connect/token";
      
      
      await PostAsync(null, url);
      
    }

    /// Performs a POST Request
    public async Task PostAsync(undefined content, string url)
    {
        //Serialize Object
        StringContent jsonContent = SerializeObject(content);

        //Execute POST request
        HttpResponseMessage response = await Client.PostAsync(url, jsonContent);
    }
    
    
    
    /// Serialize an object to Json
    private StringContent SerializeObject(undefined content)
    {
        //Serialize Object
        string jsonObject = JsonConvert.SerializeObject(content);

        //Create Json UTF8 String Content
        return new StringContent(jsonObject, Encoding.UTF8, "application/json");
    }
    
    /// Deserialize object from request response
    private async Task DeserializeObject(HttpResponseMessage response)
    {
        //Read body 
        string responseBody = await response.Content.ReadAsStringAsync();

        //Deserialize Body to object
        var result = JsonConvert.DeserializeObject(responseBody);
    }
}

```

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

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Actions.ActionModel">EdFi.Ods.AdminApi.Features.Actions.ActionModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.actions.actionmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Actions.ActionModel"></a>
<a id="tocSedfi.ods.adminapi.features.actions.actionmodel"></a>
<a id="tocsedfi.ods.adminapi.features.actions.actionmodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "uri": "string"
}

```

Action

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|none|none|
|name|string¦null|true|none|none|
|uri|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.AdminApiError">EdFi.Ods.AdminApi.Features.AdminApiError</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.adminapierror"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.AdminApiError"></a>
<a id="tocSedfi.ods.adminapi.features.adminapierror"></a>
<a id="tocsedfi.ods.adminapi.features.adminapierror"></a>

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
|title|string¦null|true|read-only|none|
|errors|[string]¦null|true|read-only|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Applications.AddApplication.Request">EdFi.Ods.AdminApi.Features.Applications.AddApplication.Request</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.applications.addapplication.request"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Applications.AddApplication.Request"></a>
<a id="tocSedfi.ods.adminapi.features.applications.addapplication.request"></a>
<a id="tocsedfi.ods.adminapi.features.applications.addapplication.request"></a>

```json
{
  "applicationName": "string",
  "vendorId": 0,
  "claimSetName": "string",
  "profileIds": [
    0
  ],
  "educationOrganizationIds": [
    0
  ],
  "odsInstanceIds": [
    0
  ]
}

```

AddApplicationRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|applicationName|string|true|none|Application name|
|vendorId|integer(int32)|true|none|Vendor/ company id|
|claimSetName|string|true|none|Claim set name|
|profileIds|[integer]¦null|false|none|Profile id|
|educationOrganizationIds|[integer]|true|none|Education organization ids|
|odsInstanceIds|[integer]|true|none|List of ODS instance id|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Applications.ApplicationModel">EdFi.Ods.AdminApi.Features.Applications.ApplicationModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.applications.applicationmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Applications.ApplicationModel"></a>
<a id="tocSedfi.ods.adminapi.features.applications.applicationmodel"></a>
<a id="tocsedfi.ods.adminapi.features.applications.applicationmodel"></a>

```json
{
  "id": 0,
  "applicationName": "string",
  "claimSetName": "string",
  "educationOrganizationIds": [
    0
  ],
  "vendorId": 0,
  "profileIds": [
    0
  ],
  "odsInstanceIds": [
    0
  ]
}

```

Application

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|none|none|
|applicationName|string¦null|true|none|none|
|claimSetName|string¦null|true|none|none|
|educationOrganizationIds|[integer]¦null|true|none|none|
|vendorId|integer(int32)¦null|true|none|none|
|profileIds|[integer]¦null|true|none|none|
|odsInstanceIds|[integer]¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Applications.ApplicationResult">EdFi.Ods.AdminApi.Features.Applications.ApplicationResult</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.applications.applicationresult"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Applications.ApplicationResult"></a>
<a id="tocSedfi.ods.adminapi.features.applications.applicationresult"></a>
<a id="tocsedfi.ods.adminapi.features.applications.applicationresult"></a>

```json
{
  "id": 0,
  "key": "string",
  "secret": "string"
}

```

ApplicationKeySecret

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|none|none|
|key|string¦null|true|none|none|
|secret|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Applications.EditApplication.Request">EdFi.Ods.AdminApi.Features.Applications.EditApplication.Request</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.applications.editapplication.request"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Applications.EditApplication.Request"></a>
<a id="tocSedfi.ods.adminapi.features.applications.editapplication.request"></a>
<a id="tocsedfi.ods.adminapi.features.applications.editapplication.request"></a>

```json
{
  "applicationName": "string",
  "vendorId": 0,
  "claimSetName": "string",
  "profileIds": [
    0
  ],
  "educationOrganizationIds": [
    0
  ],
  "odsInstanceIds": [
    0
  ]
}

```

EditApplicationRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|applicationName|string|true|none|Application name|
|vendorId|integer(int32)|true|none|Vendor/ company id|
|claimSetName|string|true|none|Claim set name|
|profileIds|[integer]¦null|false|none|Profile id|
|educationOrganizationIds|[integer]|true|none|Education organization ids|
|odsInstanceIds|[integer]|true|none|List of ODS instance id|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Applications.SimpleApplicationModel">EdFi.Ods.AdminApi.Features.Applications.SimpleApplicationModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.applications.simpleapplicationmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Applications.SimpleApplicationModel"></a>
<a id="tocSedfi.ods.adminapi.features.applications.simpleapplicationmodel"></a>
<a id="tocsedfi.ods.adminapi.features.applications.simpleapplicationmodel"></a>

```json
{
  "applicationName": "string"
}

```

Application

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|applicationName|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.AuthorizationStrategies.AuthorizationStrategyModel">EdFi.Ods.AdminApi.Features.AuthorizationStrategies.AuthorizationStrategyModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.authorizationstrategies.authorizationstrategymodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.AuthorizationStrategies.AuthorizationStrategyModel"></a>
<a id="tocSedfi.ods.adminapi.features.authorizationstrategies.authorizationstrategymodel"></a>
<a id="tocsedfi.ods.adminapi.features.authorizationstrategies.authorizationstrategymodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "displayName": "string"
}

```

AuthorizationStrategy

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|false|none|none|
|name|string¦null|false|none|none|
|displayName|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.AddClaimSet.AddClaimSetRequest">EdFi.Ods.AdminApi.Features.ClaimSets.AddClaimSet.AddClaimSetRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.addclaimset.addclaimsetrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.AddClaimSet.AddClaimSetRequest"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.addclaimset.addclaimsetrequest"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.addclaimset.addclaimsetrequest"></a>

```json
{
  "name": "string"
}

```

AddClaimSetRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string|true|none|Claim set name|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetDetailsModel">EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetDetailsModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.claimsetdetailsmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetDetailsModel"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.claimsetdetailsmodel"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.claimsetdetailsmodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "_isSystemReserved": true,
  "_applications": [
    {
      "applicationName": "string"
    }
  ],
  "resourceClaims": [
    {
      "id": 0,
      "name": "string",
      "actions": [
        {
          "name": "string",
          "enabled": true
        }
      ],
      "_defaultAuthorizationStrategiesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "authorizationStrategyOverridesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "children": [
        {}
      ]
    }
  ]
}

```

ClaimSetWithResources

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|none|none|
|name|string¦null|true|none|none|
|_isSystemReserved|boolean|false|read-only|none|
|_applications|[[EdFi.Ods.AdminApi.Features.Applications.SimpleApplicationModel](#schemaedfi.ods.adminapi.features.applications.simpleapplicationmodel)]¦null|false|read-only|none|
|resourceClaims|[[EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.claimsetresourceclaimmodel)]¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetModel">EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.claimsetmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetModel"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.claimsetmodel"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.claimsetmodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "_isSystemReserved": true,
  "_applications": [
    {
      "applicationName": "string"
    }
  ]
}

```

ClaimSet

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|none|none|
|name|string¦null|true|none|none|
|_isSystemReserved|boolean|false|read-only|none|
|_applications|[[EdFi.Ods.AdminApi.Features.Applications.SimpleApplicationModel](#schemaedfi.ods.adminapi.features.applications.simpleapplicationmodel)]¦null|false|read-only|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetResourceClaimModel">EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetResourceClaimModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.claimsetresourceclaimmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetResourceClaimModel"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.claimsetresourceclaimmodel"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.claimsetresourceclaimmodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "actions": [
    {
      "name": "string",
      "enabled": true
    }
  ],
  "_defaultAuthorizationStrategiesForCRUD": [
    {
      "actionId": 0,
      "actionName": "string",
      "authorizationStrategies": [
        {
          "authStrategyId": 0,
          "authStrategyName": "string",
          "isInheritedFromParent": true
        }
      ]
    }
  ],
  "authorizationStrategyOverridesForCRUD": [
    {
      "actionId": 0,
      "actionName": "string",
      "authorizationStrategies": [
        {
          "authStrategyId": 0,
          "authStrategyName": "string",
          "isInheritedFromParent": true
        }
      ]
    }
  ],
  "children": [
    {
      "id": 0,
      "name": "string",
      "actions": [
        {
          "name": "string",
          "enabled": true
        }
      ],
      "_defaultAuthorizationStrategiesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "authorizationStrategyOverridesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "children": []
    }
  ]
}

```

ClaimSetResourceClaim

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|read-only|none|
|name|string¦null|true|none|none|
|actions|[[EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ResourceClaimAction](#schemaedfi.ods.adminapi.infrastructure.claimseteditor.resourceclaimaction)]¦null|true|none|none|
|_defaultAuthorizationStrategiesForCRUD|[[EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSetResourceClaimActionAuthStrategies](#schemaedfi.ods.adminapi.infrastructure.claimseteditor.claimsetresourceclaimactionauthstrategies)]¦null|false|read-only|none|
|authorizationStrategyOverridesForCRUD|[[EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSetResourceClaimActionAuthStrategies](#schemaedfi.ods.adminapi.infrastructure.claimseteditor.claimsetresourceclaimactionauthstrategies)]¦null|true|none|none|
|children|[[EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.claimsetresourceclaimmodel)]¦null|true|none|Children are collection of ResourceClaim|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.CopyClaimSet.CopyClaimSetRequest">EdFi.Ods.AdminApi.Features.ClaimSets.CopyClaimSet.CopyClaimSetRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.copyclaimset.copyclaimsetrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.CopyClaimSet.CopyClaimSetRequest"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.copyclaimset.copyclaimsetrequest"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.copyclaimset.copyclaimsetrequest"></a>

```json
{
  "originalId": 0,
  "name": "string"
}

```

CopyClaimSetRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|originalId|integer(int32)|true|none|ClaimSet id to copy|
|name|string|true|none|New claimset name|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.EditClaimSet.EditClaimSetRequest">EdFi.Ods.AdminApi.Features.ClaimSets.EditClaimSet.EditClaimSetRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.editclaimset.editclaimsetrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.EditClaimSet.EditClaimSetRequest"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.editclaimset.editclaimsetrequest"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.editclaimset.editclaimsetrequest"></a>

```json
{
  "name": "string"
}

```

EditClaimSetRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string|true|none|Claim set name|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ImportClaimSet.Request">EdFi.Ods.AdminApi.Features.ClaimSets.ImportClaimSet.Request</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.importclaimset.request"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ImportClaimSet.Request"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.importclaimset.request"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.importclaimset.request"></a>

```json
{
  "name": "string",
  "resourceClaims": [
    {
      "id": 0,
      "name": "string",
      "actions": [
        {
          "name": "string",
          "enabled": true
        }
      ],
      "_defaultAuthorizationStrategiesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "authorizationStrategyOverridesForCRUD": [
        {
          "actionId": 0,
          "actionName": "string",
          "authorizationStrategies": [
            {
              "authStrategyId": 0,
              "authStrategyName": "string",
              "isInheritedFromParent": true
            }
          ]
        }
      ],
      "children": [
        {}
      ]
    }
  ]
}

```

ImportClaimSetRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string|true|none|Claim set name|
|resourceClaims|[[EdFi.Ods.AdminApi.Features.ClaimSets.ClaimSetResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.claimsetresourceclaimmodel)]|true|none|Resource Claims|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel">EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.resourceclaimmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.resourceclaimmodel"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.resourceclaimmodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "parentId": 0,
  "parentName": "string",
  "children": [
    {
      "id": 0,
      "name": "string",
      "parentId": 0,
      "parentName": "string",
      "children": []
    }
  ]
}

```

ResourceClaimModel

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|none|none|
|name|string¦null|true|none|none|
|parentId|integer(int32)¦null|true|none|none|
|parentName|string¦null|true|none|none|
|children|[[EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaimModel](#schemaedfi.ods.adminapi.features.claimsets.resourceclaimmodel)]¦null|true|none|Children are collection of SimpleResourceClaimModel|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditAuthStrategy.OverrideAuthStategyOnClaimSetRequest">EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditAuthStrategy.OverrideAuthStategyOnClaimSetRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.resourceclaims.editauthstrategy.overrideauthstategyonclaimsetrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditAuthStrategy.OverrideAuthStategyOnClaimSetRequest"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.resourceclaims.editauthstrategy.overrideauthstategyonclaimsetrequest"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.resourceclaims.editauthstrategy.overrideauthstategyonclaimsetrequest"></a>

```json
{
  "actionName": "string",
  "authorizationStrategies": [
    "string"
  ]
}

```

OverrideAuthStategyOnClaimSetRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|actionName|string¦null|true|none|none|
|authorizationStrategies|[string]|true|none|AuthorizationStrategy Names|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.AddResourceClaimOnClaimSetRequest">EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.AddResourceClaimOnClaimSetRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.addresourceclaimonclaimsetrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.AddResourceClaimOnClaimSetRequest"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.addresourceclaimonclaimsetrequest"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.addresourceclaimonclaimsetrequest"></a>

```json
{
  "resourceClaimId": 0,
  "resourceClaimActions": [
    {
      "name": "string",
      "enabled": true
    }
  ]
}

```

AddResourceClaimActionsOnClaimSetRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|resourceClaimId|integer(int32)|true|none|ResourceClaim id|
|resourceClaimActions|[[EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ResourceClaimAction](#schemaedfi.ods.adminapi.infrastructure.claimseteditor.resourceclaimaction)]|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.EditResourceClaimOnClaimSetRequest">EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.EditResourceClaimOnClaimSetRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.editresourceclaimonclaimsetrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims.EditResourceClaimActions.EditResourceClaimOnClaimSetRequest"></a>
<a id="tocSedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.editresourceclaimonclaimsetrequest"></a>
<a id="tocsedfi.ods.adminapi.features.claimsets.resourceclaims.editresourceclaimactions.editresourceclaimonclaimsetrequest"></a>

```json
{
  "resourceClaimActions": [
    {
      "name": "string",
      "enabled": true
    }
  ]
}

```

EditResourceClaimActionsOnClaimSetRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|resourceClaimActions|[[EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ResourceClaimAction](#schemaedfi.ods.adminapi.infrastructure.claimseteditor.resourceclaimaction)]|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Connect.RegisterService.Request">EdFi.Ods.AdminApi.Features.Connect.RegisterService.Request</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.connect.registerservice.request"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Connect.RegisterService.Request"></a>
<a id="tocSedfi.ods.adminapi.features.connect.registerservice.request"></a>
<a id="tocsedfi.ods.adminapi.features.connect.registerservice.request"></a>

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
|clientId|string|true|none|Client id|
|clientSecret|string|true|none|Client secret|
|displayName|string|true|none|Client display name|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Information.InformationResult">EdFi.Ods.AdminApi.Features.Information.InformationResult</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.information.informationresult"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Information.InformationResult"></a>
<a id="tocSedfi.ods.adminapi.features.information.informationresult"></a>
<a id="tocsedfi.ods.adminapi.features.information.informationresult"></a>

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
|version|string|true|none|Application version|
|build|string|true|none|Build / release version|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceDetailModel">EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceDetailModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstances.odsinstancedetailmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceDetailModel"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstances.odsinstancedetailmodel"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstances.odsinstancedetailmodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "instanceType": "string",
  "odsInstanceContexts": [
    {
      "id": 0,
      "odsInstanceId": 0,
      "contextKey": "string",
      "contextValue": "string"
    }
  ],
  "odsInstanceDerivatives": [
    {
      "id": 0,
      "odsInstanceId": 0,
      "derivativeType": "string"
    }
  ]
}

```

OdsInstanceDetail

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|false|none|none|
|name|string¦null|true|none|none|
|instanceType|string¦null|true|none|none|
|odsInstanceContexts|[[EdFi.Ods.AdminApi.Features.OdsInstanceContext.OdsInstanceContextModel](#schemaedfi.ods.adminapi.features.odsinstancecontext.odsinstancecontextmodel)]¦null|true|none|none|
|odsInstanceDerivatives|[[EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.OdsInstanceDerivativeModel](#schemaedfi.ods.adminapi.features.odsinstancederivative.odsinstancederivativemodel)]¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceModel">EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstances.odsinstancemodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.ODSInstances.OdsInstanceModel"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstances.odsinstancemodel"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstances.odsinstancemodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "instanceType": "string"
}

```

OdsInstance

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|false|none|none|
|name|string¦null|true|none|none|
|instanceType|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstanceContext.AddOdsInstanceContext.AddOdsInstanceContextRequest">EdFi.Ods.AdminApi.Features.OdsInstanceContext.AddOdsInstanceContext.AddOdsInstanceContextRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstancecontext.addodsinstancecontext.addodsinstancecontextrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstanceContext.AddOdsInstanceContext.AddOdsInstanceContextRequest"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstancecontext.addodsinstancecontext.addodsinstancecontextrequest"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstancecontext.addodsinstancecontext.addodsinstancecontextrequest"></a>

```json
{
  "odsInstanceId": 0,
  "contextKey": "string",
  "contextValue": "string"
}

```

AddOdsInstanceContextRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|odsInstanceId|integer(int32)|true|none|ODS instance context ODS instance id.|
|contextKey|string|true|none|context key.|
|contextValue|string|true|none|context value.|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstanceContext.EditOdsInstanceContext.EditOdsInstanceContextRequest">EdFi.Ods.AdminApi.Features.OdsInstanceContext.EditOdsInstanceContext.EditOdsInstanceContextRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstancecontext.editodsinstancecontext.editodsinstancecontextrequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstanceContext.EditOdsInstanceContext.EditOdsInstanceContextRequest"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstancecontext.editodsinstancecontext.editodsinstancecontextrequest"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstancecontext.editodsinstancecontext.editodsinstancecontextrequest"></a>

```json
{
  "odsInstanceId": 0,
  "contextKey": "string",
  "contextValue": "string"
}

```

EditOdsInstanceContextRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|odsInstanceId|integer(int32)|true|none|ODS instance context ODS instance id.|
|contextKey|string|true|none|context key.|
|contextValue|string|true|none|context value.|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstanceContext.OdsInstanceContextModel">EdFi.Ods.AdminApi.Features.OdsInstanceContext.OdsInstanceContextModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstancecontext.odsinstancecontextmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstanceContext.OdsInstanceContextModel"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstancecontext.odsinstancecontextmodel"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstancecontext.odsinstancecontextmodel"></a>

```json
{
  "id": 0,
  "odsInstanceId": 0,
  "contextKey": "string",
  "contextValue": "string"
}

```

OdsInstanceContext

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|false|none|none|
|odsInstanceId|integer(int32)|true|none|none|
|contextKey|string¦null|true|none|none|
|contextValue|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.AddOdsInstanceDerivative.AddOdsInstanceDerivativeRequest">EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.AddOdsInstanceDerivative.AddOdsInstanceDerivativeRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstancederivative.addodsinstancederivative.addodsinstancederivativerequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.AddOdsInstanceDerivative.AddOdsInstanceDerivativeRequest"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstancederivative.addodsinstancederivative.addodsinstancederivativerequest"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstancederivative.addodsinstancederivative.addodsinstancederivativerequest"></a>

```json
{
  "odsInstanceId": 0,
  "derivativeType": "string",
  "connectionString": "string"
}

```

AddOdsInstanceDerivativeRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|odsInstanceId|integer(int32)|true|none|ODS instance derivative ODS instance id.|
|derivativeType|string|true|none|derivative type.|
|connectionString|string|true|none|connection string.|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.EditOdsInstanceDerivative.EditOdsInstanceDerivativeRequest">EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.EditOdsInstanceDerivative.EditOdsInstanceDerivativeRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstancederivative.editodsinstancederivative.editodsinstancederivativerequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.EditOdsInstanceDerivative.EditOdsInstanceDerivativeRequest"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstancederivative.editodsinstancederivative.editodsinstancederivativerequest"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstancederivative.editodsinstancederivative.editodsinstancederivativerequest"></a>

```json
{
  "odsInstanceId": 0,
  "derivativeType": "string",
  "connectionString": "string"
}

```

EditOdsInstanceDerivativeRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|odsInstanceId|integer(int32)|true|none|ODS instance derivative ODS instance id.|
|derivativeType|string|true|none|derivative type.|
|connectionString|string|true|none|connection string.|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.OdsInstanceDerivativeModel">EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.OdsInstanceDerivativeModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstancederivative.odsinstancederivativemodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstanceDerivative.OdsInstanceDerivativeModel"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstancederivative.odsinstancederivativemodel"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstancederivative.odsinstancederivativemodel"></a>

```json
{
  "id": 0,
  "odsInstanceId": 0,
  "derivativeType": "string"
}

```

OdsInstanceDerivative

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)|true|none|none|
|odsInstanceId|integer(int32)¦null|true|none|none|
|derivativeType|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstances.AddOdsInstance.AddOdsInstanceRequest">EdFi.Ods.AdminApi.Features.OdsInstances.AddOdsInstance.AddOdsInstanceRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstances.addodsinstance.addodsinstancerequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstances.AddOdsInstance.AddOdsInstanceRequest"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstances.addodsinstance.addodsinstancerequest"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstances.addodsinstance.addodsinstancerequest"></a>

```json
{
  "name": "string",
  "instanceType": "string",
  "connectionString": "string"
}

```

AddOdsInstanceRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string|true|none|Ods Instance name|
|instanceType|string|true|none|Ods Instance type|
|connectionString|string|true|none|Ods Instance connection string|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.OdsInstances.EditOdsInstance.EditOdsInstanceRequest">EdFi.Ods.AdminApi.Features.OdsInstances.EditOdsInstance.EditOdsInstanceRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.odsinstances.editodsinstance.editodsinstancerequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.OdsInstances.EditOdsInstance.EditOdsInstanceRequest"></a>
<a id="tocSedfi.ods.adminapi.features.odsinstances.editodsinstance.editodsinstancerequest"></a>
<a id="tocsedfi.ods.adminapi.features.odsinstances.editodsinstance.editodsinstancerequest"></a>

```json
{
  "name": "string",
  "instanceType": "string",
  "connectionString": "string"
}

```

EditOdsInstanceRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string|true|none|Ods Instance name|
|instanceType|string|true|none|Ods Instance type|
|connectionString|string¦null|true|none|Ods Instance connection string|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Profiles.AddProfile.AddProfileRequest">EdFi.Ods.AdminApi.Features.Profiles.AddProfile.AddProfileRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.profiles.addprofile.addprofilerequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Profiles.AddProfile.AddProfileRequest"></a>
<a id="tocSedfi.ods.adminapi.features.profiles.addprofile.addprofilerequest"></a>
<a id="tocsedfi.ods.adminapi.features.profiles.addprofile.addprofilerequest"></a>

```json
"{\r\n  \"name\": \"Test-Profile\",\r\n  \"definition\": \"<Profile name=\\\"Test-Profile\\\"><Resource name=\\\"Resource1\\\"><ReadContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection1\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></ReadContentType><WriteContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection2\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></WriteContentType></Resource></Profile>\"\r\n}"

```

AddProfileRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string|true|none|Profile name|
|definition|string|true|none|Profile definition|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Profiles.EditProfile.EditProfileRequest">EdFi.Ods.AdminApi.Features.Profiles.EditProfile.EditProfileRequest</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.profiles.editprofile.editprofilerequest"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Profiles.EditProfile.EditProfileRequest"></a>
<a id="tocSedfi.ods.adminapi.features.profiles.editprofile.editprofilerequest"></a>
<a id="tocsedfi.ods.adminapi.features.profiles.editprofile.editprofilerequest"></a>

```json
"{\r\n  \"name\": \"Test-Profile\",\r\n  \"definition\": \"<Profile name=\\\"Test-Profile\\\"><Resource name=\\\"Resource1\\\"><ReadContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection1\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></ReadContentType><WriteContentType memberSelection=\\\"IncludeOnly\\\"><Collection name=\\\"Collection2\\\" memberSelection=\\\"IncludeOnly\\\"><Property name=\\\"Property1\\\" /><Property name=\\\"Property2\\\" /></Collection></WriteContentType></Resource></Profile>\"\r\n}"

```

EditProfileRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string|true|none|Profile name|
|definition|string|true|none|Profile definition|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Profiles.ProfileDetailsModel">EdFi.Ods.AdminApi.Features.Profiles.ProfileDetailsModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.profiles.profiledetailsmodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Profiles.ProfileDetailsModel"></a>
<a id="tocSedfi.ods.adminapi.features.profiles.profiledetailsmodel"></a>
<a id="tocsedfi.ods.adminapi.features.profiles.profiledetailsmodel"></a>

```json
{
  "id": 0,
  "name": "string",
  "definition": "string"
}

```

ProfileDetails

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)¦null|true|none|none|
|name|string¦null|true|none|none|
|definition|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Profiles.ProfileModel">EdFi.Ods.AdminApi.Features.Profiles.ProfileModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.profiles.profilemodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Profiles.ProfileModel"></a>
<a id="tocSedfi.ods.adminapi.features.profiles.profilemodel"></a>
<a id="tocsedfi.ods.adminapi.features.profiles.profilemodel"></a>

```json
{
  "id": 0,
  "name": "string"
}

```

Profile

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)¦null|true|none|none|
|name|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Vendors.AddVendor.Request">EdFi.Ods.AdminApi.Features.Vendors.AddVendor.Request</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.vendors.addvendor.request"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Vendors.AddVendor.Request"></a>
<a id="tocSedfi.ods.adminapi.features.vendors.addvendor.request"></a>
<a id="tocsedfi.ods.adminapi.features.vendors.addvendor.request"></a>

```json
{
  "company": "string",
  "namespacePrefixes": "string",
  "contactName": "string",
  "contactEmailAddress": "string"
}

```

AddVendorRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|company|string|true|none|Vendor/ company name|
|namespacePrefixes|string|true|none|Namespace prefix for the vendor. Multiple namespace prefixes can be provided as comma separated list if required.|
|contactName|string|true|none|Vendor contact name|
|contactEmailAddress|string|true|none|Vendor contact email id|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Vendors.EditVendor.Request">EdFi.Ods.AdminApi.Features.Vendors.EditVendor.Request</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.vendors.editvendor.request"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Vendors.EditVendor.Request"></a>
<a id="tocSedfi.ods.adminapi.features.vendors.editvendor.request"></a>
<a id="tocsedfi.ods.adminapi.features.vendors.editvendor.request"></a>

```json
{
  "company": "string",
  "namespacePrefixes": "string",
  "contactName": "string",
  "contactEmailAddress": "string"
}

```

EditVendorRequest

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|company|string|true|none|Vendor/ company name|
|namespacePrefixes|string|true|none|Namespace prefix for the vendor. Multiple namespace prefixes can be provided as comma separated list if required.|
|contactName|string|true|none|Vendor contact name|
|contactEmailAddress|string|true|none|Vendor contact email id|

<h2 id="tocS_EdFi.Ods.AdminApi.Features.Vendors.VendorModel">EdFi.Ods.AdminApi.Features.Vendors.VendorModel</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.features.vendors.vendormodel"></a>
<a id="schema_EdFi.Ods.AdminApi.Features.Vendors.VendorModel"></a>
<a id="tocSedfi.ods.adminapi.features.vendors.vendormodel"></a>
<a id="tocsedfi.ods.adminapi.features.vendors.vendormodel"></a>

```json
{
  "id": 0,
  "company": "string",
  "namespacePrefixes": "string",
  "contactName": "string",
  "contactEmailAddress": "string"
}

```

Vendor

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|id|integer(int32)¦null|true|none|none|
|company|string¦null|true|none|none|
|namespacePrefixes|string¦null|true|none|none|
|contactName|string¦null|true|none|none|
|contactEmailAddress|string¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.AuthorizationStrategy">EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.AuthorizationStrategy</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.infrastructure.claimseteditor.authorizationstrategy"></a>
<a id="schema_EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.AuthorizationStrategy"></a>
<a id="tocSedfi.ods.adminapi.infrastructure.claimseteditor.authorizationstrategy"></a>
<a id="tocsedfi.ods.adminapi.infrastructure.claimseteditor.authorizationstrategy"></a>

```json
{
  "authStrategyId": 0,
  "authStrategyName": "string",
  "isInheritedFromParent": true
}

```

ResourceClaimAuthorizationStrategy

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|authStrategyId|integer(int32)|true|none|none|
|authStrategyName|string¦null|true|none|none|
|isInheritedFromParent|boolean|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSetResourceClaimActionAuthStrategies">EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSetResourceClaimActionAuthStrategies</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.infrastructure.claimseteditor.claimsetresourceclaimactionauthstrategies"></a>
<a id="schema_EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSetResourceClaimActionAuthStrategies"></a>
<a id="tocSedfi.ods.adminapi.infrastructure.claimseteditor.claimsetresourceclaimactionauthstrategies"></a>
<a id="tocsedfi.ods.adminapi.infrastructure.claimseteditor.claimsetresourceclaimactionauthstrategies"></a>

```json
{
  "actionId": 0,
  "actionName": "string",
  "authorizationStrategies": [
    {
      "authStrategyId": 0,
      "authStrategyName": "string",
      "isInheritedFromParent": true
    }
  ]
}

```

ClaimSetResourceClaimActionAuthorizationStrategies

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|actionId|integer(int32)¦null|true|none|none|
|actionName|string¦null|true|none|none|
|authorizationStrategies|[[EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.AuthorizationStrategy](#schemaedfi.ods.adminapi.infrastructure.claimseteditor.authorizationstrategy)]¦null|true|none|none|

<h2 id="tocS_EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ResourceClaimAction">EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ResourceClaimAction</h2>
<!-- backwards compatibility -->
<a id="schemaedfi.ods.adminapi.infrastructure.claimseteditor.resourceclaimaction"></a>
<a id="schema_EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ResourceClaimAction"></a>
<a id="tocSedfi.ods.adminapi.infrastructure.claimseteditor.resourceclaimaction"></a>
<a id="tocsedfi.ods.adminapi.infrastructure.claimseteditor.resourceclaimaction"></a>

```json
{
  "name": "string",
  "enabled": true
}

```

ResourceClaimAction

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|name|string¦null|true|none|none|
|enabled|boolean|true|none|none|

