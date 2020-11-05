// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database;
#if NET48
    using EdFi.Ods.AdminApp.Management.Helpers;
#else
    using EdFi.Ods.AdminApp.Web;
#endif
using NUnit.Framework;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public abstract class AdminAppDataTestBase : AdminAppDbContextTestBase
    {
        protected override string ConnectionString
        {
            get
            {
                #if NET48
                    return ConfigurationHelper.GetConnectionStrings().Admin;
                #else
                    return Startup.ConfigurationConnectionStrings.Admin;
                #endif
            }
        }

        protected override AdminAppDbContext CreateDbContext()
        {
            #if NET48
                return new AdminAppDbContext();
            #else
                return new AdminAppDbContext(ConnectionString);
            #endif
        }
    }
}
