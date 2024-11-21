// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;

public interface IGetInstancesQuery
{
    Task<IEnumerable<Instance>> Execute();
}

public class GetInstancesQuery : IGetInstancesQuery
{
    private readonly IQueriesRepository<Instance> _instanceQuery;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public GetInstancesQuery(IQueriesRepository<Instance> instanceQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _instanceQuery = instanceQuery;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }
    public async Task<IEnumerable<Instance>> Execute()
    {
        var instances = await _instanceQuery.GetAllAsync();

        foreach (var instance in instances)
        {
            JsonNode? jn = JsonNode.Parse(instance.Document);

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

            instance.Document = jn!.ToJsonString();
        }

        return instances.ToList();
    }
}
