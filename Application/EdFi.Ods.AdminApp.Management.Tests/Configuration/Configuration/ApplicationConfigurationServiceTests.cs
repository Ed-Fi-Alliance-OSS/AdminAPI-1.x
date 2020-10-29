// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Configuration.Configuration
{
    public class ApplicationConfigurationServiceTests : AdminAppDataTestBase
    {
        [Test]
        public void ShouldAllowFirstRegistrationRegardlessOfConfiguration()
        {
            EnsureZeroUsers();

            EnsureZeroApplicationConfiguration();
            AllowUserRegistrations().ShouldBe(true);

            EnsureOneApplicationConfiguration(allowRegistration: true);
            AllowUserRegistrations().ShouldBe(true);

            EnsureOneApplicationConfiguration(allowRegistration: false);
            AllowUserRegistrations().ShouldBe(true);
        }

        [Test]
        public void ShouldDisallowRegistrationUponFirstRegistration()
        {
            //Until the first user explicitly chooses to allow
            //further registration, assume that it should be disallowed.

            EnsureOneUser();
            EnsureZeroApplicationConfiguration();
            AllowUserRegistrations().ShouldBe(false);
        }

        [Test]
        public void ShouldAllowRegistrationByConfigurationAfterFirstRegistration()
        {
            EnsureOneUser();

            EnsureOneApplicationConfiguration(allowRegistration: true);
            AllowUserRegistrations().ShouldBe(true);

            EnsureOneApplicationConfiguration(allowRegistration: false);
            AllowUserRegistrations().ShouldBe(false);
        }

        private bool AllowUserRegistrations()
        {
            return Transaction(database =>
            {
                using (var identity = AdminAppIdentityDbContext.Create())
                    return new ApplicationConfigurationService(database, identity).AllowUserRegistration();
            });
        }

        private void EnsureZeroApplicationConfiguration()
        {
            DeleteAll<ApplicationConfiguration>();
        }

        private void EnsureOneApplicationConfiguration(bool allowRegistration)
        {
            Transaction(database =>
            {
                var config = database.EnsureSingle<ApplicationConfiguration>();
                config.AllowUserRegistration = allowRegistration;
            });
        }

        private static void EnsureZeroUsers()
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                foreach (var entity in database.Users)
                    database.Users.Remove(entity);
                database.SaveChanges();
            }
        }

        private static void EnsureOneUser()
        {
            EnsureZeroUsers();

            using (var database = AdminAppIdentityDbContext.Create())
            {
                database.Users.Add(new AdminAppUser("testUser"));
                database.SaveChanges();
            }
        }
    }
}
