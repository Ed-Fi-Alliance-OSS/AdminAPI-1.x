// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Instances
{
    public class ApiMode : Enumeration<ApiMode, string>
    {
        public static readonly ApiMode Sandbox = new ApiMode("sandbox", "Sandbox", supportsMultipleInstances: false);
        public static readonly ApiMode YearSpecific = new ApiMode("yearspecific", "Year Specific", supportsMultipleInstances: true);
        public static readonly ApiMode SharedInstance = new ApiMode("sharedinstance", "Shared Instance", supportsMultipleInstances: false);
        public static readonly ApiMode DistrictSpecific = new ApiMode("districtspecific", "District Specific", supportsMultipleInstances: true);

        private ApiMode(string value, string displayName, bool supportsMultipleInstances)
            : base(value, displayName)
        {
            SupportsMultipleInstances = supportsMultipleInstances;
        }

        public bool SupportsMultipleInstances { get; }

        public bool SupportsSingleInstance => !SupportsMultipleInstances;
    }
}