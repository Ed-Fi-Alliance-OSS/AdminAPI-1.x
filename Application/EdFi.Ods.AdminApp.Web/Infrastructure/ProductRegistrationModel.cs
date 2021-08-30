// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class ProductRegistrationModel
    {
        /*
         * IMPORTANT: Because this model corresponds with the external product registration
         *            endpoint, changes here could easily break data collection. This model
         *            may also be used by other Ed-Fi applications, so apply model changes
         *            with great care.
         */

        public string ProductRegistrationId { get; set; }

        public OdsApiConnection[] OdsApiConnections { get; set; }

        public string ProductType => "Admin App";

        public string ProductVersion { get; set; }

        [JsonProperty(PropertyName= "OSVersion")]
        public string OsVersion { get; set; }

        public string DatabaseVersion { get; set; }

        public string HostName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime UtcTimeStamp { get; set; }

        public string Serialize() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public class OdsApiConnection
        {
            public string OdsApiVersion { get; set; }

            public string OdsApiMode { get; set; }

            public int? InstanceCount { get; set; }
        }
    }
}
