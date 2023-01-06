// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    internal class ClaimSetCheck53Service : IClaimSetCheckService
    {
        private readonly ISecurityContext _securityContext;

        public ClaimSetCheck53Service(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public bool RequiredClaimSetsExist()
        {
            return ClaimSetExists(CloudsOdsAcademicBenchmarksConnectApp.DefaultClaimSet) && ClaimSetExists(CloudOdsAdminApp.InternalAdminAppClaimSet);
            bool ClaimSetExists(string claimSetName)
            {
                return _securityContext.ClaimSets.Any(x => x.ClaimSetName == claimSetName);
            }
        }
    }
}
