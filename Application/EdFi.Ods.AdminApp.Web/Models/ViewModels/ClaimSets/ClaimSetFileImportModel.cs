// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using FluentValidation;
using log4net;
using EdFi.Ods.AdminApp.Management.Database.Queries;


namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class ClaimSetFileImportModel
    {
        private SharingModel _sharingModel;

        [DisplayName("Import File")]
        [Accept(".json")]
        public IFormFile ImportFile { get; set; }

        public SharingModel AsSharingModel()
        {
            return _sharingModel ??= SharingModel.DeserializeToSharingModel(ImportFile.OpenReadStream());
        }

        public class ClaimSetFileImportModelValidator : AbstractValidator<ClaimSetFileImportModel>
        {
            private IGetAllClaimSetsQuery _getAllClaimSetsQuery;
            private GetResourceClaimsAsFlatListQuery _getResourceClaimsQuery;

            public ClaimSetFileImportModelValidator(IGetAllClaimSetsQuery getAllClaimSetsQuery, GetResourceClaimsAsFlatListQuery getResourceClaimsQuery)
            {
                _getAllClaimSetsQuery = getAllClaimSetsQuery;
                _getResourceClaimsQuery = getResourceClaimsQuery;
                RuleFor(m => m.ImportFile).NotEmpty();

                When(m => m.ImportFile != null, () =>
                {
                    RuleFor(x => x)
                        .SafeCustom((model, context) =>
                        {
                            var validator = new SharingModelValidator(_getAllClaimSetsQuery, _getResourceClaimsQuery, context.PropertyName);

                            if (Path.GetExtension(model.ImportFile.FileName)?.ToLower() != ".json")
                            {
                                context.AddFailure("Invalid file extension. Only '*.json' files are allowed.");
                            }
                            else
                            {
                                context.AddFailures(validator.Validate(model.AsSharingModel()));
                            }
                        });
                });
            }
        }

        public class SharingModelValidator : AbstractValidator<SharingModel>
        {
            private readonly ILog _logger = LogManager.GetLogger(typeof(SharingModelValidator));

            public SharingModelValidator(IGetAllClaimSetsQuery getAllClaimSetsQuery, GetResourceClaimsAsFlatListQuery getResourceClaimsQuery, string propertyName)
            {
                const string missing = "This template is missing its expected {0}.";
                var dbResourceClaims = getResourceClaimsQuery.Execute().Select(x => x.Name).ToHashSet();

                RuleFor(x => x.Title).NotNull().WithMessage(string.Format(missing, "title"));
                RuleForEach(x => x.Template.ClaimSets)
                    .SafeCustom((sharingClaimSet, context) =>
                    {
                        if (IsAnExistingClaimSetName(sharingClaimSet.Name))
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

                bool IsAnExistingClaimSetName(string sharingClaimSetName)
                {
                    var claimSets = getAllClaimSetsQuery.Execute().ToList();
                    return claimSets.Any(x => x.Name == sharingClaimSetName);
                }
            }
        }
    }
}
