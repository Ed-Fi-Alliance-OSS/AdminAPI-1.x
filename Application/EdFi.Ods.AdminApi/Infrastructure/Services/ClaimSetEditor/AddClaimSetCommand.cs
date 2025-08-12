// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class AddClaimSetCommand
{
    private readonly AddClaimSetCommandService _v6Service;

    public AddClaimSetCommand(AddClaimSetCommandService v6Service)
    {
        _v6Service = v6Service;
    }

    public int Execute(IAddClaimSetModel claimSet)
    {
        return _v6Service.Execute(claimSet);
    }
}

public interface IAddClaimSetModel
{
    string? ClaimSetName { get; }
}
