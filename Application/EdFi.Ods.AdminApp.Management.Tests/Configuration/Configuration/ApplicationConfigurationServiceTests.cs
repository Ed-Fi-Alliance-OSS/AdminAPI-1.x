// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
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

            EnsureOneApplicationConfiguration(allowRegistration: true, enableProductImprovement: false, productRegistrationId: "");
            AllowUserRegistrations().ShouldBe(true);

            EnsureOneApplicationConfiguration(allowRegistration: false, enableProductImprovement: false, productRegistrationId: "");
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

            EnsureOneApplicationConfiguration(allowRegistration: true, enableProductImprovement: false, productRegistrationId: "");
            AllowUserRegistrations().ShouldBe(true);

            EnsureOneApplicationConfiguration(allowRegistration: false, enableProductImprovement: false, productRegistrationId: "");
            AllowUserRegistrations().ShouldBe(false);
        }

        [Test]
        public void ShouldViewAndSaveEnableProductImprovementWithOptionalProductRegistrationId()
        {
            EnsureOneApplicationConfiguration(allowRegistration: false, enableProductImprovement: false, productRegistrationId: "");
            IsProductImprovementEnabled().ShouldBe((false, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);

            EnableProductImprovement(enableProductImprovement: true, productRegistrationId: "");
            IsProductImprovementEnabled().ShouldBe((true, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);

            var productRegistrationId = Guid.NewGuid().ToString();
            EnableProductImprovement(enableProductImprovement: true, productRegistrationId: productRegistrationId);
            IsProductImprovementEnabled().ShouldBe((true, productRegistrationId));
            Count<ApplicationConfiguration>().ShouldBe(1);

            productRegistrationId = Guid.NewGuid().ToString();
            EnableProductImprovement(enableProductImprovement: true, productRegistrationId: "   " + productRegistrationId + "   ");
            IsProductImprovementEnabled().ShouldBe((true, productRegistrationId));
            Count<ApplicationConfiguration>().ShouldBe(1);

            // Although the product registration id would go unused upon disabling product improvement,
            // we still persist the value independently. This way, the user does not need to redetermine
            // their id if they later wish to enable product improvement again.
            productRegistrationId = Guid.NewGuid().ToString();
            EnableProductImprovement(enableProductImprovement: false, productRegistrationId: productRegistrationId);
            IsProductImprovementEnabled().ShouldBe((false, productRegistrationId));
            Count<ApplicationConfiguration>().ShouldBe(1);

            EnableProductImprovement(enableProductImprovement: false, productRegistrationId: "");
            IsProductImprovementEnabled().ShouldBe((false, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);

            const string NullPersistedAsEmptyString = null;
            EnableProductImprovement(enableProductImprovement: false, productRegistrationId: NullPersistedAsEmptyString);
            IsProductImprovementEnabled().ShouldBe((false, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);
        }

        [Test]
        public void ShouldSetInitialProductImprovementBasedOnSettings()
        {
            CompleteFirstTimeSetUpStatus(true);
            IsProductImprovementEnabled().ShouldBe((true, null));
            Count<ApplicationConfiguration>().ShouldBe(1);

            CompleteFirstTimeSetUpStatus(false);
            IsProductImprovementEnabled().ShouldBe((false, null));
            Count<ApplicationConfiguration>().ShouldBe(1);
        }

        [Test]
        public void ShouldDisableProductImprovementAlwaysWhenDisabledInSettings()
        {
            EnableProductImprovement(enableProductImprovement: true, productRegistrationId: "", enableProductImprovementSettings: true);
            IsProductImprovementEnabled(true).ShouldBe((true, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);

            EnableProductImprovement(enableProductImprovement: true, productRegistrationId: "", enableProductImprovementSettings: true);
            IsProductImprovementEnabled(false).ShouldBe((false, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);

            EnableProductImprovement(enableProductImprovement: true, productRegistrationId: "", enableProductImprovementSettings: false);
            IsProductImprovementEnabled(true).ShouldBe((false, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);

            EnableProductImprovement(enableProductImprovement: false, productRegistrationId: "", enableProductImprovementSettings: false);
            IsProductImprovementEnabled(true).ShouldBe((false, ""));
            Count<ApplicationConfiguration>().ShouldBe(1);
        }

        private bool AllowUserRegistrations()
        {
            return Transaction(database =>
            {
                bool allowUserRegistration = false;

                Scoped<AdminAppIdentityDbContext>(identity =>
                {
                    var appSettings = new AppSettings { EnableProductImprovementSettings = true };
                    allowUserRegistration = new ApplicationConfigurationService(database, identity, Options.Create(appSettings)).AllowUserRegistration();
                });

                return allowUserRegistration;
            });
        }

        private (bool, string) IsProductImprovementEnabled(bool areProductImprovementSettingsEnabled = true)
        {
            return Transaction(database =>
            {
                bool enableProductImprovement = false;
                string productRegistrationId = "";

                Scoped<AdminAppIdentityDbContext>(identity =>
                {
                    var appSettings = new AppSettings { EnableProductImprovementSettings = areProductImprovementSettingsEnabled };
                    enableProductImprovement = new ApplicationConfigurationService(database, identity, Options.Create(appSettings))
                        .IsProductImprovementEnabled(out productRegistrationId);
                });

                return (enableProductImprovement, productRegistrationId);
            });
        }

        private void EnableProductImprovement(bool enableProductImprovement, string productRegistrationId,
            bool enableProductImprovementSettings = true)
        {
            var appSettings = new AppSettings { EnableProductImprovementSettings = enableProductImprovementSettings };

            Transaction(database =>
            {
                Scoped<AdminAppIdentityDbContext>(identity =>
                {
                    new ApplicationConfigurationService(database, identity, Options.Create(appSettings)).EnableProductImprovement(
                        enableProductImprovement, productRegistrationId);
                });
            });
        }

        private void CompleteFirstTimeSetUpStatus(bool enableProductImprovementSettings)
        {
            EnsureZeroApplicationConfiguration();
            var appSettings = new AppSettings { EnableProductImprovementSettings = enableProductImprovementSettings };
            Transaction(database =>
            {
                Scoped<AdminAppIdentityDbContext>(identity =>
                {
                    new ApplicationConfigurationService(database, identity, Options.Create(appSettings)).UpdateFirstTimeSetUpStatus(true);
                });
            });
        }

        private void EnsureZeroApplicationConfiguration()
        {
            DeleteAll<ApplicationConfiguration>();
        }

        private void EnsureOneApplicationConfiguration(bool allowRegistration, bool enableProductImprovement, string productRegistrationId)
        {
            Transaction(database =>
            {
                var config = database.EnsureSingle<ApplicationConfiguration>();
                config.AllowUserRegistration = allowRegistration;
                config.EnableProductImprovement = enableProductImprovement;
                config.ProductRegistrationId = productRegistrationId;
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
