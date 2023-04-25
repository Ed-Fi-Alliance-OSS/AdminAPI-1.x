// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Ods.Reports;
using EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Management.Tests.User;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;
using Moq;
using NUnit.Framework;
using Shouldly;
using Microsoft.Extensions.Options;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.TestingHelper;
using static EdFi.Ods.AdminApp.Management.Tests.Instance.InstanceTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.Instance
{
    [TestFixture]
    public class RegisterOdsInstanceCommandTests: AdminAppDataTestBase
    {
        private Mock<IDatabaseValidationService> _databaseValidationService;
        private Mock<ICloudOdsAdminAppSettingsApiModeProvider> _apiModeProvider;
        private Mock<IDatabaseConnectionProvider> _connectionProvider;
        private Mock<ISetCurrentSchoolYearCommand> _setCurrentSchoolYear;
        private readonly string _dbNamePrefix = "EdFi_Ods_";


        [SetUp]
        public void Init()
        {
            _databaseValidationService = new Mock<IDatabaseValidationService>();
            _databaseValidationService.Setup(x => x.IsValidDatabase(It.IsAny<int>(), It.IsAny<ApiMode>())).Returns(true);
            _apiModeProvider = new Mock<ICloudOdsAdminAppSettingsApiModeProvider>();
            _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);
            _connectionProvider =  new Mock<IDatabaseConnectionProvider>();
            _setCurrentSchoolYear = new Mock<ISetCurrentSchoolYearCommand>();
        }

        [Test]
        public async Task ShouldRegisterDistrictSpecificOdsInstance()
        {
            var apiMode = ApiMode.DistrictSpecific;

            ResetOdsInstanceRegistrations();
            var instanceName = "23456";
            const string description = "Test Description";
            var encryptedSecretConfigValue = "Encrypted string";
            

            using (var connection = GetDatabaseConnection(instanceName, _dbNamePrefix))
            {
                _connectionProvider.Setup(x => x.CreateNewConnection(23456, apiMode))
                    .Returns(connection);

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
                        var odsInstanceFirstTimeSetupService = GetOdsInstanceFirstTimeSetupService(encryptedSecretConfigValue, instanceName, database, apiMode);
                        var inferInstanceService = new InferInstanceService(_connectionProvider.Object);
                        
                        var command = new RegisterOdsInstanceCommand(odsInstanceFirstTimeSetupService, identity, _setCurrentSchoolYear.Object, inferInstanceService);
                        return await command.Execute(newInstance, apiMode, testUsername, new CloudOdsClaimSet());
                    });
                });

                var addedInstance = Query<OdsInstanceRegistration>(newInstanceId);
                var secretConfiguration = Transaction(database =>
                    database.SecretConfigurations.FirstOrDefault(x => x.OdsInstanceRegistrationId == newInstanceId));
                secretConfiguration.ShouldNotBeNull();
                secretConfiguration.EncryptedData.ShouldBe(encryptedSecretConfigValue);
                addedInstance.Name.ShouldBe(instanceName);
                addedInstance.Description.ShouldBe(newInstance.Description);
                addedInstance.DatabaseName.ShouldBe($"{_dbNamePrefix}{instanceName}");

                _setCurrentSchoolYear.Verify(
                    x => x.Execute(It.IsAny<string>(), It.IsAny<ApiMode>(), It.IsAny<short>()),
                    Times.Never());
            }
        }

        [Test]
        public async Task ShouldRegisterYearSpecificOdsInstance()
        {
            var apiMode = ApiMode.YearSpecific;

            ResetOdsInstanceRegistrations();
            var instanceName = "2022";
            const string description = "Test Description";
            var encryptedSecretConfigValue = "Encrypted string";

            using (var connection = GetDatabaseConnection(instanceName, _dbNamePrefix))
            {
                _connectionProvider.Setup(x => x.CreateNewConnection(2022, apiMode))
                    .Returns(connection);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 2022,
                    Description = description
                };

                var testUsername = UserTestSetup.SetupUsers(1).Single().Id;

                int newInstanceId = await ScopedAsync<AdminAppIdentityDbContext, int>(async identity =>
                {
                    return await ScopedAsync<AdminAppDbContext, int>(async database =>
                    {
                        var odsInstanceFirstTimeSetupService = GetOdsInstanceFirstTimeSetupService(encryptedSecretConfigValue, instanceName, database, apiMode);
                        var inferInstanceService = new InferInstanceService(_connectionProvider.Object);

                        var command = new RegisterOdsInstanceCommand(odsInstanceFirstTimeSetupService, identity, _setCurrentSchoolYear.Object, inferInstanceService);
                        return await command.Execute(newInstance, apiMode, testUsername, new CloudOdsClaimSet());
                    });
                });

                var addedInstance = Query<OdsInstanceRegistration>(newInstanceId);
                var secretConfiguration = Transaction(database =>
                    database.SecretConfigurations.FirstOrDefault(x => x.OdsInstanceRegistrationId == newInstanceId));
                secretConfiguration.ShouldNotBeNull();
                secretConfiguration.EncryptedData.ShouldBe(encryptedSecretConfigValue);
                addedInstance.Name.ShouldBe(instanceName);
                addedInstance.Description.ShouldBe(newInstance.Description);
                addedInstance.DatabaseName.ShouldBe($"{_dbNamePrefix}{instanceName}");

                _setCurrentSchoolYear.Verify(x => x.Execute("2022", apiMode, 2022), Times.Once);
                _setCurrentSchoolYear.VerifyNoOtherCalls();
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
                new SqlConnectionStringBuilder(connectionString) {InitialCatalog = databaseName };

            var connection = new SqlConnection(sqlConnectionBuilder.ConnectionString);
            return connection;
        }

        private OdsInstanceFirstTimeSetupService GetOdsInstanceFirstTimeSetupService(string encryptedSecretConfigValue,
            string instanceName, AdminAppDbContext database, ApiMode apiMode)
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
                It.IsAny<string>(), apiMode)).ReturnsAsync(new ApplicationCreateResult());
            var odsInstanceFirstTimeSetupService = new OdsInstanceFirstTimeSetupService(odsSecretConfigurationProvider,
                mockFirstTimeSetupService.Object, mockUsersContext.Object, mockReportViewsSetUp.Object, database, options);
            return odsInstanceFirstTimeSetupService;
        }

        [Test]
        public void ShouldNotRegisterInstanceIfRequiredFieldsEmpty()
        {
            ResetOdsInstanceRegistrations();

            var newInstance = new RegisterOdsInstanceModel();

            _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);

            Scoped<AdminAppDbContext>(database =>
            {
                new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, _databaseValidationService.Object)
                    .ShouldNotValidate(newInstance,
                        "'ODS Instance District / EdOrg Id' must not be empty.",
                        "'ODS Instance Description' must not be empty.");
            });
        }

        [TestCase(null)]
        [TestCase(0)]
        [TestCase(-1)]
        public void ShouldBeInvalidDistrictOrEdOrgId(int invalidId)
        {           
            var invalidInstanceName = invalidId.ToString();

            using (var connection1 = GetDatabaseConnection(invalidInstanceName, _dbNamePrefix))
            using (var connection2 = GetDatabaseConnection(invalidInstanceName, _dbNamePrefix))
            {
                _connectionProvider
                    .SetupSequence(x => x.CreateNewConnection(invalidId, ApiMode.DistrictSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = invalidId,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, _databaseValidationService.Object).ShouldNotValidate(newInstance,
                            "'ODS Instance District / EdOrg Id' must be a valid positive integer.");
                });
            }
        }

        [TestCase(null)]
        [TestCase(3099)]
        [TestCase(1800)]
        public void ShouldBeInvalidSchoolYear(int? invalidSchoolYear)
        {
            var invalidInstanceName = invalidSchoolYear.ToString();

            using (var connection1 = GetDatabaseConnection(invalidInstanceName, _dbNamePrefix))
            using (var connection2 = GetDatabaseConnection(invalidInstanceName, _dbNamePrefix))
            {
                _connectionProvider.SetupSequence(
                        x => x.CreateNewConnection(invalidSchoolYear ?? 0, ApiMode.YearSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.YearSpecific);
                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = invalidSchoolYear,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, _databaseValidationService.Object)
                        .ShouldNotValidate(newInstance,
                            invalidSchoolYear == null
                            ? "'ODS Instance School Year' must not be empty."
                            : "'ODS Instance School Year' must be between 1900 and 2099.");
                });
            }
        }

        [Test]
        public void ShouldNotRegisterInstanceIfOdsInstanceIdentifierNotUniqueOnYearSpecificMode()
        {
            var instanceName = "2020";
            ResetOdsInstanceRegistrations();
            SetupOdsInstanceRegistration(instanceName);
            _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.YearSpecific);

            using (var connection1 = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider.SetupSequence(x => x.CreateNewConnection(2020, ApiMode.YearSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 2020,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, _databaseValidationService.Object)
                        .ShouldNotValidate(newInstance,
                            "An instance for this school year already exists.");
                });
            }
        }

        [Test]
        public void ShouldNotRegisterInstanceIfOdsInstanceIdentifierNotUniqueOnDistrictSpecificMode()
        {
            var instanceName = "8787877";
            ResetOdsInstanceRegistrations();
            SetupOdsInstanceRegistration(instanceName);

            using (var connection1 = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider
                    .SetupSequence(x => x.CreateNewConnection(8787877, ApiMode.DistrictSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 8787877,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(
                            database, _apiModeProvider.Object, _databaseValidationService.Object)
                        .ShouldNotValidate(
                            newInstance,
                            "An instance for this Education Organization / District Id already exists.");
                });
            }
        }

        [Test]
        public void ShouldNotRegisterInstanceIfOdsInstanceIdentifierAssociatedDbDoesNotExistsOnDistrictSpecificMode()
        {
            const int odsInstanceNumericSuffix = 8787877;
            const string instanceName = "8787877";

            var mockDatabaseValidationService = new Mock<IDatabaseValidationService>();
            mockDatabaseValidationService.Setup(x => x.IsValidDatabase(odsInstanceNumericSuffix, ApiMode.DistrictSpecific))
                .Returns(false);

            using (var connection1 = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider.SetupSequence(
                        x => x.CreateNewConnection(odsInstanceNumericSuffix, ApiMode.DistrictSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = odsInstanceNumericSuffix,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, mockDatabaseValidationService.Object)
                        .ShouldNotValidate(newInstance,
                            "Could not connect to an ODS instance database for this Education Organization / District Id.");
                });
            }
        }

        [Test]
        public void ShouldNotRegisterInstanceIfOdsInstanceIdentifierAssociatedDbDoesNotExistsOnYearSpecificMode()
        {
            const int odsInstanceNumericSuffix = 2020;
            const string instanceName = "2020";

            var mockDatabaseValidationService = new Mock<IDatabaseValidationService>();
            mockDatabaseValidationService.Setup(x => x.IsValidDatabase(odsInstanceNumericSuffix, ApiMode.YearSpecific))
                .Returns(false);

            using (var connection1 = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider.SetupSequence(
                        x => x.CreateNewConnection(odsInstanceNumericSuffix, ApiMode.YearSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.YearSpecific);
                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = odsInstanceNumericSuffix,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, mockDatabaseValidationService.Object)
                        .ShouldNotValidate(newInstance,
                            "Could not connect to an ODS instance database for this school year.");
                });
            }
        }

        [Test]
        public void ShouldReturnValidationMessageWithYear()
        {
            const int odsInstanceNumericSuffix = 2020;
            const string instanceName = "2020";

            var mockDatabaseValidationService = new Mock<IDatabaseValidationService>();
            mockDatabaseValidationService.Setup(x => x.IsValidDatabase(odsInstanceNumericSuffix, ApiMode.YearSpecific))
                .Returns(false);

            using (var connection1 = GetDatabaseConnection(instanceName))
                using (var connection2 = GetDatabaseConnection(instanceName))
                {
                    _connectionProvider.SetupSequence(
                            x => x.CreateNewConnection(odsInstanceNumericSuffix, ApiMode.YearSpecific))
                        .Returns(connection1)
                        .Returns(connection2);

                    _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.YearSpecific);
                    var newInstance = new RegisterOdsInstanceModel
                    {
                        NumericSuffix = odsInstanceNumericSuffix,
                        Description = Sample("Description")
                    };

                    Scoped<AdminAppDbContext>(database =>
                    {
                        new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, mockDatabaseValidationService.Object, true)
                            .ShouldNotValidate(newInstance,
                                $"Could not connect to an ODS instance database for this school year ({odsInstanceNumericSuffix}).");
                    });
                }
        }

        [Test]
        public void ShouldReturnValidationMessageWithDistrictId()
        {
            var instanceName = "8787877";
            ResetOdsInstanceRegistrations();
            SetupOdsInstanceRegistration(instanceName);

            using (var connection1 = GetDatabaseConnection(instanceName))
                using (var connection2 = GetDatabaseConnection(instanceName))
                {
                    _connectionProvider
                        .SetupSequence(x => x.CreateNewConnection(8787877, ApiMode.DistrictSpecific))
                        .Returns(connection1)
                        .Returns(connection2);

                    var newInstance = new RegisterOdsInstanceModel
                    {
                        NumericSuffix = 8787877,
                        Description = Sample("Description")
                    };

                    Scoped<AdminAppDbContext>(database =>
                    {
                        new RegisterOdsInstanceModelValidator(
                                database, _apiModeProvider.Object, _databaseValidationService.Object, true)
                            .ShouldNotValidate(
                                newInstance,
                                "An instance for this Education Organization / District Id (8787877) already exists.");
                    });
                }
        }

        [Test]
        public void ShouldReturnValidationMessageWithDistrictIdAndDescription()
        {
            var districtId = "8787877";
            var instanceName = "8787877";
            ResetOdsInstanceRegistrations();
            var createdInstance = SetupOdsInstanceRegistration(instanceName);

            using (var connection1 = GetDatabaseConnection(districtId))
                using (var connection2 = GetDatabaseConnection(districtId))
                {
                    _connectionProvider
                        .SetupSequence(x => x.CreateNewConnection(It.IsAny<int>(), ApiMode.DistrictSpecific))
                        .Returns(connection1)
                        .Returns(connection2);

                    var newInstance = new RegisterOdsInstanceModel
                    {
                        NumericSuffix = 8787878,
                        Description = createdInstance.Description
                    };

                    Scoped<AdminAppDbContext>(database =>
                    {
                        new RegisterOdsInstanceModelValidator(
                                database, _apiModeProvider.Object, _databaseValidationService.Object, true)
                            .ShouldNotValidate(newInstance,
                                $"An instance with this description (Education Organization / District Id: 8787878, Description: {newInstance.Description}) already exists.");
                    });
            }
        }

        [Test]
        public void ShouldNotRegisterInstanceIfOdsInstanceDescriptionNotUnique()
        {
            ResetOdsInstanceRegistrations();
            var instance = SetupOdsInstanceRegistration("8787877");

            var instanceName = "7878787";

            using (var connection1 = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider
                    .SetupSequence(x => x.CreateNewConnection(7878787, ApiMode.DistrictSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 7878787,
                    Description = instance.Description
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, _databaseValidationService.Object)
                        .ShouldNotValidate(newInstance, "An instance with this description already exists.");
                });
            }
        }

        [Test]
        public void ShouldValidateValidInstanceOnDistrictSpecificMode()
        {
            ResetOdsInstanceRegistrations();

            var instanceName = "7878787";

            using (var connection1 = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider
                    .SetupSequence(x => x.CreateNewConnection(7878787, ApiMode.DistrictSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 7878787,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, _databaseValidationService.Object)
                        .ShouldValidate(newInstance);
                });
            }
        }

        [Test]
        public void ShouldValidateValidInstanceOnYearSpecificMode()
        {
            ResetOdsInstanceRegistrations();

            var instanceName = "2020";

            using (var connection1 = GetDatabaseConnection(instanceName))
            using (var connection2 = GetDatabaseConnection(instanceName))
            {
                _connectionProvider.SetupSequence(x => x.CreateNewConnection(2020, ApiMode.YearSpecific))
                    .Returns(connection1)
                    .Returns(connection2);

                _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.YearSpecific);

                var newInstance = new RegisterOdsInstanceModel
                {
                    NumericSuffix = 2020,
                    Description = Sample("Description")
                };

                Scoped<AdminAppDbContext>(database =>
                {
                    new RegisterOdsInstanceModelValidator(database, _apiModeProvider.Object, _databaseValidationService.Object)
                        .ShouldValidate(newInstance);
                });
            }
        }
    }
}
