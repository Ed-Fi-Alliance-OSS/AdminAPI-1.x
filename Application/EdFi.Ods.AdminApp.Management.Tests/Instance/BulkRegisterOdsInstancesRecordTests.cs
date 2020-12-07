// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;
using Moq;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.Instance
{
    [TestFixture]
    public class BulkRegisterOdsInstancesRecordTests: AdminAppDataTestBase
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
            _connectionProvider =  new Mock<IDatabaseConnectionProvider>();
            _dataFiltrationService = new Mock<IBulkRegisterOdsInstancesFiltrationService>();
        }

        [Test]
        public void ShouldNotBulkRegisterInstanceIfFileContainsDuplicateRecords()
        {
            var dataRecords = GetSampleDataRecords(numberOfDuplicates: 2);

            _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);

            Scoped<AdminAppDbContext>(database =>
            {
                var bulkRegisterOdsInstancesModelValidator = new BulkRegisterOdsInstancesModelValidator(
                    database, _apiModeProvider.Object, _databaseValidationService.Object,
                    _connectionProvider.Object, _dataFiltrationService.Object);

                bulkRegisterOdsInstancesModelValidator.GetDuplicates(dataRecords, out var duplicateNumericSuffixes, out var duplicateDescriptions);

                duplicateNumericSuffixes.Count.ShouldBe(2);

                duplicateDescriptions.Count.ShouldBe(2);
            });
        }

        [Test]
        public void ShouldBulkRegisterInstanceIfFileDoesNotContainDuplicateRecords()
        {
            var dataRecords = GetSampleDataRecords();

            _apiModeProvider.Setup(x => x.GetApiMode()).Returns(ApiMode.DistrictSpecific);

            Scoped<AdminAppDbContext>(database =>
            {
                var bulkRegisterOdsInstancesModelValidator = new BulkRegisterOdsInstancesModelValidator(
                    database, _apiModeProvider.Object, _databaseValidationService.Object,
                    _connectionProvider.Object, _dataFiltrationService.Object);

                bulkRegisterOdsInstancesModelValidator.GetDuplicates(dataRecords, out var duplicateNumericSuffixes, out var duplicateDescriptions);

                duplicateNumericSuffixes.Count.ShouldBe(0);

                duplicateDescriptions.Count.ShouldBe(0);
            });
        }

        private List<RegisterOdsInstanceModel> GetSampleDataRecords(int numberOfRecords = 5, int numberOfDuplicates = 0)
        {
            var dataRecords = new List<RegisterOdsInstanceModel>();
            for (var i = 1; i <= numberOfRecords; i++)
            {
                dataRecords.Add(new RegisterOdsInstanceModel
                {
                    NumericSuffix = 25590 + i,
                    Description = $"Test Description {i}"
                });
            }

            for (var i = 1; i <= numberOfDuplicates; i++)
            {
                dataRecords.Add(new RegisterOdsInstanceModel
                {
                    NumericSuffix = 25590 + i,
                    Description = $"Test Description {i}"
                });
            }

            return dataRecords;
        }
    }
}
