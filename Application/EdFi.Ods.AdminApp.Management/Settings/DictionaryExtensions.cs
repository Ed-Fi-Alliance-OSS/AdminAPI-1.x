// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.Settings
{
    public static class DictionaryExtensions
    {
        public static string GetOptionalStringValue(this IDictionary<string, string> settings, string key, string defaultValue = null)
        {
            return !settings.ContainsKey(key) ? defaultValue : settings[key];
        }

        public static string GetRequiredStringValue(this IDictionary<string, string> settings, string key)
        {
            if (!settings.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Key '{key}' not found");
            }

            return settings[key];
        }

        public static int GetRequiredIntValue(this IDictionary<string, string> settings, string key)
        {
            if (!settings.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Key '{key}' not found");
            }

            int result;
            if (!int.TryParse(settings[key], out result))
            {
                throw new FormatException($"Value for key '{key}' not formatted as an integer");
            }

            return result;
        }

        public static int GetOptionalIntValueOrDefault(this IDictionary<string, string> settings, string key, int defaultValue)
        {
            string valueString;
            if (!settings.TryGetValue(key, out valueString)) return defaultValue;

            int valueInt;
            if (int.TryParse(valueString, out valueInt))
            {
                return valueInt;
            }
            throw new FormatException($"Value for key '{key}' not formatted as an integer");
        }
    }
}
