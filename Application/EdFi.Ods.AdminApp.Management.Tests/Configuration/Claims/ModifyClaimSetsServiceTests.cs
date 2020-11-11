// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Configuration.Claims
{
    [TestFixture]
    public class ModifyClaimSetsServiceTests : SecurityDataTestBase
    {
        public ModifyClaimSetsServiceTests()
        {
            CheckpointPolicy = CheckpointPolicyOptions.BeforeAnyTest;
            SeedSecurityContextOnFixtureSetup = true;
        }

        [Test]
        public void ShouldModifyExistingClaimSetToUseNoFurtherAuthStrategy()
        {
            LoadSeedData();

            var modifyClaimSets = new ModifyClaimSetsService(TestContext);
            modifyClaimSets.SetNoFurtherAuthorizationRequiredOverrideOnResouceClaim("educationStandards", CloudOdsClaimAction.Read.ActionName);

            TestContext.SaveChanges();

            var testAuthorizationStrat = Transaction(securityContext => securityContext.ResourceClaimAuthorizationMetadatas
                .Include(x => x.AuthorizationStrategy)
                .Single(x =>
                    x.Action.ActionName == CloudOdsClaimAction.Read.ActionName &&
                    x.ResourceClaim.ResourceName == "educationStandards")
                .AuthorizationStrategy);

            testAuthorizationStrat.AuthorizationStrategyName.ShouldBe(CloudOdsClaimAuthorizationStrategy
                .NoFurtherAuthorizationRequired.StrategyName);
        }

        [Test]
        public void ShouldHandleClaimsetThatDoesNotExist()
        {
            var noFurtherAuthRequiredCount = TestContext.ResourceClaimAuthorizationMetadatas.Count(x =>
                x.AuthorizationStrategy.AuthorizationStrategyName ==
                CloudOdsClaimAuthorizationStrategy.NoFurtherAuthorizationRequired.StrategyName);

            var modifyClaimSets = new ModifyClaimSetsService(TestContext);
            modifyClaimSets.SetNoFurtherAuthorizationRequiredOverrideOnResouceClaim("doesNotExist", "NotAnAction");

            TestContext.ResourceClaims.Count(x => x.ResourceName == "doesNotExist").ShouldBe(0);

            TestContext.Actions.Count(x => x.ActionName == "NotAnAction").ShouldBe(0);

            TestContext.ResourceClaimAuthorizationMetadatas
                .Count(x => x.AuthorizationStrategy.AuthorizationStrategyName ==
                            CloudOdsClaimAuthorizationStrategy.NoFurtherAuthorizationRequired.StrategyName)
                .ShouldBe(noFurtherAuthRequiredCount);
        }
    }
}
