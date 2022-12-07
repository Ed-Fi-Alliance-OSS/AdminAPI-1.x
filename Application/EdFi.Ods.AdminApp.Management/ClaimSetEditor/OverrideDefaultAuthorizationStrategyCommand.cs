// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor;

public class OverrideDefaultAuthorizationStrategyCommand
{
    private readonly IOdsSecurityModelVersionResolver _resolver;
    private readonly OverrideDefaultAuthorizationStrategyV53Service _v53Service;
    private readonly OverrideDefaultAuthorizationStrategyV6Service _v6Service;

    public OverrideDefaultAuthorizationStrategyCommand(IOdsSecurityModelVersionResolver resolver, OverrideDefaultAuthorizationStrategyV53Service v53Service, OverrideDefaultAuthorizationStrategyV6Service v6Service)
    {
        _resolver = resolver;
        _v53Service = v53Service;
        _v6Service = v6Service;
    }

    public void Execute(IOverrideDefaultAuthorizationStrategyModel model)
    {
        var securityModel = _resolver.DetermineSecurityModel();
        if (securityModel == EdFiOdsSecurityModelCompatibility.ThreeThroughFive)
            _v53Service.Execute(model);
        else if (securityModel == EdFiOdsSecurityModelCompatibility.Six)
            _v6Service.Execute(model);
        else
            throw new EdFiOdsSecurityModelCompatibilityException(securityModel);
    }
}

public interface IOverrideDefaultAuthorizationStrategyModel
{
    int ClaimSetId { get; }
    int ResourceClaimId { get; }
    int AuthorizationStrategyForCreate { get; }
    int AuthorizationStrategyForRead { get; }
    int AuthorizationStrategyForUpdate { get; }
    int AuthorizationStrategyForDelete { get; }
}
