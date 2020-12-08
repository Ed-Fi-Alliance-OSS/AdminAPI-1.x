using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Database.Ods.Reports;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Management.Tests.User;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EdFi.Ods.AdminApp.Management.Tests.Instance.InstanceTestSetup;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.TestingHelper;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class BulkRegisterOdsInstanceCommandTests : AdminAppDataTestBase
    {
        private Mock<IDatabaseValidationService> _databaseValidationService;
        private Mock<ICloudOdsAdminAppSettingsApiModeProvider> _apiModeProvider;
        private Mock<IDatabaseConnectionProvider> _connectionProvider;
        private Mock<IBulkRegisterOdsInstancesFiltrationService> _dataFiltrationService;

        [SetUp]
        public void Init()
        {
            _databaseValidationService = new Mock<IDatabaseValidationService>();
            _databaseValidationService.Setup(x => x.IsValidDatabase(It.IsAny<int>(), It.IsAny<ApiMode>())).Returns(true);
            _apiModeProvider = new Mock<ICloudOdsAdminAppSettingsApiModeProvider>();
            _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);
            _connectionProvider = new Mock<IDatabaseConnectionProvider>();
            _dataFiltrationService = new Mock<IBulkRegisterOdsInstancesFiltrationService>();
        }

        [Test]
        public async Task ShouldRegisterOneNewOdsInstanceInBulk()
        {
            ResetOdsInstanceRegistrations();
            var instanceName = "TestInstance_23456";
            const string description = "Test Description";
            var encryptedSecretConfigValue = "Encrypted string";

            using (var connection = GetDatabaseConnection(instanceName))
            {
                _connectionProvider.Setup(x => x.CreateNewConnection(23456, ApiMode.DistrictSpecific))
                    .Returns(connection);

                List<RegisterOdsInstanceModel> odsInstancesToRegister = new List<RegisterOdsInstanceModel>();
                var newInstance1 = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 23456,
                    Description = description
                };

                odsInstancesToRegister.Add(newInstance1);

                var testUsername = UserTestSetup.SetupUsers(1).Single().Id;

                var newInstances = await ScopedAsync<AdminAppIdentityDbContext, IEnumerable<BulkRegisterOdsInstancesResult>>(async identity =>
                {
                    return await ScopedAsync<AdminAppDbContext, IEnumerable<BulkRegisterOdsInstancesResult>>(async database =>
                    {
                        var odsInstanceFirstTimeSetupService = GetOdsInstanceFirstTimeSetupService(encryptedSecretConfigValue, instanceName, database);
                        RegisterOdsInstanceCommand registerOdsInstanceCommand = new RegisterOdsInstanceCommand(odsInstanceFirstTimeSetupService, _connectionProvider.Object, identity);

                        var command = new BulkRegisterOdsInstancesCommand(registerOdsInstanceCommand, _dataFiltrationService.Object);
                        return await command.Execute(odsInstancesToRegister, odsInstancesToRegister, ApiMode.DistrictSpecific, testUsername, new CloudOdsClaimSet());
                    });
                });

                var addedInstance = Query<OdsInstanceRegistration>(newInstances.FirstOrDefault().OdsInstanceRegisteredId);
                var secretConfiguration = Transaction(database =>
                    database.SecretConfigurations.FirstOrDefault(x => x.OdsInstanceRegistrationId == newInstances.FirstOrDefault().OdsInstanceRegisteredId));
                secretConfiguration.ShouldNotBeNull();
                secretConfiguration.EncryptedData.ShouldBe(encryptedSecretConfigValue);
                addedInstance.Name.ShouldBe(instanceName);
                addedInstance.Description.ShouldBe(newInstance1.Description);
            }

        }

        [Test]
        public async Task BulkShouldNotRegisterOneOdsInstancePreviouslyRegistered()
        {
            ResetOdsInstanceRegistrations();
            var instanceName = "TestInstance_23456";
            const string description = "Test Description";
            var encryptedSecretConfigValue = "Encrypted string";

            using (var connection = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            using (var connection3 = GetDatabaseConnection(instanceName))
            using (var connection4 = GetDatabaseConnection(instanceName))
            using (var connection5 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider
                    .SetupSequence(x => x.CreateNewConnection(23456, ApiMode.DistrictSpecific))
                    .Returns(connection)
                    .Returns(connection2)
                    .Returns(connection3)
                    .Returns(connection4)
                    .Returns(connection5);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 23456,
                    Description = description
                };

                var testUsername = UserTestSetup.SetupUsers(1).Single().Id;

                int newInstanceId = await ScopedAsync<AdminAppIdentityDbContext, int>(async identity =>
                {
                    return await ScopedAsync<AdminAppDbContext, int>(async database =>
                    {
                        var odsInstanceFirstTimeSetupService = GetOdsInstanceFirstTimeSetupService(encryptedSecretConfigValue, instanceName, database);

                        var command = new RegisterOdsInstanceCommand(odsInstanceFirstTimeSetupService, _connectionProvider.Object, identity);
                        return await command.Execute(newInstance, ApiMode.DistrictSpecific, testUsername, new CloudOdsClaimSet());
                    });
                });

                List<RegisterOdsInstanceModel> odsInstancesToRegister = new List<RegisterOdsInstanceModel>();
                var repeatedInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 23456,
                    Description = description
                };

                odsInstancesToRegister.Add(repeatedInstance);

                var newInstances = await ScopedAsync<AdminAppIdentityDbContext, IEnumerable<BulkRegisterOdsInstancesResult>>(async identity =>
                {
                    return await ScopedAsync<AdminAppDbContext, IEnumerable<BulkRegisterOdsInstancesResult>>(async database =>
                    {
                        var odsInstanceFirstTimeSetupService = GetOdsInstanceFirstTimeSetupService(encryptedSecretConfigValue, instanceName, database);
                        RegisterOdsInstanceCommand registerOdsInstanceCommand = new RegisterOdsInstanceCommand(odsInstanceFirstTimeSetupService, _connectionProvider.Object, identity);

                        var command = new BulkRegisterOdsInstancesCommand(registerOdsInstanceCommand, _dataFiltrationService.Object);
                        return await command.Execute(odsInstancesToRegister, new List<RegisterOdsInstanceModel>(), ApiMode.DistrictSpecific, testUsername, new CloudOdsClaimSet());
                    });
                });

                var addedInstance = Query<OdsInstanceRegistration>(newInstances.FirstOrDefault().OdsInstanceRegisteredId);
                var secretConfiguration = Transaction(database =>
                    database.SecretConfigurations.FirstOrDefault(x => x.OdsInstanceRegistrationId == newInstances.FirstOrDefault().OdsInstanceRegisteredId));

                newInstances.FirstOrDefault().ShouldNotBeNull();
                newInstances.FirstOrDefault().NumericSuffix.ShouldBe(repeatedInstance.NumericSuffix.ToString());
                newInstances.FirstOrDefault().Description.ShouldBe(repeatedInstance.Description);
                newInstances.FirstOrDefault().OdsInstanceRegisteredId.ShouldBe(0);
                newInstances.FirstOrDefault().IndividualInstanceResult.ShouldBe(IndividualInstanceResult.Skipped);
            }

        }

        private static SqlConnection GetDatabaseConnection(string instanceName, string prefix = "")
        {
            var connectionString = "Data Source=.\\;Integrated Security=True";

            var databaseName = instanceName;

            if (prefix.Length > 0)
            {
                databaseName = prefix + instanceName;
            }

            var sqlConnectionBuilder =
                new SqlConnectionStringBuilder(connectionString) { InitialCatalog = databaseName };

            var connection = new SqlConnection(sqlConnectionBuilder.ConnectionString);
            return connection;
        }

        private OdsInstanceFirstTimeSetupService GetOdsInstanceFirstTimeSetupService(string encryptedSecretConfigValue,
            string instanceName, AdminAppDbContext database)
        {
            var appSettings = new Mock<IOptions<AppSettings>>();
            appSettings.Setup(x => x.Value).Returns(new AppSettings());
            var options = appSettings.Object;

            var mockStringEncryptorService = new Mock<IStringEncryptorService>();
            mockStringEncryptorService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(encryptedSecretConfigValue);
            var odsSecretConfigurationProvider = new OdsSecretConfigurationProvider(mockStringEncryptorService.Object, database);

            var mockFirstTimeSetupService = new Mock<IFirstTimeSetupService>();
            var mockReportViewsSetUp = new Mock<IReportViewsSetUp>();
            var mockUsersContext = new Mock<IUsersContext>();
            mockFirstTimeSetupService.Setup(x => x.CreateAdminAppInAdminDatabase(It.IsAny<string>(), instanceName,
                It.IsAny<string>(), ApiMode.DistrictSpecific)).ReturnsAsync(new ApplicationCreateResult());
            var odsInstanceFirstTimeSetupService = new OdsInstanceFirstTimeSetupService(odsSecretConfigurationProvider,
                mockFirstTimeSetupService.Object, mockUsersContext.Object, mockReportViewsSetUp.Object, database, options);
            return odsInstanceFirstTimeSetupService;
        }

    }
}
