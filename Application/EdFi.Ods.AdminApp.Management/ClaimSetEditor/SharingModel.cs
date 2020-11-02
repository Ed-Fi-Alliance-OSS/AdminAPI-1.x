// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class SharingModel
    {
        public string Title { get; set; }
        public SharingTemplate Template { get; set; }

        public static string SerializeFromSharingModel(SharingModel model)
        {
            return JsonConvert.SerializeObject(model, UsingIndentedCamelCase());
        }

        public static SharingModel DeserializeToSharingModel(Stream jsonStream)
        {
            using (var streamReader = new StreamReader(jsonStream))
                using (JsonReader jsonReader = new JsonTextReader(streamReader))
                {
                    return JsonSerializer
                        .Create(UsingIndentedCamelCase())
                        .Deserialize<SharingModel>(jsonReader);
                }
        }

        private static JsonSerializerSettings UsingIndentedCamelCase()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
        }
    }

    public class SharingTemplate
    {
        public SharingClaimSet[] ClaimSets { get; set; }
    }

    public class SharingClaimSet
    {
        public string Name { get; set; }
        public List<JObject> ResourceClaims { get; set; }
    }
}