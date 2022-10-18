// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management
{
    public class CloudOdsWebsite
    {
        public CloudOdsWebsite()
        {
            Tags = new Dictionary<string, string>();
        }

        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string Url { get; set; }
        public CloudOdsEnvironment Environment { get; set; }
        public CloudOdsRole Role { get; set; }
        public Dictionary<string, string> Tags { get; set; } 
    }
}
