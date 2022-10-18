// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class EditClaimSetResourceModel: IEditResourceOnClaimSetModel
    {
        public int ClaimSetId { get; set; }
        public ResourceClaim ResourceClaim { get; set; }
        public IEnumerable<ResourceClaim> ExistingResourceClaims { get; set; }
    }

    public class EditClaimSetResourceModelValidator : AbstractValidator<EditClaimSetResourceModel>
    {
        public EditClaimSetResourceModelValidator()
        {
            RuleFor(m => m.ResourceClaim).Must(HaveValidActions)
                .WithMessage(m =>
                    $"Only valid resources can be added. A resource must have at least one action associated with it to be added. The following is an invalid resource:\n{m.ResourceClaim.Name}")
                .Must((model, resourceClaim) => NotHaveDuplicates(model.ExistingResourceClaims, resourceClaim))
                .WithMessage(m =>
                    $"Only unique resource claims can be added. The following is a duplicate resource:\n{m.ResourceClaim.Name}");
        }

        private static bool NotHaveDuplicates(IEnumerable<ResourceClaim> existingResourceClaims, ResourceClaim resourceClaim)
        {
            return existingResourceClaims == null || !existingResourceClaims.Contains(resourceClaim);
        }

        private static bool HaveValidActions(ResourceClaim resourceClaim)
        {
            return resourceClaim.Create || resourceClaim.Delete || resourceClaim.Read || resourceClaim.Update;
        }
    }
}