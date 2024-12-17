// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;

public interface IGetInstanceByIdQuery
{
    Task<Instance> Execute(int odsInstanceId);
}

public class GetInstanceByIdQuery : IGetInstanceByIdQuery
{
    private readonly IQueriesRepository<Instance> _instanceQuery;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public GetInstanceByIdQuery(IQueriesRepository<Instance> instanceQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _instanceQuery = instanceQuery;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }

    public async Task<Instance> Execute(int odsInstanceId)
    {

        var instance = await _instanceQuery.Query().SingleOrDefaultAsync(instance => instance.OdsInstanceId == odsInstanceId);

        if (instance == null)
            return null;

        JsonNode? jnDocument = JsonNode.Parse(instance.Document);

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

        instance.Document = jnDocument!.ToJsonString();

        return instance;
    }
}
