// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor
{
    public class UpdateResourcesOnClaimSetCommand
    {
        private readonly UpdateResourcesOnClaimSetCommandV6Service _v6Service;

        public UpdateResourcesOnClaimSetCommand(UpdateResourcesOnClaimSetCommandV6Service v6Service)
        {
            _v6Service = v6Service;
        }

        public void Execute(IUpdateResourcesOnClaimSetModel model)
        {
            _v6Service.Execute(model);
        }
    }

    public interface IUpdateResourcesOnClaimSetModel
    {
        int ClaimSetId { get; }
        List<ResourceClaim>? ResourceClaims { get; }
    }
}
