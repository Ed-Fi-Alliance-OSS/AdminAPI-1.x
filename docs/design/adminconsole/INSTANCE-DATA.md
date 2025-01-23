# Instance Management Data

See [APIs for Admin Console: Tenant
Management](./APIS-FOR-ADMIN-CONSOLE.md#tenant-management) for context.

## REST Interface

> [!NOTE]
> Admin Console might not be transmitting Tenant ID in the `POST` request
> as of Jan 20. We may need to modify Admin Console to include this.

### GET /adminconsole/odsInstances

Also supports `GET /adminconsole/odsInstances/{id}`

* **Purpose**: Provide instance list to Admin Console.
* **Description**:
  * Reads from the `adminconsole.Instance` table.
  * No additional authorization required.
  * The `baseUrl` value comes from the tenant information.
  * Respond with 200
* **Response Format**:

  ```json
  [
    {
      "odsInstanceId": 1,
      "tenantId": 1,
      "name": "Instance1",
      "instanceType": "enterprise",
      "baseUrl": "http://localhost/api",
      "odsInstanceContexts": [
        {
          "id": 1,
          "odsInstanceId": 1,
          "contextKey": "schoolYearFromRoute",
          "contextValue": "2024"
        }
      ],
      "odsInstanceDerivatives": [
        {
          "id": 1,
          "odsInstanceId": 2,
          "derivativeType": "Read"
        }
      ]
    }
  ]
  ```

### POST /adminconsole/odsInstances

* **Purpose**: Accept instance creation requests from the Admin Console.
* **Description**:
  * Validate the incoming payload.
  * Insert data into the `adminconsole.Instance` table.
  * Respond with `202 Accepted` and include the new jobId created.
* **Validation**:

  | Property                             | Rules                                                      |
  | ------------------------------------ | ---------------------------------------------------------- |
  | name                                 | Max length: 100. Must be unique for the tenant.            |
  | instanceType                         | Max length: 100.                                           |
  | odsInstanceContexts                  | Empty array is allowed                                     |
  | odsInstanceContext.contextKey        | Max length: 50. Must be unique for the instance.           |
  | odsInstanceDerivatives               | Empty array is allowed                                     |
  | odsInstanceContext.contextValue      | Max length: 50.                                            |
  | odsInstanceDerivative.derivativeType | Max length: 50. Allowed values: "ReadReplica", "Snapshot". |

* **Sample Payload**:

  ```json
  {
    "odsInstanceId": 1,
    "tenantId": 1,
    "name": "Instance #1 - 2024",
    "instanceType": "enterprise",
    "odsInstanceContexts": [
      {
        "contextKey": "schoolYearFromRoute",
        "contextValue": "2024"
      }
    ],
    "odsInstanceDerivatives": [
      {
        "derivativeType": "ReadReplica"
      }
    ]
  }
  ```

* **Response Format**:

  ```json
  {
    "instanceId": 1,
  }
  ```

### PUT /adminconsole/odsInstances/{id}

* **Purpose**: Update an instance definition.
* **Description**:
  * Validate the incoming payload.
  * Updates both the `adminconsole.Instance` and the `dbo.OdsInstances` tables.
  * Respond with `202 No Content`
* **Validation**:

  | Property                             | Rules                                                      |
  | ------------------------------------ | ---------------------------------------------------------- |
  | name                                 | Max length: 100. Must be unique for the tenant.            |
  | instanceType                         | Max length: 100.                                           |
  | odsInstanceContexts                  | Empty array is allowed                                     |
  | odsInstanceContext.contextKey        | Max length: 50. Must be unique for the instance.           |
  | odsInstanceDerivatives               | Empty array is allowed                                     |
  | odsInstanceContext.contextValue      | Max length: 50.                                            |
  | odsInstanceDerivative.derivativeType | Max length: 50. Allowed values: "ReadReplica", "Snapshot". |

* **Sample Payload**:

  ```json
  {
    "odsInstanceId": 1,
    "tenantId": 1,
    "name": "Instance #1 - 2024",
    "instanceType": "enterprise",
    "odsInstanceContexts": [
      {
        "contextKey": "schoolYearFromRoute",
        "contextValue": "2024"
      }
    ],
    "odsInstanceDerivatives": [
      {
        "derivativeType": "ReadReplica"
      }
    ]
  }
  ```

### DELETE /adminconsole/odsInstances/{id}

* Not supported at this time. Respond with `405 Method Not Allowed`.

### GET /adminconsole/instances

Also supports `GET /adminconsole/instances/{id}`

* **Purpose**: Provide instance list for the worker applications.
* **Description**:
  * Reads from the `adminconsole.Instance` table.
  * Must be authorized with an appropriate Role name in the token: `clientId`
    and `clientSecret` must be left blank when this endpoint is accessed by an
    Admin Console user.
  * Returns a separate object for each ODS Instance Context.
  * The `resourceUrl` is constructed from the tenant's base URL plus instance
    context information.
  * `odsInstanceId` will be null if the `status` is not "Completed".
  * Return all values without the need for paging.
  * Respond with 200
* **Query String Parameters**:
  * `status` to search by Status.
  * `tenantName` to search by Tenant
* **Response Format**:

  ```json
  [
    {
      "tenantId": 1,
      "tenantName": "Tenant1",
      "instanceId": 1,
      "odsInstanceId": 1,
      "instanceName": "Instance #1 - 2024",
      "resourceUrl": "http://localhost/api/2024/data/v3",
      "oauthUrl": "http://localhost/api/2024/oauth/token",
      "clientId": "abc123",
      "clientSecret": "d5rftyguht67gyhuijk",
      "status": "Completed"
    }
  ]
  ```

> [!NOTE]
> In the future the health check worker should use the read replica / snapshot
> if available. For now, it will use the primary database instance.

### POST /adminconsole/instances/{id}/completed

> [!NOTE]
> In Admin API 2.4, this may be deprecated in favor of
> `/adminconsole/instances/jobs/{id}/completed`, which is described below.

* **Purpose**: Updates the given `adminconsole.Instance` record by changing the
  status to "Completed".
* **Description**:
  * Responds with `204 No Content` if the record is _already complete_ or if the operations described below succeed.
  * Responds with `404 Not Found` if the Id does not exist.
  * As described in [Instance Management Worker](./INSTANCE-MANAGEMENT.md), this
    action does the following work if the status is not already "Completed",
    using a single database transaction:
    * Insert into `dbo.OdsInstances`.
    * If needed, insert into `dbo.OdsInstanceContexts` and `dbo.OdsInstanceDerivatives`.
    * Insert into `dbo.ApiClients` to create credentials for the [Health Check Worker](./HEALTH-CHECK-WORKER.md).
    * Insert into `dbo.ApiClientOdsInstances`.
    * Update `adminconsole.Instance` to set:
      * New credentials
      * Status = "Completed"

### POST /adminconsole/instances/jobs/start

> [!NOTE]
> This endpoint might not be included in the Admin API 2.3 for Admin Console 1.
> Job support is a low priority for future consideration.

* **Purpose**: Start processing jobs from the `adminconsole.Instance` table.
* **Description**:
  * Select rows with any of these conditions:
    * Status is "Pending" and `lockDateTime is null`.
    * Status is "In Progress" and `lockDateTime` is expired (expiration timeout
      value to be set in appsettings, for example 60 minutes). _This provides an
      automated retry process_
  * Lock rows in the `adminconsole.Instance` table for processing by setting a
    `jobId` value (UUID) and setting column `lockDateTime` to "now".
  * Changes the status to `In Progress`
  * Responds with `200 OK`.
* **Response Format**:

  ```json
  {
    "jobId": "<int>",
    "instances": [
      {
        "odsInstanceId": 1,
        "tenantId": 1,
        "name": "Instance #1 - 2024",
        "instanceType": "enterprise",
        "odsInstanceContexts": [
        {
          "contextKey": "schoolYearFromRoute",
          "contextValue": "2024"
        }
        ],
        "odsInstanceDerivatives": [
        {
          "derivativeType": "ReadReplica"
        }
        ]
      }
    ]
  }
  ```

### POST /adminconsole/instances/jobs/{id}/completed

> [!NOTE]
> This endpoint might not be included in the Admin API 2.3 for Admin Console 1.
> If it is included, then `/adminconsole/instances/{id}/completed` will be removed.

* **Purpose**: Mark a job as complete and perform transactional updates.
* **Enhancements**:
  * Accept a job completion payload.
  * Add resultant data to tables `OdsInstances`, `OdsInstanceContext`, `OdsInstanceDerivatives` and update `adminconsole.Instance` status column to mark job as `Compelete` within a single transaction.
  * Roll back on failure.
  * Respond with `200 Ok`.

### PUT /adminconsole/instances/{id}

* Not supported at this time. Respond with `405 Method Not Allowed`.

### DELETE /adminconsole/instances/{id}

* Not supported at this time. Respond with `405 Method Not Allowed`.

## Data Storage

No modifications will be made in the `dbo.*` tables.

### adminconsole.Instance

In the normal flow of work, this table will be populated by Admin Console /
Admin API _before_ a matching record exists in the `dbo.OdsInstances` table. The
`Document` column shown below is a flexible JSON object to whatever information
is necessary to support both the user interface and the creation of records in
`dbo.OdsInstances`, `dbo.OdsInstanceContext`, and `dbo.OdsInstanceDerivatives`.
The JSON structure gives the team flexibility for rapid iteration.

Columns that need to be indexed (e.g. `status`) or updated by worker processes
should be real columns, instead of embedding them in the JSON data.

| Column Name   | Type           | Nullable | Purpose                                                      |
| ------------- | -------------- | -------- | ------------------------------------------------------------ |
| InstanceId    | int            | no       | Auto-incrementing identifier                                 |
| OdsInstanceId | int            | yes      | Matching value from `dbo.OdsInstances`                       |
| TenantId      | int            | no       | Tenant identifier                                            |
| Document      | JSON / string  | yes      | JSON document containing all but credentials information     |
| Credentials   | varbinary(500) | no       | Encrypted JSON document with `client_id` and `client_secret` |
| Status        | string         | no       | Pending, Completed, InProgress, or Error                     |
| CompletedAt   | datetime       | yes      | Set this value when completed                                |

> [!NOTE]
> Is `varbinary(500)` sufficient to hold encrypted credentials?
