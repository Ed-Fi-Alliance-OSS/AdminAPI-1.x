// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;

namespace EdFi.Ods.AdminApp.Web.Display.TabEnumeration
{
    public sealed class OdsInstanceSettingsTabEnumeration : Enumeration<OdsInstanceSettingsTabEnumeration>, ITabEnumeration
    {
        public string ControllerName => RouteHelpers.GetControllerName<OdsInstanceSettingsController>();
        public string ActionName { get; }

        private OdsInstanceSettingsTabEnumeration(int value, string displayName, string actionName) : base(value, displayName)
        {
            ActionName = actionName;
        }

        public static readonly OdsInstanceSettingsTabEnumeration Applications = new OdsInstanceSettingsTabEnumeration(1, "Applications", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.Applications()));
        public static readonly OdsInstanceSettingsTabEnumeration Descriptors = new OdsInstanceSettingsTabEnumeration(2, "Descriptors", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.Descriptors()));
        public static readonly OdsInstanceSettingsTabEnumeration Logging = new OdsInstanceSettingsTabEnumeration(3, "Logging", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.Logging()));
        public static readonly OdsInstanceSettingsTabEnumeration EducationOrganizations = new OdsInstanceSettingsTabEnumeration(4, "Education Organizations", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.EducationOrganizations()));
        public static readonly OdsInstanceSettingsTabEnumeration Setup = new OdsInstanceSettingsTabEnumeration(5, "Setup", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.Setup()));
        public static readonly OdsInstanceSettingsTabEnumeration BulkLoad = new OdsInstanceSettingsTabEnumeration(6, "Bulk Load", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.BulkLoad()));
        public static readonly OdsInstanceSettingsTabEnumeration LearningStandards = new OdsInstanceSettingsTabEnumeration(7, "Learning Standards", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.LearningStandards()));
        public static readonly OdsInstanceSettingsTabEnumeration Reports = new OdsInstanceSettingsTabEnumeration(8, "Reports", RouteHelpers.GetActionName<OdsInstanceSettingsController>(x => x.SelectDistrict(0)));
    }
}