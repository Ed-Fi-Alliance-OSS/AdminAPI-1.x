// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class ResetToDefaultAuthStrategyModel : IResetToDefaultAuthStrategyModel
    {
        public int ClaimSetId { get; set; }
        public int ResourceClaimId { get; set; }
    }

    public class ResetToDefaultAuthStrategyModelValidator : AbstractValidator<ResetToDefaultAuthStrategyModel>
    {
        private readonly ISecurityContext _context;

        public ResetToDefaultAuthStrategyModelValidator(ISecurityContext context)
        {
            _context = context;

            RuleFor(m => m).Must(m => ExistInTheSystem(m.ResourceClaimId, m.ClaimSetId)).WithMessage("No actions for this claimset and resource exist in the system");

        }

        private bool ExistInTheSystem(int resourceClaimId, int claimSetId)
        {
            return _context.ClaimSetResourceClaimActions.Any(x =>
                x.ResourceClaim.ResourceClaimId == resourceClaimId && x.ClaimSet.ClaimSetId == claimSetId);
        }
    }
}
