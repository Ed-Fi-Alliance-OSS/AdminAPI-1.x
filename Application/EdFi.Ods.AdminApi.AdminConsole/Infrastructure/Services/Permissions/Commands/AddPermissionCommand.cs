// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Commands;

public interface IAddPermissionCommand
{
    Task<Permission> Execute(IAddPermissionModel permission);
}

public class AddPermissionCommand : IAddPermissionCommand
{
    private readonly ICommandRepository<Permission> _permissionCommand;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public AddPermissionCommand(ICommandRepository<Permission> permissionCommand, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _permissionCommand = permissionCommand;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }

    public async Task<Permission> Execute(IAddPermissionModel permission)
    {
        JsonNode? jnDocument = JsonNode.Parse(permission.Document);

        var clientId = jnDocument!["clientId"]?.AsValue().ToString();
        var clientSecret = jnDocument!["clientSecret"]?.AsValue().ToString();

        var encryptedClientId = string.Empty;
        var encryptedClientSecret = string.Empty;

        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            _encryptionService.TryEncrypt(clientId, _encryptionKey, out encryptedClientId);
            _encryptionService.TryEncrypt(clientSecret, _encryptionKey, out encryptedClientSecret);

            jnDocument!["clientId"] = encryptedClientId;
            jnDocument!["clientSecret"] = encryptedClientSecret;
        }

        try
        {
            return await _permissionCommand.AddAsync(new Permission
            {
                InstanceId = permission.InstanceId,
                TenantId = permission.TenantId,
                EdOrgId = permission.EdOrgId,
                Document = jnDocument!.ToJsonString(),
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
}

public interface IAddPermissionModel
{
    int InstanceId { get; }
    int? EdOrgId { get; }
    int TenantId { get; }
    string Document { get; }
}

public class AddPermissionResult
{
    public int DocId { get; set; }
}
