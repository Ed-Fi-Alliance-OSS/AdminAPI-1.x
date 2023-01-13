// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using EdFi.SecurityCompatiblity53.DataAccess.Models;

using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;
using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using ResourceClaim = EdFi.SecurityCompatiblity53.DataAccess.Models.ResourceClaim;
using Action = EdFi.SecurityCompatiblity53.DataAccess.Models.Action;
using ActionName = EdFi.Ods.AdminApp.Management.ClaimSetEditor.Action;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class GetResourcesByClaimSetIdQueryV53ServiceTests : SecurityData53TestBase
    {
        [Test]
        public void ShouldGetParentResourcesByClaimSetId()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };

            Save(testApplication);
            var testClaimSets = SetupApplicationWithClaimSets(testApplication).ToList();
            var testResourceClaims = SetupParentResourceClaims(testClaimSets, testApplication);
            foreach (var testClaimSet in testClaimSets)
            {
                var results = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

                var testResourceClaimsForId =
                    testResourceClaims.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId).Select(x => x.ResourceClaim).ToArray();
                results.Count.ShouldBe(testResourceClaimsForId.Length);
                results.Select(x => x.Name).ShouldBe(testResourceClaimsForId.Select(x => x.ResourceName), true);
                results.Select(x => x.Id).ShouldBe(testResourceClaimsForId.Select(x => x.ResourceClaimId), true);
                results.All(x => x.Create).ShouldBe(true);
            }
        }

        [Test]
        public void ShouldGetSingleResourceByClaimSetIdAndResourceId()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };

            Save(testApplication);
            var testClaimSets = SetupApplicationWithClaimSets(testApplication).ToList();
            var testResourceClaims = SetupParentResourceClaims(testClaimSets, testApplication);

            foreach (var testClaimSet in testClaimSets)
            {
                var testResourceClaim =
                    testResourceClaims.Single(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ResourceName == "TestResourceClaim3.00").ResourceClaim;
                var result = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testResourceClaim.ResourceClaimId);

                result.Name.ShouldBe(testResourceClaim.ResourceName);
                result.Id.ShouldBe(testResourceClaim.ResourceClaimId);
                result.Create.ShouldBe(true);
                result.Read.ShouldBe(false);
                result.Update.ShouldBe(false);
                result.Delete.ShouldBe(false);
            }

        }

        [Test]
        public void ShouldGetParentResourcesWithChildrenByClaimSetId()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };

            Save(testApplication);
            var testClaimSets = SetupApplicationWithClaimSets(testApplication);
            var testResourceClaims = SetupParentResourceClaimsWithChildren(testClaimSets, testApplication);

            using var securityContext = TestContext;
            foreach (var testClaimSet in testClaimSets)
            {
                var results = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

                var testParentResourceClaimsForId =
                    testResourceClaims.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ParentResourceClaim == null).Select(x => x.ResourceClaim).ToArray();

                results.Count.ShouldBe(testParentResourceClaimsForId.Length);
                results.Select(x => x.Name).ShouldBe(testParentResourceClaimsForId.Select(x => x.ResourceName), true);
                results.Select(x => x.Id).ShouldBe(testParentResourceClaimsForId.Select(x => x.ResourceClaimId), true);
                results.All(x => x.Create).ShouldBe(true);

                foreach (var testParentResourceClaim in testParentResourceClaimsForId)
                {
                    var testChildren = securityContext.ResourceClaims.Where(x =>
                        x.ParentResourceClaimId == testParentResourceClaim.ResourceClaimId).ToList();
                    var parentResult = results.First(x => x.Id == testParentResourceClaim.ResourceClaimId);
                    parentResult.Children.Select(x => x.Name).ShouldBe(testChildren.Select(x => x.ResourceName), true);
                    parentResult.Children.Select(x => x.Id).ShouldBe(testChildren.Select(x => x.ResourceClaimId), true);
                    parentResult.Children.All(x => x.Create).ShouldBe(true);
                }
            }
        }

        [Test]
        public void ShouldGetDefaultAuthorizationStrategiesForParentResourcesByClaimSetId()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet
            {
                ClaimSetName = "TestClaimSet",
                Application = testApplication
            };
            Save(testClaimSet);

            var appAuthorizationStrategies = SetupApplicationAuthorizationStrategies(testApplication).ToList();
            var testResourceClaims = SetupParentResourceClaims(new List<ClaimSet>{testClaimSet}, testApplication);
            var testAuthStrategies = SetupResourcesWithDefaultAuthorizationStrategies(appAuthorizationStrategies, testResourceClaims.ToList());
            var results = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId).ToArray();
            results.Select(x => x.DefaultAuthStrategiesForCRUD[0].AuthStrategyName).ShouldBe(testAuthStrategies.Select(x => x.AuthorizationStrategy.AuthorizationStrategyName), true);

        }

        [Test]
        public void ShouldGetDefaultAuthorizationStrategiesForSingleResourcesByClaimSetIdAndResourceId()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet
            {
                ClaimSetName = "TestClaimSet",
                Application = testApplication
            };
            Save(testClaimSet);

            var appAuthorizationStrategies = SetupApplicationAuthorizationStrategies(testApplication).ToList();
            var testResourceClaims = SetupParentResourceClaims(new List<ClaimSet> { testClaimSet }, testApplication);
            var testAuthStrategies = SetupResourcesWithDefaultAuthorizationStrategies(appAuthorizationStrategies, testResourceClaims.ToList());

            var testResourceClaim =
                testResourceClaims.Single(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ResourceName == "TestResourceClaim3.00").ResourceClaim;
            var testAuthStrategy = testAuthStrategies.Single(x =>
                x.ResourceClaim.ResourceClaimId == testResourceClaim.ResourceClaimId && x.Action.ActionName == ActionName.Create.Value).AuthorizationStrategy;

            var result = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testResourceClaim.ResourceClaimId);

            result.Name.ShouldBe(testResourceClaim.ResourceName);
            result.Id.ShouldBe(testResourceClaim.ResourceClaimId);
            result.Create.ShouldBe(true);
            result.Read.ShouldBe(false);
            result.Update.ShouldBe(false);
            result.Delete.ShouldBe(false);
            result.DefaultAuthStrategiesForCRUD[0].AuthStrategyName.ShouldBe(testAuthStrategy.DisplayName);
        }

        [Test]
        public void ShouldGetDefaultAuthorizationStrategiesForParentResourcesWithChildrenByClaimSetId()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet
            {
                ClaimSetName = "TestClaimSet",
                Application = testApplication
            };
            Save(testClaimSet);

            var appAuthorizationStrategies = SetupApplicationAuthorizationStrategies(testApplication).ToList();

            var testResourceClaims = SetupParentResourceClaimsWithChildren(new List<ClaimSet> { testClaimSet }, testApplication);
            var testAuthStrategies = SetupResourcesWithDefaultAuthorizationStrategies(appAuthorizationStrategies, testResourceClaims.ToList());


            var results = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId).ToArray();

            var testParentResourceClaimsForId =
                testResourceClaims
                    .Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId &&
                                x.ResourceClaim.ParentResourceClaim == null).Select(x => x.ResourceClaim).ToArray();

            var testAuthStrategiesForParents =
                testAuthStrategies.Where(x => x.ResourceClaim.ParentResourceClaim == null);

            results.Select(x => x.DefaultAuthStrategiesForCRUD[0].AuthStrategyName).ShouldBe(testAuthStrategiesForParents.Select(x => x.AuthorizationStrategy.AuthorizationStrategyName), true);

            foreach (var testParentResourceClaim in testParentResourceClaimsForId)
            {
                var parentResult = results.First(x => x.Id == testParentResourceClaim.ResourceClaimId);
                var testAuthStrategiesForChildren =
                    testAuthStrategies.Where(x =>
                        x.ResourceClaim.ParentResourceClaimId == testParentResourceClaim.ResourceClaimId);
                parentResult.Children.Select(x => x.DefaultAuthStrategiesForCRUD[0].AuthStrategyName).ShouldBe(testAuthStrategiesForChildren.Select(x => x.AuthorizationStrategy.AuthorizationStrategyName), true);
            }

        }

        private IReadOnlyCollection<ClaimSet> SetupApplicationWithClaimSets(Application testApplication, int claimSetCount = 5)
        {
            var testClaimSetNames = Enumerable.Range(1, claimSetCount)
                .Select((x, index) => $"TestClaimSetName{index:N}")
                .ToArray();

            var testClaimSets = testClaimSetNames
                .Select(x => new ClaimSet
                {
                    ClaimSetName = x,
                    Application = testApplication
                })
                .ToArray();

            Save(testClaimSets.Cast<object>().ToArray());

            return testClaimSets;
        }

        private IReadOnlyCollection<ClaimSetResourceClaim> SetupParentResourceClaims(IEnumerable<ClaimSet> testClaimSets, Application testApplication, int resourceClaimCount = 5)
        {
            var claimSetResourceClaims = new List<ClaimSetResourceClaim>();
            foreach (var claimSet in testClaimSets)
            {
                foreach (var index in Enumerable.Range(1, resourceClaimCount))
                {
                    var resourceClaim = new ResourceClaim
                    {
                        ClaimName = $"TestResourceClaim{index:N}",
                        DisplayName = $"TestResourceClaim{index:N}",
                        ResourceName = $"TestResourceClaim{index:N}",
                        Application = testApplication
                    };
                    var action = new Action
                    {
                        ActionName = ActionName.Create.Value,
                        ActionUri = "create"
                    };
                    var claimSetResourceClaim = new ClaimSetResourceClaim
                    {
                        ResourceClaim = resourceClaim, Action = action, ClaimSet = claimSet
                    };
                    claimSetResourceClaims.Add(claimSetResourceClaim);
                }
            }

            Save(claimSetResourceClaims.Cast<object>().ToArray());

            return claimSetResourceClaims;
        }

        private IReadOnlyCollection<ClaimSetResourceClaim> SetupParentResourceClaimsWithChildren(IEnumerable<ClaimSet> testClaimSets, Application testApplication, int resourceClaimCount = 5, int childResourceClaimCount = 3)
        {
            var parentResourceClaims = new List<ResourceClaim>();
            var childResourceClaims = new List<ResourceClaim>();
            foreach (var parentIndex in Enumerable.Range(1, resourceClaimCount))
            {
                var resourceClaim = new ResourceClaim
                {
                    ClaimName = $"TestParentResourceClaim{parentIndex:N}",
                    DisplayName = $"TestParentResourceClaim{parentIndex:N}",
                    ResourceName = $"TestParentResourceClaim{parentIndex:N}",
                    Application = testApplication
                };
                parentResourceClaims.Add(resourceClaim);

                childResourceClaims.AddRange(Enumerable.Range(1, childResourceClaimCount)
                    .Select(childIndex => new ResourceClaim
                    {
                        ClaimName = $"TestChildResourceClaim{childIndex:N}",
                        DisplayName = $"TestChildResourceClaim{childIndex:N}",
                        ResourceName = $"TestChildResourceClaim{childIndex:N}",
                        Application = testApplication,
                        ParentResourceClaim = resourceClaim,
                        ParentResourceClaimId = resourceClaim.ResourceClaimId
                    }));
            }

            Save(parentResourceClaims.Cast<object>().ToArray());
            Save(childResourceClaims.Cast<object>().ToArray());

            var claimSetResourceClaims = new List<ClaimSetResourceClaim>();
            var claimSets = testClaimSets.ToList();
            foreach (var claimSet in claimSets)
            {
                foreach (var index in Enumerable.Range(1, childResourceClaimCount))
                {
                    var action = new Action
                    {
                        ActionName = ActionName.Create.Value,
                        ActionUri = "create"
                    };
                    var claimSetResourceClaim = new ClaimSetResourceClaim
                    {
                        ResourceClaim = childResourceClaims[index - 1],
                        Action = action,
                        ClaimSet = claimSet
                    };
                    claimSetResourceClaims.Add(claimSetResourceClaim);
                }
            }

            Save(claimSetResourceClaims.Cast<object>().ToArray());

            claimSetResourceClaims = new List<ClaimSetResourceClaim>();
            foreach (var claimSet in claimSets)
            {
                foreach (var index in Enumerable.Range(1, resourceClaimCount))
                {
                    var parentResource = parentResourceClaims[index - 1];
                    var action = new Action
                    {
                        ActionName = ActionName.Create.Value,
                        ActionUri = "create"
                    };
                    var claimSetResourceClaim = new ClaimSetResourceClaim
                    {
                        ResourceClaim = parentResource,
                        Action = action,
                        ClaimSet = claimSet
                    };
                    claimSetResourceClaims.Add(claimSetResourceClaim);
                    var childResources = childResourceClaims
                        .Where(x => x.ParentResourceClaimId == parentResource.ResourceClaimId).Select(x =>
                            new ClaimSetResourceClaim
                            {
                                ResourceClaim = x,
                                Action = action,
                                ClaimSet = claimSet
                            }).ToArray();
                    claimSetResourceClaims.AddRange(childResources);
                }
            }

            Save(claimSetResourceClaims.Cast<object>().ToArray());

            return claimSetResourceClaims;
        }
    }
}
