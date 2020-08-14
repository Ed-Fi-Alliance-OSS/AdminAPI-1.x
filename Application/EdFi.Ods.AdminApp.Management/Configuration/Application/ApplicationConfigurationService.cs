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

        public ApplicationConfigurationService(AdminAppDbContext database) => _database = database;

        public bool AllowUserRegistration()
        {
            if (!AnyUsersExist())
                return true;

            return _database
                       .ApplicationConfigurations
                       .SingleOrDefault()?
                       .AllowUserRegistration ?? false;
        }

        public bool IsFirstTimeSetUpCompleted()
        {
            using (var database = new AdminAppDbContext())
            {
                return database
                           .ApplicationConfigurations
                           .SingleOrDefault()?
                           .FirstTimeSetUpCompleted ?? false;
            }
        }

        public void UpdateFirstTimeSetUpStatus(bool setUpCompleted)
        {
            var config = _database.EnsureSingle<ApplicationConfiguration>();
            config.FirstTimeSetUpCompleted = setUpCompleted;
           _database.SaveChanges();
        }

        private static bool AnyUsersExist()
        {
            using (var identity = AdminAppIdentityDbContext.Create())
                return identity.Users.Any();
        }
    }
}