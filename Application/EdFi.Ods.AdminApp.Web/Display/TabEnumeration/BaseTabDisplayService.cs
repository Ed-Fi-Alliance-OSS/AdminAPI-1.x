// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Web.Display.TabEnumeration
{
    public abstract class BaseTabDisplayService 
    {
        public virtual List<TabDisplay<GlobalSettingsTabEnumeration>> GetGlobalSettingsTabDisplay(GlobalSettingsTabEnumeration selectedTab)
        {
            var tabDisplayList = new List<TabDisplay<GlobalSettingsTabEnumeration>>
            {
                new TabDisplay<GlobalSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = GlobalSettingsTabEnumeration.Vendors
                },

                new TabDisplay<GlobalSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = GlobalSettingsTabEnumeration.AdvancedSettings
                },              

                new TabDisplay<GlobalSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = GlobalSettingsTabEnumeration.ClaimSets
                },

                new TabDisplay<GlobalSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = GlobalSettingsTabEnumeration.Users
                }
            };

            tabDisplayList.Find(a => a.Tab == selectedTab).IsSelected = true;

            return tabDisplayList;
        }

        public virtual List<TabDisplay<OdsInstanceSettingsTabEnumeration>> GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration selectedTab)
        {
            var tabDisplayList = new List<TabDisplay<OdsInstanceSettingsTabEnumeration>>
            {
                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.Applications
                },

                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.Descriptors
                },

                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.Logging
                },

                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.EducationOrganizations
                },

                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.Setup
                },

                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.BulkLoad
                },

                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.LearningStandards
                },

                new TabDisplay<OdsInstanceSettingsTabEnumeration>
                {
                    IsEnabled = true,
                    IsVisible = true,
                    Tab = OdsInstanceSettingsTabEnumeration.Reports
                }
            };

            tabDisplayList.Find(a => a.Tab == selectedTab).IsSelected = true;

            return tabDisplayList;
        }
    }
}
