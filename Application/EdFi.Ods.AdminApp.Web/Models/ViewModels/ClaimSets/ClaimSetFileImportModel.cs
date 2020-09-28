// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation;
using log4net;


namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class ClaimSetFileImportModel
    {
        [DisplayName("Import File")]
        [Accept(".json")]
        public HttpPostedFileBase ImportFile { get; set; }

        public class ClaimSetFileImportModelValidator : AbstractValidator<ClaimSetFileImportModel>
        {
            public ClaimSetFileImportModelValidator(ISecurityContext securityContext)
            {
                RuleFor(m => m.ImportFile).NotEmpty();

                When(m => m.ImportFile != null, () =>
                {
                    RuleFor(x => x.ImportFile)
                        .SafeCustom((model, context) =>
                        {
                            var validator = new SharingModelValidator(securityContext, context.PropertyName);
                            var sharingModel = model.DeserializeToSharingModel();
                            context.AddFailures(validator.Validate(sharingModel));
                        });
                });
            }
        }

        public class SharingModelValidator : AbstractValidator<SharingModel>
        {
            private readonly ILog _logger = LogManager.GetLogger(typeof(SharingModelValidator));

            public SharingModelValidator(ISecurityContext securityContext, string propertyName)
            {
                const string missing = "This template is missing its expected {0}.";
                var dbResourceClaims = securityContext.ResourceClaims.Select(x => x.ResourceName);

                RuleFor(x => x.Title).NotNull().WithMessage(string.Format(missing, "title"));
                RuleForEach(x => x.Template.ClaimSets)
                    .SafeCustom((sharingClaimSet, context) =>
                    {
                        var isAnExistingClaimSetName =
                            securityContext.ClaimSets.Any(c => c.ClaimSetName == sharingClaimSet.Name);

                        if (isAnExistingClaimSetName)
                        {
                            context.AddFailure(propertyName, $"This template contains a claimset with a name which already exists in the system. Please use a unique name for '{sharingClaimSet.Name}'.\n");
                            return;
                        }

                        var resourceClaims = new List<ResourceClaim>();
                        try
                        {
                            resourceClaims = sharingClaimSet.ResourceClaims.Select(x => x.ToObject<ResourceClaim>()).ToList();
                        }
                        catch (Exception exception)
                        {
                            var errorMsg =
                                $"This template contains an invalid definition of the resourceclaims for the claimset '{sharingClaimSet.Name}'. {exception.Message}\n";
                            context.AddFailure(propertyName, errorMsg);
                            _logger.Error(errorMsg, exception);
                        }

                        if (resourceClaims.Any())
                        {
                            foreach (var resourceClaim in resourceClaims)
                            {
                                if (!dbResourceClaims.Contains(resourceClaim.Name))
                                {
                                    context.AddFailure(propertyName, $"This template contains a resource which is not in the system. Claimset Name: {sharingClaimSet.Name} Resource name: '{resourceClaim.Name}'.\n");
                                }
                            }
                        }
                    });
            }
        }
    }
}