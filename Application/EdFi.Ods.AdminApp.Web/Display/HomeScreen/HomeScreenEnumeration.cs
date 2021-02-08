// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Web.Display.HomeScreen
{
    public sealed class HomeScreenEnumeration : Enumeration<HomeScreenEnumeration> , IHomeScreen
    {
        private HomeScreenEnumeration(int value, string displayName) : base(value, displayName)
        {
        }

        public static readonly HomeScreenEnumeration Settings = new HomeScreenEnumeration(1, "Settings");
        public static readonly HomeScreenEnumeration OdsInstances = new HomeScreenEnumeration(2, "ODS Instances");
        public static readonly HomeScreenEnumeration Global = new HomeScreenEnumeration(4, "Global");
    }
}
