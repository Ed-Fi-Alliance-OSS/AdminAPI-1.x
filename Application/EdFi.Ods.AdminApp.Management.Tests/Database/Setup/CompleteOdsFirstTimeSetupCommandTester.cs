// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Management.OnPrem;
using EdFi.Security.DataAccess.Contexts;
using Moq;
using NUnit.Framework;
using Application = EdFi.Admin.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Setup
{
    [TestFixture]
    public class CompleteOdsFirstTimeSetupCommandTester
    {
        private Mock<IDatabaseConnectionProvider> _connectionProvider;

        [SetUp]
        public void Init()
        {
            _connectionProvider = new Mock<IDatabaseConnectionProvider>();
            var connectionString = "Data Source=.\\;Integrated Security=True";
            var sqlConnectionBuilder =
                new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "Ed-Fi_Ods" };
            using var connection = new SqlConnection(sqlConnectionBuilder.ConnectionString);
            _connectionProvider.Setup(x => x.CreateNewConnection(0, ApiMode.SharedInstance)).Returns(connection);
        }

        [Test]
        public async Task ShouldRunSqlConfigurator()
        {
            var odsSecretConfiguration = GetOdsSecretConfiguration();
            var odsSqlConfiguration = GetOdsSqlConfiguration();
            var setupConfig = GetFirstTimeSetupConfiguration();
            setupConfig.SqlConfiguration = odsSqlConfiguration;

            var mockOdsInstances = MockExtensions.EmptyMockDbSet<OdsInstance>();
            var mockVendors = MockExtensions.EmptyMockDbSet<Vendor>();
            var mockApps = MockExtensions.EmptyMockDbSet<Application>();

            var mockUsersContext = new Mock<IUsersContext>();
            mockUsersContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);
            mockUsersContext.Setup(c => c.Vendors).Returns(mockVendors.Object);
            mockUsersContext.Setup(c => c.Applications).Returns(mockApps.Object);

            var mockApplications = MockExtensions.EmptyMockDbSet<Security.DataAccess.Models.Application>();

            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Object.Applications = mockApplications.Object;

            var mockClaimSetConfigurator = new Mock<ICloudOdsClaimSetConfigurator>();
            mockClaimSetConfigurator.Setup(m => m.ApplyConfiguration(null)).Callback(() => { });

            var mockCloudOdsInstanceQuery = new Mock<IGetCloudOdsInstanceQuery>();
            mockCloudOdsInstanceQuery.Setup(a => a.Execute(It.IsAny<string>())).ReturnsAsync(GetDefaultInstance());

            var mockOdsSecretConfigurationProvider = new Mock<IOdsSecretConfigurationProvider>();
            
            mockOdsSecretConfigurationProvider.Setup(a => a.GetSecretConfiguration(null)).ReturnsAsync(odsSecretConfiguration);
            mockOdsSecretConfigurationProvider.Setup(a => a.SetSecretConfiguration(It.IsAny<OdsSecretConfiguration>(), null)).Returns(Task.CompletedTask);

            var mockFirstTimeSetupService = new Mock<IOdsInstanceFirstTimeSetupService>();

            var mockAssessmentVendorAdjustment = new Mock<IAssessmentVendorAdjustment>();

            var mockLearningStandardsSetup = new Mock<ILearningStandardsSetup>();
        
            var mockClaimSetCheckService = new Mock<IClaimSetCheckService>();

            var command = new CompleteOnPremFirstTimeSetupCommand(
                mockUsersContext.Object,
                mockSecurityContext.Object,
                mockClaimSetConfigurator.Object,
                mockFirstTimeSetupService.Object,
                mockAssessmentVendorAdjustment.Object,
                mockLearningStandardsSetup.Object,
                mockClaimSetCheckService.Object,
                _connectionProvider.Object);

            await command.Execute(GetOdsName(), GetClaimSet(), ApiMode.SharedInstance);
        }

        [Test]
        public async Task ShouldConfigureClaimSet()
        {
            var odsInstanceList = new List<OdsInstance>();
            var mockOdsInstances = MockExtensions.MockDbSet(odsInstanceList);

            var vendorsList = new List<Vendor>();
            var mockVendors = MockExtensions.MockDbSet(vendorsList);

            var appsList = new List<Application>();
            var mockApps = MockExtensions.MockDbSet(appsList);

            var mockUsersContext = new Mock<IUsersContext>();
            mockUsersContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);
            mockUsersContext.Setup(c => c.Vendors).Returns(mockVendors.Object);
            mockUsersContext.Setup(c => c.Applications).Returns(mockApps.Object);

            var mockApplications = MockExtensions.EmptyMockDbSet<Security.DataAccess.Models.Application>();

            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Object.Applications = mockApplications.Object;

            var mockClaimSetConfigurator = new Mock<ICloudOdsClaimSetConfigurator>();
            mockClaimSetConfigurator.Setup(m => m.ApplyConfiguration(null)).Callback(() => { });

            var mockCloudOdsInstanceQuery = new Mock<IGetCloudOdsInstanceQuery>();
            mockCloudOdsInstanceQuery.Setup(a => a.Execute(It.IsAny<string>())).ReturnsAsync(GetDefaultInstance());

            var mockOdsSecretConfigurationProvider = new Mock<IOdsSecretConfigurationProvider>();
            var odsSecretConfiguration = GetOdsSecretConfiguration();
            mockOdsSecretConfigurationProvider.Setup(a => a.GetSecretConfiguration(null)).ReturnsAsync(odsSecretConfiguration);
            mockOdsSecretConfigurationProvider.Setup(a => a.SetSecretConfiguration(It.IsAny<OdsSecretConfiguration>(), null)).Returns(Task.CompletedTask);

            var mockFirstTimeSetupService = new Mock<IOdsInstanceFirstTimeSetupService>();

            var mockAssessmentVendorAdjustment = new Mock<IAssessmentVendorAdjustment>();

            var mockLearningStandardsSetup = new Mock<ILearningStandardsSetup>();

            var mockClaimSetCheckService = new Mock<IClaimSetCheckService>();
            mockClaimSetCheckService.Setup(a => a.RequiredClaimSetsExist()).Returns(false);

            var command = new CompleteOnPremFirstTimeSetupCommand(
                mockUsersContext.Object,
                mockSecurityContext.Object,
                mockClaimSetConfigurator.Object,
                mockFirstTimeSetupService.Object,
                mockAssessmentVendorAdjustment.Object,
                mockLearningStandardsSetup.Object,
                mockClaimSetCheckService.Object,
                _connectionProvider.Object);

            await command.Execute(GetOdsName(), GetClaimSet(), ApiMode.SharedInstance);

            mockClaimSetConfigurator.Verify(x => x.ApplyConfiguration(It.Is<CloudOdsClaimSet>(c => c.ClaimSetName == CloudOdsAdminApp.InternalAdminAppClaimSet && c.ApplicationName == CloudOdsAdminApp.ApplicationName)), Times.Once);
        }

        #region MockGetters
        private OdsFirstTimeSetupConfiguration GetFirstTimeSetupConfiguration()
        {
            return new OdsFirstTimeSetupConfiguration
            {
                Components = GetMockComponents(),
                Name = GetOdsName(),
                SqlConfiguration = new OdsSqlConfiguration(),
                Version = GetDefaultInstance().Version,
                ClaimSet = GetClaimSet()
            };
        }

        private OdsSecretConfiguration GetOdsSecretConfiguration()
        {
            var odsSecretConfiguration = new OdsSecretConfiguration
            {
                ProductionApiKeyAndSecret = new OdsApiCredential("production", "api")
            };

            return odsSecretConfiguration;
        }

        private OdsSqlConfiguration GetOdsSqlConfiguration()
        {
            var odsSqlConfiguration = new OdsSqlConfiguration
            {
                HostName = "test",
                AdminCredentials = new OdsSqlAdminCredential { UserName = "test", Password = "test" },
                AdminAppCredentials = new OdsSqlCredential { UserName = "test", Password = "test" },
                ProductionApiCredentials = new OdsSqlCredential { UserName = "test", Password = "test" }
            };

            return odsSqlConfiguration;
        }

        private IEnumerable<OdsComponent> GetMockComponents()
        {
            return new List<OdsComponent>
            {
                new OdsComponent
                {
                    Name = "Api",
                    Environment = "Production",
                    Url = "http://fake.production.server",
                    Version = "0.0"
                },
                new OdsComponent
                {
                    Name = "AdminApp",
                    Environment = "Production",
                    Url = "http://fake.adminapp.server",
                    Version = "0.0"
                },
                new OdsComponent
                {
                    Name = "Swagger",
                    Environment = "Staging",
                    Url = "http://fake.swagger.server",
                    Version = "0.0"
                }
            };
        }

        private CloudOdsClaimSet GetClaimSet()
        {
            return new CloudOdsClaimSet
            {
                ApplicationName = CloudOdsAdminApp.ApplicationName,
                ClaimSetName = CloudOdsAdminApp.InternalAdminAppClaimSet
            };
        }

        private CloudOdsInstance GetDefaultInstance()
        {
            return new CloudOdsInstance
            {
                FriendlyName = GetOdsName(),
                InstanceType = "Instance Type",
                IsExtended = true,
                Name = "Name",
                Status = "Status",
                Version = "0.0"
            };
        }

        private string GetOdsName()
        {
            return "Test ODS Instance";
        }
        #endregion
    }
}
