// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Configuration.Claims;

namespace EdFi.Ods.AdminApp.Management
{
    public interface ILearningStandardsSetup
    {
        void SetupLearningStandardsClaims();
    }

    public class LearningStandardsSetup : ILearningStandardsSetup
    {
        private readonly ICloudOdsClaimSetConfigurator _cloudOdsClaimSetConfigurator;
        private readonly IModifyClaimSetsService _claimSetsService;

        public LearningStandardsSetup(ICloudOdsClaimSetConfigurator cloudOdsClaimSetConfigurator,
            IModifyClaimSetsService claimSetsService)
        {
            _cloudOdsClaimSetConfigurator = cloudOdsClaimSetConfigurator;
            _claimSetsService = claimSetsService;
        }

        public void SetupLearningStandardsClaims()
        {
            _cloudOdsClaimSetConfigurator.ApplyConfiguration(AcademicBenchmarkClaimSetConfiguration.Default);

            _claimSetsService.SetNoFurtherAuthorizationRequiredOverrideOnResouceClaim("educationStandards",
                CloudOdsClaimAction.Read.ActionName);
        }
    }
}
