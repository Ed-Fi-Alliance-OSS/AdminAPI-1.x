// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;

namespace EdFi.Ods.AdminApp.Management.Instances
{
    public class RegisterOdsInstanceCommand
    {
        private readonly IOdsInstanceFirstTimeSetupService _odsInstanceFirstTimeSetupService;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly AdminAppIdentityDbContext _identity;
        private readonly ISetCurrentSchoolYearCommand _setCurrentSchoolYear;

        public RegisterOdsInstanceCommand(IOdsInstanceFirstTimeSetupService odsInstanceFirstTimeSetupService
            , IDatabaseConnectionProvider connectionProvider
            , AdminAppIdentityDbContext identity
            , ISetCurrentSchoolYearCommand setCurrentSchoolYear)
        {
            _odsInstanceFirstTimeSetupService = odsInstanceFirstTimeSetupService;
            _connectionProvider = connectionProvider;
            _identity = identity;
            _setCurrentSchoolYear = setCurrentSchoolYear;
        }

        public async Task<int> Execute(IRegisterOdsInstanceModel instance, ApiMode mode, string userId, CloudOdsClaimSet cloudOdsClaimSet = null)
        {
            var instanceName = instance.NumericSuffix.Value.ToString();

            var newInstance = new OdsInstanceRegistration
            {
                Name = instanceName,
                DatabaseName = InferInstanceDatabaseName(instance.NumericSuffix.Value, mode),
                Description = instance.Description
            };
            await _odsInstanceFirstTimeSetupService.CompleteSetup(newInstance, cloudOdsClaimSet, mode);

            await _identity.UserOdsInstanceRegistrations.AddAsync(
                new UserOdsInstanceRegistration
                {
                    OdsInstanceRegistrationId = newInstance.Id,
                    UserId = userId
                });

            await _identity.SaveChangesAsync();

            if (mode == ApiMode.YearSpecific)
                _setCurrentSchoolYear.Execute(instanceName, mode, (short)instance.NumericSuffix.Value);

            return newInstance.Id;
        }

        private string InferInstanceDatabaseName(int odsInstanceNumericSuffix, ApiMode mode)
        {
            using var connection = _connectionProvider.CreateNewConnection(odsInstanceNumericSuffix, mode);

            return connection.Database;
        }
    }

    public interface IRegisterOdsInstanceModel
    {
        int? NumericSuffix { get; }
        string Description { get; }
    }
}
