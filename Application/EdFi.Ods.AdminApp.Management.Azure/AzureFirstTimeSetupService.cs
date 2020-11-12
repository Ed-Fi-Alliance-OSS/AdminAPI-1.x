// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Security.DataAccess.Contexts;
#if NET48
using EdFi.Ods.Common.Security;
#else
using EdFi.Common.Security;
#endif

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureFirstTimeSetupService : FirstTimeSetupServiceBase
    {
        private readonly ISecurityContext _securityContext;

        public int MaxAttempts { get; set; }
        public int RunsAttempted { get; private set; }

        public AzureFirstTimeSetupService(
            IUsersContext usersContext,
            ISecurityContext securityContext,
            ISecurePackedHashProvider securePackedHashProvider,
            IHashConfigurationProvider hashConfigurationProvider)
            : base(usersContext, securePackedHashProvider, hashConfigurationProvider)
        {
            MaxAttempts = 5;
            RunsAttempted = 0;

            _securityContext = securityContext;
        }

        public override void EnsureAdminDatabaseInitialized()
        {
            int exponentialWait = 2;
            int delayInMilliseconds = 1000;

            //there appears to be a timing issue in Azure SQL where a login is not usable
            //until a few seconds after it is created. automatic retry logic is implemented below
            //as a workaround for that situation
            for (int i = 0; i < MaxAttempts - 1; ++i)
            {
                try
                {
                    ++RunsAttempted;
                    RunDatabaseMigrations();
                    return;
                }
                catch
                {
                    Thread.Sleep(delayInMilliseconds);
                    delayInMilliseconds *= exponentialWait;
                }
            }

            //run the final retry.  this will throw on error
            //so ultimately the exception will bubble up to the user
            //after we've used all our retries
            ++RunsAttempted;
            RunDatabaseMigrations();
        }

        private void RunDatabaseMigrations()
        {
            //trigger migrations by running a query
            var instances = UsersContext.OdsInstances.ToList();
            var applications = _securityContext.Applications.ToList();
        }
    }
}
