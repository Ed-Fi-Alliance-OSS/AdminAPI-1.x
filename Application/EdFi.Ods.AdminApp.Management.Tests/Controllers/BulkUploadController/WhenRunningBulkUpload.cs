// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.BulkUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.BulkUploadController
{
    [TestFixture]
    public class WhenRunningBulkUpload : BulkUploadControllerFixture
    {
        private static readonly InstanceContext OdsInstanceContext = new InstanceContext
        {
            Id = 1234,
            Name = "Ed_Fi_Ods_1234"
        };
        private static readonly OdsSecretConfiguration OdsSecretConfig = new OdsSecretConfiguration()
        {
            BulkUploadCredential = new BulkUploadCredential()
            {
                ApiKey = "key",
                ApiSecret = "secret"
            }
        };
        private readonly OdsApiConnectionInformation _connectionInformation = new OdsApiConnectionInformation(OdsInstanceContext.Name, ApiMode.Sandbox) {
            ApiServerUrl = "http://example.com", ClientKey = OdsSecretConfig.BulkUploadCredential.ApiKey, ClientSecret = OdsSecretConfig.BulkUploadCredential.ApiSecret
        };

        [Test]
        public async Task When_Perform_Get_Request_To_BulkUploadForm_Return_PartialView_With_Expected_Model()
        {
            // Arrange
            OdsSecretConfigurationProvider.Setup(x => x.GetSecretConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(new OdsSecretConfiguration()));

            // Act
            var result = (ViewResult) await SystemUnderTest.Index();

            // Assert
            result.ShouldNotBeNull();
            var model = (BulkFileUploadIndexModel) result.Model;
            model.ShouldNotBeNull();
            model.BulkFileUploadModel.ApiKey.ShouldBeEmpty();
            model.BulkFileUploadModel.ApiSecret.ShouldBeEmpty();
            model.BulkFileUploadModel.CredentialsSaved.ShouldBeFalse();
        }

        [Test]
        public async Task When_Perform_Get_Request_To_BulkUploadForm_Return_PartialView_With_Expected_ModelData()
        {
            // Arrange
            const string expectedKey = "key";
            const string expectedSecret = "secret";

            OdsSecretConfigurationProvider.Setup(x => x.GetSecretConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(new OdsSecretConfiguration
                {
                    BulkUploadCredential = new BulkUploadCredential
                    {
                        ApiKey = expectedKey,
                        ApiSecret = expectedSecret
                    }
                }));

            // Act
            var result = (ViewResult) await SystemUnderTest.Index();

            // Assert
            result.ShouldNotBeNull();
            var model = (BulkFileUploadIndexModel) result.Model;
            model.ShouldNotBeNull();
            model.BulkFileUploadModel.ApiKey.ShouldBe(expectedKey);
            model.BulkFileUploadModel.ApiSecret.ShouldBe(expectedSecret);
            model.BulkFileUploadModel.CredentialsSaved.ShouldBeTrue();
        }

        [Test]
        public async Task When_Perform_Get_Request_To_BulkUploadForm_With_Null_SecretConfig_Returns_JsonError()
        {
            // Arrange
            OdsSecretConfigurationProvider.Setup(x => x.GetSecretConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult<OdsSecretConfiguration>(null));

            // Act
            var result = (ContentResult)await SystemUnderTest.Index();

            // Assert
            result.ShouldNotBeNull();
            result.Content.Contains("ODS secret configuration can not be null").ShouldBeTrue();
        }

        [Test]
        public void When_Perform_Post_Request_To_BulkFileUpload_With_No_File_ThrowsException()
        {
            // Arrange
            var model = new BulkFileUploadModel
            {
                    BulkFiles = new List<IFormFile>()
            };

            // Assert
            Assert.ThrowsAsync<Exception>(() => SystemUnderTest.BulkFileUpload(model)).Message
                .Contains("The given file is empty. Please upload a file compatible with the Ed-Fi Data Standard.").ShouldBeTrue();
        }

        [Test]
        public void When_Perform_Post_Request_To_BulkFileUpload_With_Greater_File_ContentLength_ThrowsException()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            file.Setup(x => x.Length).Returns(20000002);
            var model = new BulkFileUploadModel
            {
                    BulkFiles = new List<IFormFile>
                    {
                        file.Object
                    }
            };

            // Assert
            Assert.ThrowsAsync<Exception>(() => SystemUnderTest.BulkFileUpload(model)).Message
                .Contains("Upload exceeds maximum limit").ShouldBeTrue();
        }

        [Test]
        public void When_Perform_Post_Request_To_BulkFileUpload_With_Multiple_Files_ThrowsException()
        {
            // Arrange
            var file1 = new Mock<IFormFile>();
            file1.Setup(x => x.Length).Returns(200);
            var file2 = new Mock<IFormFile>();
            file2.Setup(x => x.Length).Returns(200);

            var model = new BulkFileUploadModel
                {
                    BulkFiles = new List<IFormFile>
                    {
                        file1.Object, file2.Object
                    }
                };

            // Assert
            Assert.ThrowsAsync<Exception>(() => SystemUnderTest.BulkFileUpload(model)).Message
                .Contains("Currently, the bulk import process only supports a single file at a time").ShouldBeTrue();
        }

        [Test]
        public async Task When_Perform_Post_Request_To_BulkFileUpload_With_Valid_File_Job_Should_Be_Enqueued()
        {
            const string odsApiVersion = "5.0.0";
            const string edfiStandardVersion = "3.2.0-c";
            InferOdsApiVersion.Setup(x => x.Version("http://example.com")).Returns(Task.FromResult(odsApiVersion));
            InferOdsApiVersion.Setup(x => x.EdFiStandardVersion("http://example.com")).Returns(Task.FromResult(edfiStandardVersion));

            var schemaBasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Schema");
            var schemaPath = $"{schemaBasePath}\\{edfiStandardVersion}";

            var model = SetupBulkUpload(out var fileUploadResult);

            BulkUploadJob.Setup(x => x.IsJobRunning()).Returns(false);
            BulkUploadJob.Setup(x => x.IsSameOdsInstance(OdsInstanceContext.Id, typeof(BulkUploadJobContext))).Returns(true);

            var result = (PartialViewResult) await SystemUnderTest.BulkFileUpload(model);

            // Assert
            Func<BulkUploadJobContext, bool> bulkUploadJobEnqueueVerifier = actual =>
            {
                actual.ShouldSatisfyAllConditions(
                    () => actual.DataDirectoryFullPath.ShouldBe(fileUploadResult.Directory),
                    () => actual.OdsInstanceId.ShouldBe(OdsInstanceContext.Id),
                    () => actual.ApiBaseUrl.ShouldBe(_connectionInformation.ApiBaseUrl),
                    () => actual.ClientKey.ShouldBe(_connectionInformation.ClientKey),
                    () => actual.ClientSecret.ShouldBe(_connectionInformation.ClientSecret),
                    () => actual.DependenciesUrl.ShouldBe(_connectionInformation.DependenciesUrl),
                    () => actual.MetadataUrl.ShouldBe(_connectionInformation.MetadataUrl),
                    () => actual.OauthUrl.ShouldBe(_connectionInformation.OAuthUrl),
                    () => actual.SchemaPath.ShouldBe(schemaPath),
                    () => actual.MaxSimultaneousRequests.ShouldBe(20)
                );
                return true;
            };
            result.ShouldNotBeNull();
            result.ViewName.ShouldBe("_SignalRStatus_BulkLoad");
            result.Model.ShouldNotBeNull();
            var uploadModel = (BulkFileUploadModel) result.Model;
            uploadModel.ShouldNotBeNull();
            uploadModel.IsSameOdsInstance.ShouldBeTrue();
            BulkUploadJob.Verify(
                x => x.EnqueueJob(It.Is<BulkUploadJobContext>(y => bulkUploadJobEnqueueVerifier(y))),
                Times.Once);
        }

        [Test]
        public async Task When_Perform_Post_Request_To_BulkFileUpload_Against_ODS3_Job_Should_Be_Enqueued_With_Pessimistic_Throttling()
        {
            const string odsApiVersion = "3.4.0";
            const string edfiStandardVersion = "3.2.0-b";
            InferOdsApiVersion.Setup(x => x.Version("http://example.com")).Returns(Task.FromResult(odsApiVersion));
            InferOdsApiVersion.Setup(x => x.EdFiStandardVersion("http://example.com")).Returns(Task.FromResult(edfiStandardVersion));

            var model = SetupBulkUpload(out var fileUploadResult);

            BulkUploadJob.Setup(x => x.IsJobRunning()).Returns(false);
            BulkUploadJob.Setup(x => x.IsSameOdsInstance(OdsInstanceContext.Id, typeof(BulkUploadJobContext))).Returns(true);

            var result = (PartialViewResult)await SystemUnderTest.BulkFileUpload(model);

            // Assert
            Func<BulkUploadJobContext, bool> bulkUploadJobEnqueueVerifier = actual =>
            {
                actual.MaxSimultaneousRequests.ShouldBe(1);
                return true;
            };
            result.ShouldNotBeNull();
            result.ViewName.ShouldBe("_SignalRStatus_BulkLoad");
            result.Model.ShouldNotBeNull();
            var uploadModel = (BulkFileUploadModel)result.Model;
            uploadModel.ShouldNotBeNull();
            uploadModel.IsSameOdsInstance.ShouldBeTrue();
            BulkUploadJob.Verify(
                x => x.EnqueueJob(It.Is<BulkUploadJobContext>(y => bulkUploadJobEnqueueVerifier(y))),
                Times.Once);
        }

        [Test]
        public async Task When_Job_Is_Already_Running_New_Job_Should_Not_Be_Enqueued()
        {
            const string odsApiVersion = "3.4.0";
            const string edfiStandardVersion = "3.2.0-b";
            InferOdsApiVersion.Setup(x => x.Version("http://example.com")).Returns(Task.FromResult(odsApiVersion));
            InferOdsApiVersion.Setup(x => x.EdFiStandardVersion("http://example.com")).Returns(Task.FromResult(edfiStandardVersion));

            var model = SetupBulkUpload(out var fileUploadResult);

            BulkUploadJob.Setup(x => x.IsJobRunning()).Returns(true);
            BulkUploadJob.Setup(x => x.IsSameOdsInstance(OdsInstanceContext.Id, typeof(BulkUploadJobContext))).Returns(true);

            var result = (PartialViewResult)await SystemUnderTest.BulkFileUpload(model);

            // Assert
            result.ShouldNotBeNull();
            result.ViewName.ShouldBe("_SignalRStatus_BulkLoad");
            result.Model.ShouldNotBeNull();
            var uploadModel = (BulkFileUploadModel)result.Model;
            uploadModel.ShouldNotBeNull();
            uploadModel.IsJobRunning.ShouldBeTrue();
            uploadModel.IsSameOdsInstance.ShouldBeTrue();
            BulkUploadJob.Verify(
                x => x.EnqueueJob(It.IsAny<BulkUploadJobContext>()),
                Times.Never);
        }

        private BulkFileUploadModel SetupBulkUpload(out FileUploadResult fileUploadResult)
        {
            const string filename = "test.xml";

            var file = new Mock<IFormFile>();
            file.Setup(x => x.Length).Returns(200);
            file.Setup(x => x.FileName).Returns("test.xml");

            var model = new BulkFileUploadModel
            {
                BulkFileType = InterchangeFileType.AssessmentMetadata.Value,
                BulkFiles = new List<IFormFile>
                {
                    file.Object
                }
            };

            fileUploadResult = new FileUploadResult
            {
                Directory = "directoryPath",
                FileNames = new[] {filename}
            };

            InstanceContext.Id = OdsInstanceContext.Id;
            InstanceContext.Name = OdsInstanceContext.Name;

            FileUploadHandler.Setup(x =>
                    x.SaveFilesToUploadDirectory(It.IsAny<IFormFile[]>(), It.IsAny<Func<string, string>>(), WebHostingEnvironment.Object))
                .Returns(fileUploadResult);

            ApiConnectionInformationProvider
                .Setup(x => x.GetConnectionInformationForEnvironment())
                .ReturnsAsync(_connectionInformation);

            OdsSecretConfigurationProvider.Setup(x => x.GetSecretConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(OdsSecretConfig));
            return model;
        }

        [Test]
        public async Task When_Perform_Post_Request_To_SaveBulkLoadCredentials_With_BulkUpload_Credentials_Return_Json_Success()
        {
            // Arrange
            const string expectedKey = "key";
            const string expectedSecret = "secret";
            var model = new SaveBulkUploadCredentialsModel
            {
                ApiKey = expectedKey,
                ApiSecret = expectedSecret
            };
          
            OdsSecretConfigurationProvider.Setup(x => x.GetSecretConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(new OdsSecretConfiguration()));

            // Act
            var result = (ContentResult) await SystemUnderTest.SaveBulkLoadCredentials(model);

            // Assert
            result.Content.Contains("Credentials successfully saved").ShouldBeTrue();
        }

        [Test]
        public async Task When_Perform_Post_Request_To_ResetCredentials_With_Valid_OdsSecretConfig_Returns_Json_Success()
        {
            // Arrange
            const string expectedKey = "key";
            const string expectedSecret = "secret";
            var odsConfig = new OdsSecretConfiguration
            {
                BulkUploadCredential = new BulkUploadCredential
                {
                    ApiKey = expectedKey,
                    ApiSecret = expectedSecret
                }
            };
            OdsSecretConfigurationProvider.Setup(x => x.GetSecretConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(odsConfig));

            // Act
            var result = (ContentResult) await SystemUnderTest.ResetCredentials();

            // Assert
            result.Content.Contains("Credentials successfully reset").ShouldBeTrue();
        }

        [Test]
        public async Task When_Perform_Post_Request_To_ResetCredentials_With_No_BulkUploadCredentials_Returns_Json_Error()
        {
            // Arrange
            OdsSecretConfigurationProvider.Setup(x => x.GetSecretConfiguration(It.IsAny<int>()))
                .Returns(Task.FromResult(new OdsSecretConfiguration()));

            // Act
            var result = (ContentResult) await SystemUnderTest.ResetCredentials();

            // Assert
            result.Content.Contains("Missing bulk load credentials").ShouldBeTrue();
        }
    }
}
