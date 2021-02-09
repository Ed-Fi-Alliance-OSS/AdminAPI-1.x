// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Home
{
    public class IndexModel
    {
        public bool SetupJustCompleted { get; set; }
        public List<HomeScreenEnumeration> HomeScreenDisplays { get; set; }
    }
}
