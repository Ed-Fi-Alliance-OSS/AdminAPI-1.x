// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class JsonObjectManipulation
    {
        public static JObject RemoveProperties(JObject jsonObject, List<string> propertyNames)
        {
            var token = JToken.FromObject(jsonObject);
            RemoveProperties(token, propertyNames);
            return token.ToObject<JObject>();
        }

        private static void RemoveProperties(JToken token, List<string> propertyNames)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                {
                    foreach (var prop in token.Children<JProperty>().ToList())
                    {
                        var removed = false;
                        foreach (var name in propertyNames)
                        {
                            if (!name.Equals(prop.Name)) continue;
                            prop.Remove();
                            removed = true;
                            break;
                        }
                        if (!removed)
                        {
                            RemoveProperties(prop.Value, propertyNames);
                        }
                    }

                    break;
                }
                case JTokenType.Array:
                {
                    foreach (var child in token.Children())
                    {
                        RemoveProperties(child, propertyNames);
                    }

                    break;
                }
            }
        }
    }
}