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
    public class OverrideDefaultAuthorizationStrategyModel: IOverrideDefaultAuthorizationStrategyModel
    {
        public int ClaimSetId { get; set; }
        public int ResourceClaimId { get; set; }
        public int AuthorizationStrategyForCreate { get; set; }
        public int AuthorizationStrategyForRead { get; set; }
        public int AuthorizationStrategyForUpdate { get; set; }
        public int AuthorizationStrategyForDelete { get; set; }
    }

    public class OverrideDefaultAuthorizationStrategyModelValidator : AbstractValidator<OverrideDefaultAuthorizationStrategyModel>
    {
        private readonly ISecurityContext _context;

        public OverrideDefaultAuthorizationStrategyModelValidator(ISecurityContext context)
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
