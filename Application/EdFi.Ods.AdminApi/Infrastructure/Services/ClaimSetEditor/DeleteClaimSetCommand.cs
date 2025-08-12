// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public interface IDeleteClaimSetCommand
{
    void Execute(IDeleteClaimSetModel claimSet);
}

public class DeleteClaimSetCommand : IDeleteClaimSetCommand
{
    private readonly DeleteClaimSetCommandService _v6Service;

    public DeleteClaimSetCommand(DeleteClaimSetCommandService v6Service)
    {
        _v6Service = v6Service;
    }

    public void Execute(IDeleteClaimSetModel claimSet)
    {
        _v6Service.Execute(claimSet);
    }
}

public interface IDeleteClaimSetModel
{
    string? Name { get; }
    int Id { get; }
}
