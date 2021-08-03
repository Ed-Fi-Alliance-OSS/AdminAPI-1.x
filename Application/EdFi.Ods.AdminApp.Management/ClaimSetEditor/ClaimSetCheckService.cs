// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class ClaimSetCheckService
    {
        private readonly ISecurityContext _securityContext;

        public ClaimSetCheckService(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public bool IsRestartRequired()
        {
            return !ClaimSetExists("AB Connect") || !ClaimSetExists("Ed-Fi ODS Admin App");

            bool ClaimSetExists(string claimSetName)
            {
                return _securityContext.ClaimSets.Any(x => x.ClaimSetName == claimSetName);
            }
        }
    }
}
