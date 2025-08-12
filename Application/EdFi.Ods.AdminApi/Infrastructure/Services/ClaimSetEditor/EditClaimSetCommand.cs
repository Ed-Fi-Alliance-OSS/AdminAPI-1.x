// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor
{
    public interface IEditClaimSetCommand
    {
        int Execute(IEditClaimSetModel claimSet);
    }

    public class EditClaimSetCommand : IEditClaimSetCommand
    {
        private readonly EditClaimSetCommandV6Service _v6Service;

        public EditClaimSetCommand(EditClaimSetCommandV6Service v6Service)
        {
            _v6Service = v6Service;
        }

        public int Execute(IEditClaimSetModel claimSet)
        {
            return _v6Service.Execute(claimSet);
        }
    }

    public interface IEditClaimSetModel
    {
        string? ClaimSetName { get; }
        int ClaimSetId { get; }
    }
}
