// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        var cleanedDocument = ExpandoObjectHelper.NormalizeExpandoObject(instance.Document);
        var cleanedApiCredencials = ExpandoObjectHelper.NormalizeExpandoObject(instance.ApiCredentials);

        var document = JsonConvert.SerializeObject(cleanedDocument, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Converters = new List<JsonConverter> { new ExpandoObjectConverter() },
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        var apiCredentialsDocument = JsonConvert.SerializeObject(cleanedApiCredencials, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Converters = new List<JsonConverter> { new ExpandoObjectConverter() },
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        JsonNode? jnDocument = JsonNode.Parse(document);
        JsonNode? jnApiCredentialsDocument = JsonNode.Parse(apiCredentialsDocument);

        var clientId = jnApiCredentialsDocument!["clientId"]?.AsValue().ToString();
        var clientSecret = jnApiCredentialsDocument!["clientSecret"]?.AsValue().ToString();

        var encryptedClientId = string.Empty;
        var encryptedClientSecret = string.Empty;

        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            _encryptionService.TryEncrypt(clientId, _encryptionKey, out encryptedClientId);
            _encryptionService.TryEncrypt(clientSecret, _encryptionKey, out encryptedClientSecret);

            jnApiCredentialsDocument!["clientId"] = encryptedClientId;
            jnApiCredentialsDocument!["clientSecret"] = encryptedClientSecret;
        }

        try
        {
            return await _instanceCommand.AddAsync(new Instance
            {
                OdsInstanceId = instance.OdsInstanceId,
                TenantId = instance.TenantId,
                EdOrgId = instance.EdOrgId,
                Document = jnDocument!.ToJsonString(),
                ApiCredentials = jnApiCredentialsDocument!.ToJsonString(),
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
    int OdsInstanceId { get; }
    int? EdOrgId { get; }
    int TenantId { get; }
    ExpandoObject Document { get; }
    ExpandoObject ApiCredentials { get; }
}

public class AddInstanceResult
{
    public int DocId { get; set; }
}
