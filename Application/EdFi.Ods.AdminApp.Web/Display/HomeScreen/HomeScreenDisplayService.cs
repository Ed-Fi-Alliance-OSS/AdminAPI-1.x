// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
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

        public List<HomeScreenEnumeration> GetHomeScreenDisplays()
        {
            var homeScreenDisplayList = new List<HomeScreenEnumeration>();

            if (_userContext.Has(Permission.AccessGlobalSettings))
            {
                homeScreenDisplayList.Add(HomeScreenEnumeration.Global);
            }

            if (CloudOdsAdminAppSettings.Instance.Mode.SupportsMultipleInstances)
            {
                homeScreenDisplayList.Add(HomeScreenEnumeration.OdsInstances);
            }
            else
            {
                homeScreenDisplayList.Add(HomeScreenEnumeration.Settings);
            }

            return homeScreenDisplayList;
        }
    }
}
