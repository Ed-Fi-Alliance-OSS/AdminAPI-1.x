// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class GetClaimSetByIdQuery : IGetClaimSetByIdQuery
{
    private readonly IOdsSecurityModelVersionResolver _resolver;
    private readonly GetClaimSetByIdQueryV53Service _v53Service;
    private readonly GetClaimSetByIdQueryV6Service _v6Service;

    public GetClaimSetByIdQuery(IOdsSecurityModelVersionResolver resolver,
        GetClaimSetByIdQueryV53Service v53Service,
        GetClaimSetByIdQueryV6Service v6Service)
    {
        _resolver = resolver;
        _v53Service = v53Service;
        _v6Service = v6Service;
    }

    public ClaimSet Execute(int securityContextClaimSetId)
    {
        var securityModel = _resolver.DetermineSecurityModel();

        return securityModel switch
        {
            EdFiOdsSecurityModelCompatibility.ThreeThroughFive or EdFiOdsSecurityModelCompatibility.FiveThreeCqe => _v53Service.Execute(securityContextClaimSetId),
            EdFiOdsSecurityModelCompatibility.Six => _v6Service.Execute(securityContextClaimSetId),
            _ => throw new EdFiOdsSecurityModelCompatibilityException(securityModel),
        };
    }
}

public interface IGetClaimSetByIdQuery
{
    ClaimSet Execute(int securityContextClaimSetId);
}

