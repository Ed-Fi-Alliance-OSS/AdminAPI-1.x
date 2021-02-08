// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.Display.HomeScreen
{
    public class HomeScreenDisplayAssertions
    {
        private readonly HomeScreenDisplayService _tabDisplayService;

        public HomeScreenDisplayAssertions(HomeScreenDisplayService tabDisplayService)
        {
            _tabDisplayService = tabDisplayService;
        }

        public void TabShouldBeEnabled(HomeScreenEnumeration tabToVerify)
        {
            GetHomeScreenTabs().ShouldContain(tabToVerify);
        }

        public void TabShouldNotBeEnabled(HomeScreenEnumeration tabToVerify)
        {
            GetHomeScreenTabs().ShouldNotContain(tabToVerify);
        }

        private List<HomeScreenEnumeration> GetHomeScreenTabs() => _tabDisplayService.GetHomeScreenDisplays();
    }
}
