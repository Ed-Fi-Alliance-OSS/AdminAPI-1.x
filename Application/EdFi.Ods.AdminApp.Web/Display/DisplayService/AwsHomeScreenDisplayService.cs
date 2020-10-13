// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;

namespace EdFi.Ods.AdminApp.Web.Display.DisplayService
{
    [System.Obsolete("Review usages.")]
    public class AwsHomeScreenDisplayService : BaseHomeScreenDisplayService, IHomeScreenDisplayService
    {
        public AwsHomeScreenDisplayService(AdminAppUserContext userContext) : base(userContext)
        {
            
        }

        public override List<HomeScreenDisplay<HomeScreenEnumeration>> GetHomeScreenDisplays()
        {
            var homeScreenDisplayList = base.GetHomeScreenDisplays();

            var screenToRemove = homeScreenDisplayList.Single(x =>
                x.HomeScreen == HomeScreenEnumeration.Updates);

            homeScreenDisplayList.Remove(screenToRemove);

            return homeScreenDisplayList;
        }
    }
}
