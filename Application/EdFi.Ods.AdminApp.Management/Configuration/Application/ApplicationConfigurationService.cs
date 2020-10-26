// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Management.Configuration.Application
{
    public class ApplicationConfigurationService
    {
        private readonly AdminAppDbContext _database;
        private readonly AdminAppIdentityDbContext _identity;

        public ApplicationConfigurationService(AdminAppDbContext database, AdminAppIdentityDbContext identity)
        {
            _database = database;
            _identity = identity;
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
           _database.SaveChanges();
        }

        private bool AnyUsersExist()
        {
            return _identity.Users.Any();
        }
    }
}
