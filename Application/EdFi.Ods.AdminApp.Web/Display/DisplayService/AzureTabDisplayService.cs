// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;

namespace EdFi.Ods.AdminApp.Web.Display.DisplayService
{
    public class AzureTabDisplayService : BaseTabDisplayService, ITabDisplayService
    {
        public override List<TabDisplay<GlobalSettingsTabEnumeration>> GetGlobalSettingsTabDisplay(GlobalSettingsTabEnumeration selectedTab)
        {
            var globalSettingsTabs = base.GetGlobalSettingsTabDisplay(selectedTab);
            return globalSettingsTabs;
        }

        public override List<TabDisplay<OdsInstanceSettingsTabEnumeration>> GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration selectedTab)
        {
            var instanceSettings = base.GetOdsInstanceSettingsTabDisplay(selectedTab);

            return instanceSettings;
        }
    }
}
