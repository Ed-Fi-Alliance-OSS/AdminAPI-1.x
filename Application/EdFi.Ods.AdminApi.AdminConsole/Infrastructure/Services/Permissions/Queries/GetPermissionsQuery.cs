// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Queries;

public interface IGetPermissionsQuery
{
    Task<IEnumerable<Permission>> Execute();
}

public class GetPermissionsQuery : IGetPermissionsQuery
{
    private readonly IQueriesRepository<Permission> _permissionQuery;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public GetPermissionsQuery(IQueriesRepository<Permission> permissionQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _permissionQuery = permissionQuery;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }
    public async Task<IEnumerable<Permission>> Execute()
    {
        var permissions = await _permissionQuery.GetAllAsync();

        foreach (var permission in permissions)
        {
            JsonNode? jn = JsonNode.Parse(permission.Document);

            var encryptedClientId = jn!["clientId"]?.AsValue().ToString();
            var encryptedClientSecret = jn!["clientSecret"]?.AsValue().ToString();

            var clientId = string.Empty;
            var clientSecret = string.Empty;

            if (!string.IsNullOrEmpty(encryptedClientId) && !string.IsNullOrEmpty(encryptedClientSecret))
            {
                _encryptionService.TryDecrypt(encryptedClientId, _encryptionKey, out clientId);
                _encryptionService.TryDecrypt(encryptedClientSecret, _encryptionKey, out clientSecret);

                jn!["clientId"] = clientId;
                jn!["clientSecret"] = clientSecret;
            }

            permission.Document = jn!.ToJsonString();
        }

        return permissions.ToList();
    }
}
