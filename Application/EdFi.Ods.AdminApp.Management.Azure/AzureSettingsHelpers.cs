// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Settings;
using Microsoft.Azure.Management.WebSites.Models;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public static class AzureSettingsHelpers
    {
        public static StringDictionary BuildStringDictionary(this IDictionary<string, string> dictionary)
        {
            return new StringDictionary
            {
                Properties = dictionary
            };
        }

        public static string GetRequiredStringSetting(this StringDictionary settings, string key)
        {
            return settings.Properties.GetRequiredStringValue(key);
        }

        public static int GetRequiredIntSetting(this StringDictionary settings, string key)
        {
            return settings.Properties.GetRequiredIntValue(key);
        }
    }
}
