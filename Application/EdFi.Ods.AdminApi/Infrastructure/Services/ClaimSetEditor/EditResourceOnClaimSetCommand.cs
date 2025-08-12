// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class EditResourceOnClaimSetCommand(EditResourceOnClaimSetCommandService v6Service)
{
    private readonly EditResourceOnClaimSetCommandService _v6Service = v6Service;
    
    public void Execute(IEditResourceOnClaimSetModel model)
    {
        _v6Service.Execute(model);
    }
}

public interface IEditResourceOnClaimSetModel
{
    int ClaimSetId { get; }
    ResourceClaim? ResourceClaim { get; }
}

