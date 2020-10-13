// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.Common.Security;

namespace EdFi.Ods.AdminApp.Management.Aws
{
    [System.Obsolete("Review usages.")]
    public class AwsFirstTimeSetupService : FirstTimeSetupServiceBase
    {
        public AwsFirstTimeSetupService(
            IUsersContext usersContext, 
            ISecurePackedHashProvider securePackedHashProvider, 
            IHashConfigurationProvider hashConfigurationProvider) 
            : base(usersContext, securePackedHashProvider, hashConfigurationProvider)
        {
        }

        public override void EnsureAdminDatabaseInitialized()
        {
        }
    }
}
