// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Display.DisplayService;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using NUnit.Framework;

namespace EdFi.Ods.AdminApp.Management.Tests.Display.HomeScreenDisplayService
{
    [TestFixture]
    public class WhenDisplayingOnPremisesHomeScreen
    {
        private static HomeScreenDisplayAssertions HomeScreenTabAssertions(params Permission[] permissions)
        {
            return new HomeScreenDisplayAssertions(new OnPremHomeScreenDisplayService(new AdminAppUserContext
            {
                Permissions = permissions
            }));
        }

        [Test]
        public void Should_display_settings_tab()
        {
            HomeScreenTabAssertions().TabShouldBeEnabled(HomeScreenEnumeration.Settings);
        }

        [Test]
        public void Should_display_production_global_tab()
        {
            HomeScreenTabAssertions(Permission.AccessGlobalSettings).TabShouldBeEnabled(HomeScreenEnumeration.Global);
        }

        [Test]
        public void Should_not_display_production_global_tab()
        {
            HomeScreenTabAssertions().TabShouldNotBeEnabled(HomeScreenEnumeration.Global);
        }

        [Test]
        public void Should_display_updates_tab()
        {
            HomeScreenTabAssertions().TabShouldNotBeEnabled(HomeScreenEnumeration.Updates);
        }
    }
}
