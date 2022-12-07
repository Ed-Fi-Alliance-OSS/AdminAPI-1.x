﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor;

public class OverrideDefaultAuthorizationStrategyCommand
{
    private readonly OverrideDefaultAuthorizationStrategyV53Service _v53Service;

    public OverrideDefaultAuthorizationStrategyCommand(OverrideDefaultAuthorizationStrategyV53Service v53Service)
    {
        _v53Service = v53Service;
    }

    public void Execute(IOverrideDefaultAuthorizationStrategyModel model)
    {
        _v53Service.Execute(model);
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
