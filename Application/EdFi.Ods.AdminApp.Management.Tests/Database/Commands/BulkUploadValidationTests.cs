// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class BulkUploadValidationTests : AdminDataTestBase
    {
        [Test]
        public void ShouldAllowBulkLoadCredentialsFromTheSameInstance()
        {
            var odsInstance1 = new OdsInstance
            {
                Name = "Test Instance 1",
                InstanceType = "Ods",
                IsExtended = false,
                Status = "OK",
                Version = "1.0.0"
            };

            var apiClientForInstance1 = SetupApplicationForInstance(odsInstance1);

            var odsInstance2 = new OdsInstance
            {
                Name = "Test Instance 2",
                InstanceType = "Ods",
                IsExtended = false,
                Status = "OK",
                Version = "1.0.0"
            };

            SetupApplicationForInstance(odsInstance2);

            var saveBulkUploadCredentialsModel = new SaveBulkUploadCredentialsModel
            {
                ApiKey = apiClientForInstance1.Key,
                ApiSecret = apiClientForInstance1.Secret
            };

            var instanceContext = new InstanceContext
            {
                Id = odsInstance1.OdsInstanceId,
                Name = odsInstance1.Name
            };

            var validator = new SaveBulkUploadCredentialsModelValidator(TestContext, instanceContext);
            var validationResults = validator.Validate(saveBulkUploadCredentialsModel);
            validationResults.IsValid.ShouldBe(true);
        }

        [Test]
        public void ShouldNotAllowBulkLoadCredentialsFromADifferentInstance()
        {
            var odsInstance1 = new OdsInstance
            {
                Name = "Test Instance 1",
                InstanceType = "Ods",
                IsExtended = false,
                Status = "OK",
                Version = "1.0.0"
            };

            var apiClientForInstance1 = SetupApplicationForInstance(odsInstance1);

            var odsInstance2 = new OdsInstance
            {
                Name = "Test Instance 2",
                InstanceType = "Ods",
                IsExtended = false,
                Status = "OK",
                Version = "1.0.0"
            };

            SetupApplicationForInstance(odsInstance2);

            var saveBulkUploadCredentialsModel = new SaveBulkUploadCredentialsModel
            {
                ApiKey = apiClientForInstance1.Key,
                ApiSecret = apiClientForInstance1.Secret
            };

            var instanceContext = new InstanceContext
            {
                Id = odsInstance2.OdsInstanceId,
                Name = odsInstance2.Name
            };

            var validator = new SaveBulkUploadCredentialsModelValidator(TestContext, instanceContext);
            var validationResults = validator.Validate(saveBulkUploadCredentialsModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("The Api Key provided is not associated with the currently selected ODS instance.");
        }

        [Test]
        public void ShouldNotAllowEmptyBulkLoadCredentials()
        {
            var saveBulkUploadCredentialsModel = new SaveBulkUploadCredentialsModel
            {
                ApiKey = "",
                ApiSecret = ""
            };

            var instanceContext = new InstanceContext
            {
                Id = 1,
                Name = "Test Instance"
            };

            var validator = new SaveBulkUploadCredentialsModelValidator(TestContext, instanceContext);
            var validationResults = validator.Validate(saveBulkUploadCredentialsModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldBe(new List<string>
            {
                "'Api Key' must not be empty.",
                "'Api Secret' must not be empty."
            });
        }

        private ApiClient SetupApplicationForInstance(OdsInstance instance)
        {
            var vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Integration Tests"
            };

            var user = new Admin.DataAccess.Models.User
            {
                Email = "nobody@nowhere.com",
                FullName = "Integration Tests",
                Vendor = vendor
            };

            var profile = new Profile
            {
                ProfileName = "Test Profile"
            };

            Save(vendor, user, profile, instance);

            var instanceContext = new InstanceContext
            {
                Id = instance.OdsInstanceId,
                Name = instance.Name
            };

            var command = new AddApplicationCommand(SetupContext, instanceContext);
            var newApplication = new TestApplication
            {
                Environment = CloudOdsEnvironment.Production,
                ApplicationName = "Test Application",
                ClaimSetName = "FakeClaimSet",
                ProfileId = profile.ProfileId,
                VendorId = vendor.VendorId,
                EducationOrganizationIds = new List<int> { 12345, 67890 }
            };

            var result = command.Execute(newApplication);

            var persistedApplication = TestContext.Applications.Single(a => a.ApplicationId == result.ApplicationId);

            persistedApplication.ApiClients.Count.ShouldBe(1);
            var apiClient = persistedApplication.ApiClients.First();
            apiClient.Name.ShouldBe(CloudOdsApplicationName.GetPersistedName("Test Application", CloudOdsEnvironment.Production));
            apiClient.ApplicationEducationOrganizations.All(o => o.EducationOrganizationId == 12345 || o.EducationOrganizationId == 67890).ShouldBeTrue();
            apiClient.Key.ShouldBe(result.Key);
            apiClient.Secret.ShouldBe(result.Secret);

            persistedApplication.OdsInstance.ShouldNotBeNull();
            persistedApplication.OdsInstance.Name.ShouldBe(instance.Name);

            return apiClient;
        }
        

        private class TestApplication : IAddApplicationModel
        {
            public CloudOdsEnvironment Environment { get; set; }
            public string ApplicationName { get; set; }
            public int VendorId { get; set; }
            public string ClaimSetName { get; set; }
            public int? ProfileId { get; set; }
            public IEnumerable<int> EducationOrganizationIds { get; set; }
        }
    }
}
