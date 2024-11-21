// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Steps.Queries;

public interface IGetStepsQuery
{
    Task<IEnumerable<Step>> Execute();
}

public class GetStepsQuery : IGetStepsQuery
{
    private readonly IQueriesRepository<Step> _stepQuery;

    public GetStepsQuery(IQueriesRepository<Step> stepQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _stepQuery = stepQuery;
    }
    public async Task<IEnumerable<Step>> Execute()
    {
        var steps = await _stepQuery.GetAllAsync();
        return steps.ToList();
    }
}
