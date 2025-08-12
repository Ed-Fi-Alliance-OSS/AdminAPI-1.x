// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using ClaimSet = EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSet;

namespace EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor;

public interface IGetAllClaimSetsQuery
{
    IReadOnlyList<ClaimSet> Execute();
    IReadOnlyList<ClaimSet> Execute(CommonQueryParams commonQueryParams);
}

public class GetAllClaimSetsQuery : IGetAllClaimSetsQuery
{
    private readonly GetAllClaimSetsQueryService _v6Service;

    public GetAllClaimSetsQuery(GetAllClaimSetsQueryService v6Service)
    {
        _v6Service = v6Service;
    }

    public IReadOnlyList<ClaimSet> Execute()
    {
        return _v6Service.Execute();
    }

    public IReadOnlyList<ClaimSet> Execute(CommonQueryParams commonQueryParams)
    {
        return _v6Service.Execute(commonQueryParams);
    }
}
