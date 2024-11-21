// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.UserProfiles.Queries;

public interface IGetUserProfilesQuery
{
    Task<IEnumerable<UserProfile>> Execute();
}

public class GetUserProfilesQuery : IGetUserProfilesQuery
{
    private readonly IQueriesRepository<UserProfile> _userProfilesQuery;

    public GetUserProfilesQuery(IQueriesRepository<UserProfile> userProfilesQuery)
    {
        _userProfilesQuery = userProfilesQuery;
    }
    public async Task<IEnumerable<UserProfile>> Execute()
    {
        return await _userProfilesQuery.GetAllAsync();
    }
}
