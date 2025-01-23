# Tenant Management Data

See [APIs for Admin Console: Tenant
Management](./APIS-FOR-ADMIN-CONSOLE.md#tenant-management) for context.

> [!NOTE]
> Full CRUD support with database backend will be covered in a future version.

## REST Interface

### GET /adminconsole/tenants

Also supports `GET /adminconsole/tenants/{id}`

* **Purpose**: Provide tenant list to Admin Console.
* **Description**:
  * Reads from the appsettings file.
  * No additional authorization required.
  * Respond with 200
* **Response Format**:

  ```json
  [
    {
      "tenantId": 1,
      "document": {
        "name": "Tenant1",
        "discoveryApiUrl": "http://localhost/api"
      }
    }
  ]
  ```

## Data Storage

The required values will initially be stored in the appSettings file instead of
a database table.
