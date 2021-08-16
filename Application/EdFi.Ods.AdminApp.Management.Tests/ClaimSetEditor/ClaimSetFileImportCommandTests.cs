// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Web.Helpers;
using Shouldly;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using EdFi.Security.DataAccess.Contexts;
using Moq;
using static EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets.ClaimSetFileImportModel;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class ClaimSetFileImportCommandTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldImportClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            SetupResourceClaims(testApplication);

            var testJSON = @"{
                ""title"": ""testfile"",
                ""template"": {
                    ""claimSets"": [
                      {
                        ""name"": ""Test Claimset"",
                        ""resourceClaims"": [
                          {
                            ""Name"": ""TestParentResourceClaim1"",
                            ""Read"": true,
                            ""Create"": false,
                            ""Update"": false,
                            ""Delete"": false,
                            ""Children"": []
                          },
                          {
                            ""Name"": ""TestParentResourceClaim2"",
                            ""Read"": true,
                            ""Create"": false,
                            ""Update"": false,
                            ""Delete"": false,	
                            ""Children"": []
                          },
                          {
                            ""Name"": ""TestParentResourceClaim3"",
                            ""Read"": true,
                            ""Create"": true,
                            ""Update"": true,
                            ""Delete"": true,
                            ""Children"": []
                          }
                        ]
                      }
                    ]
                }
            }";

            var importModel = GetImportModel(testJSON);
            var importSharingModel = SharingModel.DeserializeToSharingModel(importModel.ImportFile.OpenReadStream());

            Scoped<ClaimSetFileImportCommand>(command => command.Execute(importSharingModel));

            var testClaimSet = Transaction(securityContext => securityContext.ClaimSets.SingleOrDefault(x => x.ClaimSetName == "Test Claimset"));
            testClaimSet.ShouldNotBeNull();

            var resourcesForClaimSet =
                Scoped<IGetResourcesByClaimSetIdQuery, List<Management.ClaimSetEditor.ResourceClaim>>(
                    query => query.AllResources(testClaimSet.ClaimSetId).ToList());

            resourcesForClaimSet.Count.ShouldBeGreaterThan(0);
            var testResources = resourcesForClaimSet.Where(x => x.ParentId == 0).ToArray();
            testResources.Count().ShouldBe(3);

            var testResource1 = testResources[0];
            MatchActions(testResource1, "TestParentResourceClaim1", new bool[] { false, true, false, false});

            var testResource2 = testResources[1];
            MatchActions(testResource2, "TestParentResourceClaim2", new bool[] { false, true, false, false });

            var testResource3 = testResources[2];
            MatchActions(testResource3, "TestParentResourceClaim3", new bool[] { true, true, true, true });

        }

        private void MatchActions(ResourceClaim dbResource, string expectedResourceName, bool[] expectedCrudArray)
        {
            dbResource.Name.ShouldBe(expectedResourceName);
            dbResource.Create.ShouldBe(expectedCrudArray[0]);
            dbResource.Read.ShouldBe(expectedCrudArray[1]);
            dbResource.Update.ShouldBe(expectedCrudArray[2]);
            dbResource.Delete.ShouldBe(expectedCrudArray[3]);
        }

        [Test]
        public void ShouldNotImportIfClaimSetNotUnique()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var alreadyExistingClaimSet = new ClaimSet { ClaimSetName = "Test ClaimSet", Application = testApplication };
            Save(alreadyExistingClaimSet);

            SetupResourceClaims(testApplication);

            var testJSON = @"{
                ""title"": ""testfile"",
                ""template"": {
                    ""claimSets"": [
                      {
                        ""name"": ""Test Claimset"",
                        ""resourceClaims"": [
                          {
                            ""Name"": ""TestParentResourceClaim1"",
                            ""Read"": true,
                            ""Create"": false,
                            ""Update"": false,
                            ""Delete"": false,
                            ""Children"": []
                          }
                        ]
                      }
                    ]
                }
            }";

            var importModel = GetImportModel(testJSON);

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new ClaimSetFileImportModelValidator(securityContext);
                var validationResults = validator.Validate(importModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain(
                    "This template contains a claimset with a name which already exists in the system. Please use a unique name for 'Test Claimset'.\n");
            });
        }

        [Test]
        public void ShouldNotImportIfResourceDoesNotExist()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            SetupResourceClaims(testApplication);         

            var testJSON = @"{
                ""title"": ""testfile"",
                ""template"": {
                    ""claimSets"": [
                      {
                        ""name"": ""Test Claimset"",
                        ""resourceClaims"": [
                          {
                            ""Name"": ""TestParentResourceClaim88"",
                            ""Read"": true,
                            ""Create"": false,
                            ""Update"": false,
                            ""Delete"": false,
                            ""Children"": []
                          }
                        ]
                      }
                    ]
                }
            }";

            var importModel = GetImportModel(testJSON);

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new ClaimSetFileImportModelValidator(securityContext);
                var validationResults = validator.Validate(importModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("This template contains a resource which is not in the system. Claimset Name: Test Claimset Resource name: 'TestParentResourceClaim88'.\n");
            });
        }

        [Test]
        public void ShouldNotImportIfImportFileEmpty()
        {
            var importModel = new ClaimSetFileImportModel();

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new ClaimSetFileImportModelValidator(securityContext);
                var validationResults = validator.Validate(importModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("'Import File' must not be empty.");
            });
        }

        [Test]
        public void ShouldNotImportIfImportFileHasAnInvalidExtension()
        {
            var importModel = new ClaimSetFileImportModel();

            var importFile = new Mock<IFormFile>();

            importFile.Setup(f => f.FileName).Returns("testfile.xml");
            importModel.ImportFile = importFile.Object;

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new ClaimSetFileImportModelValidator(securityContext);
                var validationResults = validator.Validate(importModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("Invalid file extension. Only '*.json' files are allowed.");
            });
        }

        private static ClaimSetFileImportModel GetImportModel(string testJson)
        {
            var byteArray = Encoding.UTF8.GetBytes(testJson);
            var stream = new MemoryStream(byteArray);
            var importModel = new ClaimSetFileImportModel();

            var importFile = new Mock<IFormFile>();
            importFile.Setup(f => f.Length).Returns(stream.Capacity);
            importFile.Setup(f => f.OpenReadStream()).Returns(stream);

            importFile.Setup(f => f.FileName).Returns("testfile.json");
            importModel.ImportFile = importFile.Object;

            return importModel;
        }
    }
}
