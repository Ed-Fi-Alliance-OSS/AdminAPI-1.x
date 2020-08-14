// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;

namespace EdFi.Ods.AdminApp.Management
{
    public class OdsFirstTimeSetupConfiguration
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public IEnumerable<OdsComponent> Components { get; set; } 
        public OdsSqlConfiguration SqlConfiguration { get; set; }
        public CloudOdsClaimSet ClaimSet { get; set; }
    }
}