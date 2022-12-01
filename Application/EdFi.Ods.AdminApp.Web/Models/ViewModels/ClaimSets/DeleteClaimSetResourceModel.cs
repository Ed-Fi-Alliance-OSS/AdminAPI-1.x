// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class DeleteClaimSetResourceModel: IDeleteResourceOnClaimSetModel
    {
        public int ClaimSetId { get; set; }
        public int ResourceClaimId { get; set; }
        public string ClaimSetName { get; set; }
        public string ResourceName { get; set; }

        public class DeleteClaimSetResourceModelValidator : AbstractValidator<DeleteClaimSetResourceModel>
        {
            private readonly IGetResourcesByClaimSetIdQuery _getResourcesByClaimSetIdQuery;

            public DeleteClaimSetResourceModelValidator(IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery)
            {
                _getResourcesByClaimSetIdQuery = getResourcesByClaimSetIdQuery;

                RuleFor(m => m.ClaimSetId).NotEmpty();
                RuleFor(m => m.ResourceClaimId)
                    .NotEmpty()
                    .Must(BeOnTheClaimSetAlready)
                    .WithMessage("This resource does not exist on the claimset.");
            }

            private bool BeOnTheClaimSetAlready(DeleteClaimSetResourceModel model, int resourceClaimId)
            {
                return _getResourcesByClaimSetIdQuery.SingleResource(model.ClaimSetId, resourceClaimId) != null;
            }
        }
    }
}