// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management
{
    public interface IAssessmentVendorAdjustment
    {
        void ReadAndCreatePerformanceLevelDescriptors();
    }

    public class AssessmentVendorAdjustment : IAssessmentVendorAdjustment
    {
        private readonly ISecurityContext _securityContext;

        public AssessmentVendorAdjustment(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public void ReadAndCreatePerformanceLevelDescriptors()
        {
            var resource = _securityContext.ResourceClaims.FirstOrDefault(x => x.ResourceName == "performanceLevelDescriptor");
            if (resource == null) return;

            var claimSet = _securityContext.ClaimSets.FirstOrDefault(x => x.ClaimSetName == "Assessment Vendor");
            if (claimSet == null) return;

            var createAction = _securityContext.Actions.Single(x => x.ActionName == "Create");
            var readAction = _securityContext.Actions.Single(x => x.ActionName == "Read");

            _securityContext.ClaimSetResourceClaims.AddRange(new[]
            {
                new ClaimSetResourceClaim {Action = createAction, ClaimSet = claimSet, ResourceClaim = resource},
                new ClaimSetResourceClaim {Action = readAction, ClaimSet = claimSet, ResourceClaim = resource}
            });

            _securityContext.SaveChanges();
        }
    }
}