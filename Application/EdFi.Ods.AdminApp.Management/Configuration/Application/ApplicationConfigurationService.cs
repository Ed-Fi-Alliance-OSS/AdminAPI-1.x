// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Management.Configuration.Application
{
    public class ApplicationConfigurationService
    {
        private readonly AdminAppDbContext _database;
        private readonly AdminAppIdentityDbContext _identity;
        private readonly AppSettings _appSettings;

        public ApplicationConfigurationService(AdminAppDbContext database, AdminAppIdentityDbContext identity, IOptions<AppSettings> appSettings)
        {
            _database = database;
            _identity = identity;
            _appSettings = appSettings.Value;
        }

        public bool AllowUserRegistration()
        {
            if (!AnyUsersExist())
                return true;

            return _database
                       .ApplicationConfigurations
                       .SingleOrDefault()?
                       .AllowUserRegistration ?? false;
        }

        public void UpdateFirstTimeSetUpStatus(bool setUpCompleted)
        {
            var config = _database.EnsureSingle<ApplicationConfiguration>();
            config.FirstTimeSetUpCompleted = setUpCompleted;
            config.EnableProductImprovement = _appSettings.EnableProductImprovementSettings;
           _database.SaveChanges();
        }

        public bool IsProductImprovementEnabled()
        {
            return IsProductImprovementEnabled(out _);
        }

        public bool IsProductImprovementEnabled(out string productRegistrationId)
        {
            var config = _database.EnsureSingle<ApplicationConfiguration>();
            var enableProductImprovement = config.EnableProductImprovement;

            productRegistrationId = config.ProductRegistrationId;

            return _appSettings.EnableProductImprovementSettings && enableProductImprovement;
        }

        public void EnableProductImprovement(bool enableProductImprovement, string productRegistrationId)
        {
            var config = _database.EnsureSingle<ApplicationConfiguration>();
            config.EnableProductImprovement = _appSettings.EnableProductImprovementSettings && enableProductImprovement;
            config.ProductRegistrationId = (productRegistrationId ?? "").Trim();
            _database.SaveChanges();
        }

        private bool AnyUsersExist()
        {
            return _identity.Users.Any();
        }
    }
}
