// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;

namespace EdFi.Ods.AdminApp.Web.Display.TabEnumeration
{
    public sealed class GlobalSettingsTabEnumeration : Enumeration<GlobalSettingsTabEnumeration>, ITabEnumeration
    {
        public string ControllerName => RouteHelpers.GetControllerName<GlobalSettingsController>();
        public string ActionName { get; }

        private GlobalSettingsTabEnumeration(int value, string displayName, string actionName) : base(value, displayName)
        {
            ActionName = actionName;
        }

        public static readonly GlobalSettingsTabEnumeration Vendors = new GlobalSettingsTabEnumeration(1, "Vendors", RouteHelpers.GetActionName<GlobalSettingsController>(x => x.Vendors()));
        public static readonly GlobalSettingsTabEnumeration AdvancedSettings = new GlobalSettingsTabEnumeration(2, "Advanced Settings", RouteHelpers.GetActionName<GlobalSettingsController>(x => x.AdvancedSettings()));        
        public static readonly GlobalSettingsTabEnumeration ClaimSets = new GlobalSettingsTabEnumeration(4, "Claim Sets", RouteHelpers.GetActionName<GlobalSettingsController>(x => x.ClaimSets()));
        public static readonly GlobalSettingsTabEnumeration Users = new GlobalSettingsTabEnumeration(5, "Users", RouteHelpers.GetActionName<GlobalSettingsController>(x => x.Users()));
    }
}
