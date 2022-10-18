// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstanceSettings
{
    public abstract class BaseOdsInstanceSettingsModel
    {
        public InstanceContext OdsInstance { get; set; }
        public List<TabDisplay<OdsInstanceSettingsTabEnumeration>> OdsInstanceSettingsTabEnumerations { get; set; }
    }
}
