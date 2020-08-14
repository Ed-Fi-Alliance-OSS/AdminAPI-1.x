// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class EditClaimSetCommand
    {
        private readonly ISecurityContext _context;

        public EditClaimSetCommand(ISecurityContext context)
        {
            _context = context;
        }

        public int Execute(IEditClaimSetModel claimSet)
        {
            var existingClaimSet = _context.ClaimSets.Single(x => x.ClaimSetId == claimSet.ClaimSetId);
            existingClaimSet.ClaimSetName = claimSet.ClaimSetName;

            _context.SaveChanges();

            return existingClaimSet.ClaimSetId;
        }
    }

    public interface IEditClaimSetModel
    {
        string ClaimSetName { get; }
        int ClaimSetId { get; }
    }
}
