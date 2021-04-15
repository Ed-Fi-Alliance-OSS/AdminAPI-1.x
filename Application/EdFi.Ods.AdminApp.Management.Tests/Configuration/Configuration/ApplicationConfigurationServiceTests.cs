// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

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

            EnsureOneApplicationConfiguration(allowRegistration: true, enableProductImprovement: false);
            AllowUserRegistrations().ShouldBe(true);

            EnsureOneApplicationConfiguration(allowRegistration: false, enableProductImprovement: false);
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

            EnsureOneApplicationConfiguration(allowRegistration: true, enableProductImprovement: false);
            AllowUserRegistrations().ShouldBe(true);

            EnsureOneApplicationConfiguration(allowRegistration: false, enableProductImprovement: false);
            AllowUserRegistrations().ShouldBe(false);
        }

        [Test]
        public void ShouldViewAndSaveEnableProductImprovement()
        {
            EnsureOneApplicationConfiguration(allowRegistration: false, enableProductImprovement: false);
            IsProductImprovementEnabled().ShouldBe(false);
            Count<ApplicationConfiguration>().ShouldBe(1);

            EnableProductImprovement(enableProductImprovement: true);
            IsProductImprovementEnabled().ShouldBe(true);
            Count<ApplicationConfiguration>().ShouldBe(1);

            EnableProductImprovement(enableProductImprovement: false);
            IsProductImprovementEnabled().ShouldBe(false);
            Count<ApplicationConfiguration>().ShouldBe(1);
        }

        private bool AllowUserRegistrations()
        {
            return Transaction(database =>
            {
                bool allowUserRegistration = false;

                Scoped<AdminAppIdentityDbContext>(identity =>
                {
                    allowUserRegistration = new ApplicationConfigurationService(database, identity).AllowUserRegistration();
                });

                return allowUserRegistration;
            });
        }

        private bool IsProductImprovementEnabled()
        {
            return Transaction(database =>
            {
                bool enableProductImprovement = false;

                Scoped<AdminAppIdentityDbContext>(identity =>
                {
                    enableProductImprovement = new ApplicationConfigurationService(database, identity)
                        .IsProductImprovementEnabled();
                });

                return enableProductImprovement;
            });
        }

        private void EnableProductImprovement(bool enableProductImprovement)
        {
            Transaction(database =>
            {
                Scoped<AdminAppIdentityDbContext>(identity =>
                {
                    new ApplicationConfigurationService(database, identity).EnableProductImprovement(
                        enableProductImprovement);
                });
            });
        }

        private void EnsureZeroApplicationConfiguration()
        {
            DeleteAll<ApplicationConfiguration>();
        }

        private void EnsureOneApplicationConfiguration(bool allowRegistration, bool enableProductImprovement)
        {
            Transaction(database =>
            {
                var config = database.EnsureSingle<ApplicationConfiguration>();
                config.AllowUserRegistration = allowRegistration;
                config.EnableProductImprovement = enableProductImprovement;
            });
        }

        private static void EnsureZeroUsers()
        {
            Scoped<AdminAppIdentityDbContext>(database =>
            {
                foreach (var entity in database.Users)
                    database.Users.Remove(entity);
                database.SaveChanges();
            });
        }

        private static void EnsureOneUser()
        {
            EnsureZeroUsers();

            Scoped<AdminAppIdentityDbContext>(database =>
            {
                database.Users.Add(new AdminAppUser("testUser"));
                database.SaveChanges();
            });
        }
    }
}
