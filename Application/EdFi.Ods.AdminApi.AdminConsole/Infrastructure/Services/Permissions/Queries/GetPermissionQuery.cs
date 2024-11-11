// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Queries;

public interface IGetPermissionQuery
{
    Task<Permission> Execute(int tenantId);
}

public class GetPermissionQuery : IGetPermissionQuery
{
    private readonly IQueriesRepository<Permission> _permissionQuery;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public GetPermissionQuery(IQueriesRepository<Permission> permissionQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _permissionQuery = permissionQuery;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }

    public async Task<Permission> Execute(int tenantId)
    {

        var permission = await _permissionQuery.Query().SingleOrDefaultAsync(permission => permission.TenantId == tenantId);

        if (permission == null)
            return null;

        JsonNode? jnDocument = JsonNode.Parse(permission.Document);

        var encryptedClientId = jnDocument!["clientId"]?.AsValue().ToString();
        var encryptedClientSecret = jnDocument!["clientSecret"]?.AsValue().ToString();

        var clientId = string.Empty;
        var clientSecret = string.Empty;

        if (!string.IsNullOrEmpty(encryptedClientId) && !string.IsNullOrEmpty(encryptedClientSecret))
        {
            _encryptionService.TryDecrypt(encryptedClientId, _encryptionKey, out clientId);
            _encryptionService.TryDecrypt(encryptedClientSecret, _encryptionKey, out clientSecret);

            jnDocument!["clientId"] = clientId;
            jnDocument!["clientSecret"] = clientSecret;
        }

        permission.Document = jnDocument!.ToJsonString();

        return permission;
    }
}
