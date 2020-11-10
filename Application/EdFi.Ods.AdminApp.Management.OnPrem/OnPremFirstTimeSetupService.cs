// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
#if NET48
using EdFi.Ods.Common.Security;
#else
using EdFi.Common.Security;
#endif

namespace EdFi.Ods.AdminApp.Management.OnPrem
{
    public class OnPremFirstTimeSetupService : FirstTimeSetupServiceBase
    {
        public OnPremFirstTimeSetupService(
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
