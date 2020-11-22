// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
#if NET48
using EdFi.Ods.Common.Utils.Extensions;
#else
using EdFi.Common.Utils.Extensions;
#endif

namespace EdFi.Ods.AdminApp.Web.Display.DisplayService
{
    public class OnPremTabDisplayService : BaseTabDisplayService, ITabDisplayService
    {
        public override List<TabDisplay<GlobalSettingsTabEnumeration>>
            GetGlobalSettingsTabDisplay(GlobalSettingsTabEnumeration selectedTab)
        {
            var globalSettingsTabs = base.GetGlobalSettingsTabDisplay(selectedTab);

            var tabsToDisable = globalSettingsTabs.Where(x =>
                x.Tab == GlobalSettingsTabEnumeration.AdvancedSettings);

            tabsToDisable.ForEach(tab =>
            {
                tab.IsEnabled = false;
                tab.IsVisible = false;
            });

            return globalSettingsTabs;
        }

        public override List<TabDisplay<OdsInstanceSettingsTabEnumeration>> GetOdsInstanceSettingsTabDisplay(
            OdsInstanceSettingsTabEnumeration selectedTab)
        {
            var instanceSettings = base.GetOdsInstanceSettingsTabDisplay(selectedTab);

            var tabsToDisable = instanceSettings.Where(x =>
                x.Tab == OdsInstanceSettingsTabEnumeration.Logging).ToList();

            if (!CloudOdsAdminAppSettings.Instance.DbSetupEnabled)
            {
                 tabsToDisable.AddRange(instanceSettings.Where(x =>
                    x.Tab == OdsInstanceSettingsTabEnumeration.Setup).ToList());
            }

            tabsToDisable.ForEach(tab =>
            {
                tab.IsEnabled = false;
                tab.IsVisible = false;
            });

            return instanceSettings;
        }
    }
}
