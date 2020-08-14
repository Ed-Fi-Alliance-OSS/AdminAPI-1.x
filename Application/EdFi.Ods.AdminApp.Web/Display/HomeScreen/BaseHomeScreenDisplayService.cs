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
    public abstract class BaseHomeScreenDisplayService
    {
        private readonly AdminAppUserContext _userContext;

        protected BaseHomeScreenDisplayService(AdminAppUserContext userContext)
        {
            _userContext = userContext;
        }

        public virtual List<HomeScreenDisplay<HomeScreenEnumeration>> GetHomeScreenDisplays()
        {
            var homeScreenDisplayList = new List<HomeScreenDisplay<HomeScreenEnumeration>>
            {
                new HomeScreenDisplay<HomeScreenEnumeration>
                {
                    IsEnabled = true,
                    HomeScreen = HomeScreenEnumeration.OdsInstances
                },
                new HomeScreenDisplay<HomeScreenEnumeration>
                {
                    IsEnabled = true,
                    HomeScreen = HomeScreenEnumeration.Settings
                },
                new HomeScreenDisplay<HomeScreenEnumeration>
                {
                    IsEnabled = true,
                    HomeScreen = HomeScreenEnumeration.Global
                },
                new HomeScreenDisplay<HomeScreenEnumeration>
                {
                    IsEnabled = true,
                    HomeScreen = HomeScreenEnumeration.Updates
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