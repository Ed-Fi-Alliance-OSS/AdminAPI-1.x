// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;

namespace EdFi.Ods.AdminApp.Management.Instances
{
    public class RegisterOdsInstanceCommand
    {
        private readonly IOdsInstanceFirstTimeSetupService _odsInstanceFirstTimeSetupService;
        private readonly AdminAppIdentityDbContext _identity;
        private readonly ISetCurrentSchoolYearCommand _setCurrentSchoolYear;
        private readonly IInferInstanceService _inferInstanceService;

        public RegisterOdsInstanceCommand(IOdsInstanceFirstTimeSetupService odsInstanceFirstTimeSetupService
            , AdminAppIdentityDbContext identity
            , ISetCurrentSchoolYearCommand setCurrentSchoolYear
            , IInferInstanceService inferInstanceService)
        {
            _odsInstanceFirstTimeSetupService = odsInstanceFirstTimeSetupService;
            _identity = identity;
            _setCurrentSchoolYear = setCurrentSchoolYear;
            _inferInstanceService = inferInstanceService;
        }

        public async Task<int> Execute(IRegisterOdsInstanceModel instance, ApiMode mode, string userId, CloudOdsClaimSet cloudOdsClaimSet = null)
        {
            var instanceName = instance.NumericSuffix.Value.ToString();

            var newInstance = new OdsInstanceRegistration
            {
                Name = instanceName,
                DatabaseName = _inferInstanceService.DatabaseName(instance.NumericSuffix.Value, mode),
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
    }

    public interface IRegisterOdsInstanceModel
    {
        int? NumericSuffix { get; }
        string Description { get; }
    }
}
