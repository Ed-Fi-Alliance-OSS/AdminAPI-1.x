// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Global
{
    public abstract class BaseGlobalSettingsModel
    {
        public List<TabDisplay<GlobalSettingsTabEnumeration>> GlobalSettingsTabEnumerations{ get; set; }
    }
}
