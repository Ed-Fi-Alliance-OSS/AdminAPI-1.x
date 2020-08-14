// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Security.DataAccess.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetClaimSetNamesQueryTests : SecurityDataTestBase
    {
        public GetClaimSetNamesQueryTests()
        {
            SeedSecurityContextOnFixtureSetup = true;
        }

        [Test]
        public void Should_Retreive_ClaimSetNames()
        {
            var application = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(application);

            var claimSet1 = GetClaimSet(application);
            var claimSet2 = GetClaimSet(application);
            Save(claimSet1, claimSet2);

            var query = new GetClaimSetNamesQuery(TestContext);
            var claimSetNames = query.Execute();

            claimSetNames.ShouldContain(claimSet1.ClaimSetName);
            claimSetNames.ShouldContain(claimSet2.ClaimSetName);
        }

        private static int _claimSetId = 0;
        private ClaimSet GetClaimSet(Application application)
        {
            return new ClaimSet
            {
                Application = application,
                ClaimSetName = $"Test Claim Set {_claimSetId++} - {DateTime.Now:O}"
            };
        }
    }
}
