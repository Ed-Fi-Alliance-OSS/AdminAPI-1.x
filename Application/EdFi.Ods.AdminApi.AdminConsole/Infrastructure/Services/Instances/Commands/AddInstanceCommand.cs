// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;

public interface IAddInstanceCommand
{
    Task<Instance> Execute(IAddInstanceModel instance);
}

public class AddInstanceCommand : IAddInstanceCommand
{
    private readonly ICommandRepository<Instance> _instanceCommand;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public AddInstanceCommand(ICommandRepository<Instance> instanceCommand, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _instanceCommand = instanceCommand;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }

    public async Task<Instance> Execute(IAddInstanceModel instance)
    {
        JsonNode? jnDocument = JsonNode.Parse(instance.Document);

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
            return await _instanceCommand.AddAsync(new Instance
            {
                InstanceId = instance.InstanceId,
                TenantId = instance.TenantId,
                EdOrgId = instance.EdOrgId,
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

public interface IAddInstanceModel
{
    int InstanceId { get; }
    int? EdOrgId { get; }
    int TenantId { get; }
    string Document { get; }
}

public class AddInstanceResult
{
    public int DocId { get; set; }
}
