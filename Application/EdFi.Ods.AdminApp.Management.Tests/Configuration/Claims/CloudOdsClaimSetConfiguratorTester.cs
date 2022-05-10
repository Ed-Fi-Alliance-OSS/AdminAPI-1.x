// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Security.DataAccess.Models;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Configuration.Claims.CloudOdsClaimAction;
using static EdFi.Ods.AdminApp.Management.Configuration.Claims.CloudOdsClaimAuthorizationStrategy;

namespace EdFi.Ods.AdminApp.Management.Tests.Configuration.Claims
{
    [TestFixture]
    public class CloudOdsClaimSetConfiguratorTester : SecurityDataTestBase
    {
        public CloudOdsClaimSetConfiguratorTester()
        {
            CheckpointPolicy = CheckpointPolicyOptions.BeforeAnyTest;

            SeedSecurityContextOnFixtureSetup = true;
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var application = new Application
            {
                ApplicationName = "IntegrationTests"
            };

            SetupContext.Applications.Add(application);

            var authorizationStrategy = new AuthorizationStrategy
            {
                Application = application,
                AuthorizationStrategyName = NoFurtherAuthorizationRequired.StrategyName,
                DisplayName = NoFurtherAuthorizationRequired.DisplayName
            };
            SetupContext.AuthorizationStrategies.Add(authorizationStrategy);

            SetupContext.SaveChanges();
        }
        

        [Test]
        public void ShouldPersistClaimSet()
        {
            LoadSeedData();

            var testClaimSet = new CloudOdsClaimSet
            {
                ApplicationName = "IntegrationTests",
                ClaimSetName = "ODS AdminApp",
                Claims = new[]
                {
                    new CloudOdsEntityClaim
                    {
                        EntityName = "educationOrganizations",
                        Actions = new[] { Create, Read, Update, CloudOdsClaimAction.Delete },
                        AuthorizationStrategy = NoFurtherAuthorizationRequired
                    },
                    new CloudOdsEntityClaim
                    {
                        EntityName = "systemDescriptors",
                        Actions = new[] { Create, Read, Update, CloudOdsClaimAction.Delete },
                        AuthorizationStrategy = NoFurtherAuthorizationRequired
                    },
                    new CloudOdsEntityClaim
                    {
                        EntityName = "managedDescriptors",
                        Actions = new[] { Create, Read, Update, CloudOdsClaimAction.Delete },
                        AuthorizationStrategy = NoFurtherAuthorizationRequired
                    },
                    new CloudOdsEntityClaim
                    {
                        EntityName = "types",
                        Actions = new[] { Read },
                        AuthorizationStrategy = NoFurtherAuthorizationRequired
                    }
                }
            };

            var configurator = new CloudOdsClaimSetConfigurator(SetupContext);
            configurator.ApplyConfiguration(testClaimSet);

            var claimSet = Transaction(securityContext => securityContext.ClaimSets.Single(cs => cs.ClaimSetName == testClaimSet.ClaimSetName));

            Transaction(securityContext =>
            {
                var claimSetResourceClaims = securityContext.ClaimSetResourceClaimActions
                    .Include(c => c.Action)
                    .Include(c => c.ResourceClaim)
                    .Include(c => c.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy))
                    .Where(c => c.ClaimSet.ClaimSetId == claimSet.ClaimSetId).ToList();

                foreach (var claim in testClaimSet.Claims)
                {
                    foreach (var resourceClaim in claim.Actions.Select(action => claimSetResourceClaims.Single(rc => rc.ResourceClaim.ResourceName == claim.EntityName && rc.Action.ActionName == action.ActionName)))
                    {
                        resourceClaim.AuthorizationStrategyOverrides.First().AuthorizationStrategy.AuthorizationStrategyName.ShouldBe(claim.AuthorizationStrategy.StrategyName);
                    }
                }
            });
        }

        [Test]
        public void ShouldNotErrorIfClaimSetAlreadyExists()
        {
            var application = SetupContext.Applications.Single(a => a.ApplicationName == "IntegrationTests");
            
            var claimSet = new ClaimSet
            {
                Application = application,
                ClaimSetName = "Other ODS AdminApp"
            };

            SetupContext.ClaimSets.Add(claimSet);
            SetupContext.SaveChanges();

            var testClaimSet = new CloudOdsClaimSet
            {
                ApplicationName = "IntegrationTests",
                ClaimSetName = "Other ODS AdminApp",
                Claims = new[]
                {
                    new CloudOdsEntityClaim
                    {
                        EntityName = "educationOrganizations",
                        Actions = new[] { Create, Read, Update, CloudOdsClaimAction.Delete },
                        AuthorizationStrategy = NoFurtherAuthorizationRequired
                    }
                }
            };

            var configurator = new CloudOdsClaimSetConfigurator(SetupContext);
            configurator.ApplyConfiguration(testClaimSet);
        }
    }
}
