// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Web;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;
using NUnit.Framework;
using Action = EdFi.Security.DataAccess.Models.Action;
using ActionName = EdFi.Ods.AdminApp.Management.ClaimSetEditor.Action;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public abstract class SecurityDataTestBase : PlatformSecurityContextTestBase
    {
        protected override string ConnectionString
        {
            get
            {
                return Startup.ConfigurationConnectionStrings.Security;
            }
        }

        protected override SqlServerSecurityContext CreateDbContext()
        {
            return new SqlServerSecurityContext(ConnectionString);
        }

        // This bool controls whether or not to run SecurityContext initialization
        // method. Setting this flag to true will cause seed data to be
        // inserted into the security database on fixture setup.
        protected bool SeedSecurityContextOnFixtureSetup { get; set; } = false;

        protected override void AdditionalFixtureSetup()
        {
            if (SeedSecurityContextOnFixtureSetup)
            {
                SetupContext.Database.Initialize(true);
            }
        }

        protected void LoadSeedData()
        {
            Application odsApplication = GetOrCreateApplication("Ed-Fi ODS API");

            var readAction = GetOrCreateAction("Read");
            GetOrCreateAction("Create");
            GetOrCreateAction("Update");
            GetOrCreateAction("Delete");


            GetOrCreateAuthorizationStrategy(odsApplication, "Namespace Based", "NamespaceBased");

            var noFurtherStrategy = GetOrCreateAuthorizationStrategy(odsApplication, "No Further Authorization Required",
                "NoFurtherAuthorizationRequired");


            var educationStandardsResourceClaim = GetOrCreateResourceClaim("educationStandards", odsApplication);
            GetOrCreateResourceClaim("types", odsApplication);
            GetOrCreateResourceClaim("managedDescriptors", odsApplication);
            GetOrCreateResourceClaim("systemDescriptors", odsApplication);
            GetOrCreateResourceClaim("educationOrganizations", odsApplication);


            GetOrCreateResourceClaimAuthorizationMetadata(readAction, noFurtherStrategy,
                educationStandardsResourceClaim);

            TestContext.SaveChanges();

            Application GetOrCreateApplication(string applicationName)
            {
                var application = TestContext.Applications.FirstOrDefault(a => a.ApplicationName == applicationName) ??
                                  TestContext.Applications.Add(new Application
                                  {
                                      ApplicationName = "Ed-Fi ODS API"
                                  });
                return application;
            }

            Action GetOrCreateAction(string actionName)
            {
                var action = TestContext.Actions.FirstOrDefault(a => a.ActionName == actionName) ??
                             TestContext.Actions.Add(new Action
                             {
                                 ActionName = actionName,
                                 ActionUri = $"http://ed-fi.org/odsapi/actions/{actionName}"
                             });

                return action;
            }

            AuthorizationStrategy GetOrCreateAuthorizationStrategy(Application application, string displayName,
                string authorizationStrategyName)
            {
                var authorizationStrategy = TestContext.AuthorizationStrategies.FirstOrDefault(a =>
                                                a.Application.ApplicationId == application.ApplicationId && a.DisplayName == displayName &&
                                                a.AuthorizationStrategyName == authorizationStrategyName) ??
                                            TestContext.AuthorizationStrategies.Add(
                                                new AuthorizationStrategy
                                                {
                                                    DisplayName = displayName,
                                                    AuthorizationStrategyName = authorizationStrategyName,
                                                    Application = application
                                                });

                return authorizationStrategy;
            }

            ResourceClaim GetOrCreateResourceClaim(string resourceName, Application application)
            {
                var resourceClaim =
                    TestContext.ResourceClaims.FirstOrDefault(r =>
                        r.ResourceName == resourceName && r.Application.ApplicationId == application.ApplicationId) ??
                    TestContext.ResourceClaims.Add(new ResourceClaim
                    {
                        Application = application,
                        DisplayName = resourceName,
                        ResourceName = resourceName,
                        ClaimName = $"http://ed-fi.org/ods/identity/claims/domains/{resourceName}",
                        ParentResourceClaim = null
                    });

                return resourceClaim;
            }

            void GetOrCreateResourceClaimAuthorizationMetadata(Action action,
                AuthorizationStrategy authorizationStrategy,
                ResourceClaim resourceClaim)
            {
                var resourceClaimAuthorizationMetadata = TestContext.ResourceClaimAuthorizationMetadatas.FirstOrDefault(rcm =>
                        rcm.Action.ActionId == action.ActionId && rcm.AuthorizationStrategy.AuthorizationStrategyId == authorizationStrategy.AuthorizationStrategyId &&
                        rcm.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId);

                if (resourceClaimAuthorizationMetadata == null)
                {
                    TestContext.ResourceClaimAuthorizationMetadatas.Add(new ResourceClaimAuthorizationMetadata
                    {
                        Action = action,
                        AuthorizationStrategy = authorizationStrategy,
                        ResourceClaim = resourceClaim,
                        ValidationRuleSetName = null
                    });
                }
            }
        }

        protected IReadOnlyCollection<ResourceClaim> SetupResourceClaims(Application testApplication, int resourceClaimCount = 5, int childResourceClaimCount = 3)
        {
            var parentResourceClaims = new List<ResourceClaim>();
            var childResourceClaims = new List<ResourceClaim>();
            var actions = new List<Action>();
            foreach (var parentIndex in Enumerable.Range(1, resourceClaimCount))
            {
                var resourceClaim = new ResourceClaim
                {
                    ClaimName = $"TestParentResourceClaim{parentIndex}",
                    DisplayName = $"TestParentResourceClaim{parentIndex}",
                    ResourceName = $"TestParentResourceClaim{parentIndex}",
                    Application = testApplication
                };
                parentResourceClaims.Add(resourceClaim);

                childResourceClaims.AddRange(Enumerable.Range(1, childResourceClaimCount)
                    .Select(childIndex => new ResourceClaim
                    {
                        ClaimName = $"TestChildResourceClaim{childIndex}",
                        DisplayName = $"TestChildResourceClaim{childIndex}",
                        ResourceName = $"TestChildResourceClaim{childIndex}",
                        Application = testApplication,
                        ParentResourceClaim = resourceClaim,
                        ParentResourceClaimId = resourceClaim.ResourceClaimId
                    }));
            }

            foreach (var action in ActionName.GetAll())
            {
                var actionObject = new Action
                {
                    ActionName = action.Value,
                    ActionUri = action.Value
                };
                actions.Add(actionObject);
            }

            Save(parentResourceClaims.Cast<object>().ToArray());
            Save(childResourceClaims.Cast<object>().ToArray());
            Save(actions.Cast<object>().ToArray());

            return parentResourceClaims;
        }

        protected IReadOnlyCollection<ClaimSetResourceClaim> SetupParentResourceClaimsWithChildren(ClaimSet testClaimSet, Application testApplication, int resourceClaimCount = 5, int childResourceClaimCount = 3)
        {
            var actions = ActionName.GetAll().Select(action => new Action {ActionName = action.Value, ActionUri = action.Value}).ToList();
            Save(actions.Cast<object>().ToArray());

            var parentResourceClaims = Enumerable.Range(1, resourceClaimCount).Select(parentIndex => new ResourceClaim
            {
                ClaimName = $"TestParentResourceClaim{parentIndex}",
                DisplayName = $"TestParentResourceClaim{parentIndex}",
                ResourceName = $"TestParentResourceClaim{parentIndex}", Application = testApplication
            }).ToList();

            var childResourceClaims = parentResourceClaims.SelectMany(x => Enumerable.Range(1, childResourceClaimCount)
                .Select(childIndex => new ResourceClaim
                {
                    ClaimName = $"TestChildResourceClaim{childIndex}",
                    DisplayName = $"TestChildResourceClaim{childIndex}",
                    ResourceName = $"TestChildResourceClaim{childIndex}",
                    Application = testApplication,
                    ParentResourceClaim = x
                })).ToList();

            Save(childResourceClaims.Cast<object>().ToArray());

            var claimSetResourceClaims = Enumerable.Range(1, resourceClaimCount)
                .Select(index => parentResourceClaims[index - 1]).Select(parentResource => new ClaimSetResourceClaim
                {
                    ResourceClaim = parentResource,
                    Action = actions.Single(x => x.ActionName == ActionName.Create.Value), ClaimSet = testClaimSet
                }).ToList();

            var childResources = parentResourceClaims.SelectMany(x => childResourceClaims
                .Where(child => child.ParentResourceClaimId == x.ResourceClaimId)
                .Select(child => new ClaimSetResourceClaim
                {
                    ResourceClaim = child,
                    Action = actions.Single(a => a.ActionName == ActionName.Create.Value),
                    ClaimSet = testClaimSet
                }).ToList()).ToList();

            claimSetResourceClaims.AddRange(childResources);

            Save(claimSetResourceClaims.Cast<object>().ToArray());

            return claimSetResourceClaims;
        }

        protected IReadOnlyCollection<AuthorizationStrategy> SetupApplicationAuthorizationStrategies(Application testApplication, int authStrategyCount = 5)
        {
            var testAuthStrategies = Enumerable.Range(1, authStrategyCount)
                .Select(index => $"TestAuthStrategy{index}")
                .ToArray();

            var authStrategies = testAuthStrategies
                .Select(x => new AuthorizationStrategy
                {
                    AuthorizationStrategyName = x,
                    DisplayName = x,
                    Application = testApplication
                })
                .ToArray();

            Save(authStrategies.Cast<object>().ToArray());

            return authStrategies;
        }

        protected IReadOnlyCollection<ResourceClaimAuthorizationMetadata> SetupResourcesWithDefaultAuthorizationStrategies(List<AuthorizationStrategy> testAuthorizationStrategies, List<ClaimSetResourceClaim> claimSetResourceClaims)
        {
            var resourceClaimWithDefaultAuthStrategies = new List<ResourceClaimAuthorizationMetadata>();
            var random = new Random();
            foreach (var resourceClaim in claimSetResourceClaims)
            {
                var testAuthorizationStrategy = testAuthorizationStrategies[random.Next(testAuthorizationStrategies.Count)];

                var resourceClaimWithDefaultAuthStrategy = new ResourceClaimAuthorizationMetadata
                {
                    ResourceClaim = resourceClaim.ResourceClaim,
                    Action = resourceClaim.Action,
                    AuthorizationStrategy = testAuthorizationStrategy
                };
                resourceClaimWithDefaultAuthStrategies.Add(resourceClaimWithDefaultAuthStrategy);
            }

            Save(resourceClaimWithDefaultAuthStrategies.Cast<object>().ToArray());

            return resourceClaimWithDefaultAuthStrategies;
        }
    }
}
