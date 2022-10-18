// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets.ClaimSetFileExportModel;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class ClaimSetFileExportCommandTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldExportClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet1 = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(testClaimSet1);

            var testClaimSet2 = new ClaimSet { ClaimSetName = "TestClaimSet2", Application = testApplication };
            Save(testClaimSet2);

            SetupParentResourceClaimsWithChildren(testClaimSet1, testApplication);

            SetupParentResourceClaimsWithChildren(testClaimSet2, testApplication);

            var exportModel = Scoped<IGetClaimSetByIdQuery, ClaimSetFileExportModel>(getClaimSetById =>
            {
                var editorClaimSets = new List<Management.ClaimSetEditor.ClaimSet>
                {
                    getClaimSetById.Execute(testClaimSet1.ClaimSetId),
                    getClaimSetById.Execute(testClaimSet2.ClaimSetId)
                };

                return new ClaimSetFileExportModel
                {
                    Title = "TestDownload",
                    ClaimSets = editorClaimSets,
                    SelectedForExport = new List<int>
                    {
                        testClaimSet1.ClaimSetId, testClaimSet2.ClaimSetId
                    }
                };
            });

            var sharingModel = Scoped<ClaimSetFileExportCommand, SharingModel>(command => command.Execute(exportModel));

            var resourcesForClaimSet1 =
                Scoped<IGetResourcesByClaimSetIdQuery, Management.ClaimSetEditor.ResourceClaim[]>(
                    query => query.AllResources(testClaimSet1.ClaimSetId).ToArray());
            var resourcesForClaimSet2 =
                Scoped<IGetResourcesByClaimSetIdQuery, Management.ClaimSetEditor.ResourceClaim[]>(
                    query => query.AllResources(testClaimSet2.ClaimSetId).ToArray());

            sharingModel.Title.ShouldContain("TestDownload");
            var sharedClaimSets = sharingModel.Template.ClaimSets;

            sharedClaimSets.Length.ShouldBe(2);

            var sharedClaimSet1 = sharedClaimSets[0];

            var sharedClaimSet2 = sharedClaimSets[1];

            sharedClaimSet1.Name.ShouldBe(testClaimSet1.ClaimSetName);
            MatchResources(sharedClaimSet1.ResourceClaims, resourcesForClaimSet1);

            sharedClaimSet2.Name.ShouldBe(testClaimSet2.ClaimSetName);
            MatchResources(sharedClaimSet2.ResourceClaims, resourcesForClaimSet2);
        }

        private static void MatchResources(IEnumerable<JObject> resourceClaims, IReadOnlyCollection<ResourceClaim> testResourceClaims)
        {
            foreach (var resourceObject in resourceClaims)
            {
                var resourceClaim = resourceObject.ToObject<ResourceClaim>();
                var testResource = testResourceClaims.FirstOrDefault(x => x.Name == resourceClaim.Name);
                testResource.ShouldNotBeNull();
                testResource.Create.ShouldBe(resourceClaim.Create);
                testResource.Read.ShouldBe(resourceClaim.Read);
                testResource.Update.ShouldBe(resourceClaim.Update);
                testResource.Delete.ShouldBe(resourceClaim.Delete);
                if (resourceClaim.Children.Count > 0)
                {
                    var children = resourceObject.GetValue("Children").Select(child => child.ToObject<JObject>()).ToList();
                    MatchResources(children, testResource.Children);
                }
            }
        }

        [Test]
        public void ShouldNotExportIfNoSelectedClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet1 = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(testClaimSet1);

            var testClaimSet2 = new ClaimSet { ClaimSetName = "TestClaimSet2", Application = testApplication };
            Save(testClaimSet2);

            var exportModel = Scoped<IGetClaimSetByIdQuery, ClaimSetFileExportModel>(
                getClaimSetById => new ClaimSetFileExportModel
                {
                    Title = "TestDownload",
                    ClaimSets = new List<Management.ClaimSetEditor.ClaimSet>
                    {
                        getClaimSetById.Execute(testClaimSet1.ClaimSetId),
                        getClaimSetById.Execute(testClaimSet2.ClaimSetId)
                    },
                    SelectedForExport = new List<int>()
                });

            var validator = new ClaimSetFileExportModelValidator();
            var validationResults = validator.Validate(exportModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("You must select at least one claimset to proceed.");
        }

        [Test]
        public void ShouldNotExportIfTitleEmpty()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet1 = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(testClaimSet1);

            var testClaimSet2 = new ClaimSet { ClaimSetName = "TestClaimSet2", Application = testApplication };
            Save(testClaimSet2);

            var exportModel = Scoped<IGetClaimSetByIdQuery, ClaimSetFileExportModel>(
                getClaimSetById => new ClaimSetFileExportModel
                {
                    ClaimSets = new List<Management.ClaimSetEditor.ClaimSet>
                    {
                        getClaimSetById.Execute(testClaimSet1.ClaimSetId),
                        getClaimSetById.Execute(testClaimSet2.ClaimSetId)
                    },
                    SelectedForExport = new List<int>
                    {
                        testClaimSet1.ClaimSetId, testClaimSet2.ClaimSetId
                    }
                });

            var validator = new ClaimSetFileExportModelValidator();
            var validationResults = validator.Validate(exportModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("'Title' must not be empty.");
        }
    }
}
