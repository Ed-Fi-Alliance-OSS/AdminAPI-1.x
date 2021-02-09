// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using NUnit.Framework;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.Display.HomeScreen
{
    [TestFixture]
    public class WhenDisplayingHomeScreen
    {
        private static HomeScreenDisplayAssertions HomeScreenTabAssertions(params Permission[] permissions)
        {
            return new HomeScreenDisplayAssertions(new HomeScreenDisplayService(new AdminAppUserContext
            {
                Permissions = permissions
            }));
        }

        [Test]
        public void Should_display_the_settings_button()
        {
            HomeScreenTabAssertions().TabShouldBeEnabled(HomeScreenEnumeration.Settings);
        }

        [Test]
        public void Should_display_global_button()
        {
            HomeScreenTabAssertions(Permission.AccessGlobalSettings).TabShouldBeEnabled(HomeScreenEnumeration.Global);
        }

        [Test]
        public void Should_not_display_global_button()
        {
            HomeScreenTabAssertions().TabShouldNotBeEnabled(HomeScreenEnumeration.Global);
        }
    }
}
