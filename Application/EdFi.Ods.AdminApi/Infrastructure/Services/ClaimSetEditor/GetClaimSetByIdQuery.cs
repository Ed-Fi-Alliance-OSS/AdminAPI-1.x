// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class GetClaimSetByIdQuery : IGetClaimSetByIdQuery
{
    private readonly GetClaimSetByIdQueryV6Service _v6Service;

    public GetClaimSetByIdQuery(GetClaimSetByIdQueryV6Service v6Service)
    {
        _v6Service = v6Service;
    }

    public ClaimSet Execute(int securityContextClaimSetId)
    {
        return _v6Service.Execute(securityContextClaimSetId);
    }
}

public interface IGetClaimSetByIdQuery
{
    ClaimSet Execute(int securityContextClaimSetId);
}

