// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web.Display.HomeScreen
{
    public class HomeScreenDisplayService : IHomeScreenDisplayService
    {
        private readonly AdminAppUserContext _userContext;

        public HomeScreenDisplayService(AdminAppUserContext userContext)
        {
            _userContext = userContext;
        }

        public virtual List<HomeScreenDisplay> GetHomeScreenDisplays()
        {
            var homeScreenDisplayList = new List<HomeScreenDisplay>
            {
                new HomeScreenDisplay
                {
                    IsEnabled = true,
                    HomeScreen = HomeScreenEnumeration.OdsInstances
                },
                new HomeScreenDisplay
                {
                    IsEnabled = true,
                    HomeScreen = HomeScreenEnumeration.Settings
                },
                new HomeScreenDisplay
                {
                    IsEnabled = true,
                    HomeScreen = HomeScreenEnumeration.Global
                }
            };

            if (CloudOdsAdminAppSettings.Instance.Mode.SupportsMultipleInstances)
            {
                homeScreenDisplayList.Single(x =>
                    x.HomeScreen == HomeScreenEnumeration.Settings).IsEnabled = false;
            }

            if (!_userContext.Has(Permission.AccessGlobalSettings))
                homeScreenDisplayList.RemoveAll(x => x.HomeScreen == HomeScreenEnumeration.Global);

            return homeScreenDisplayList;
        }
    }
}
