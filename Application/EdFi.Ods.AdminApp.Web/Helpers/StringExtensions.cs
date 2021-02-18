// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string text)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(text);
        }

        public static string RemoveWhitespace(this string text)
        {
            return Regex.Replace(text, @"\s+", "");
        }

        public static Version ToVersion(this string versionText)
        {
            Version version;
            return Version.TryParse(versionText, out version) ? version : null;
        }

        public static string SafeDecodeBase64String(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(input));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetDescriptorCategoryName(this string descriptorPath)
        {
            var descriptorPathParts = DescriptorPathParts(descriptorPath);

            string descriptorName = null;

            if (descriptorPathParts.Length == 2)
            {
                var routePrefix = descriptorPathParts[0];
                var name = descriptorPathParts[1];

                descriptorName = name.Remove(name.Length - 1, 1).CapitalizeFirstLetter();

                if (routePrefix != "ed-fi")
                    descriptorName = $"{descriptorName} [{routePrefix.CapitalizeFirstLetter()}]";
            }

            return descriptorName;
        }

        public static string GetDescriptorCategoryIdentifier(this string descriptorPath)
        {
            var descriptorPathParts = DescriptorPathParts(descriptorPath);

            string descriptorId = null;

            if (descriptorPathParts.Length == 2)
            {
                var routePrefix = descriptorPathParts[0];
                var name = descriptorPathParts[1];

                descriptorId = $"{name}-{routePrefix}";
            }

            return descriptorId;
        }

        private static string CapitalizeFirstLetter(this string text)
        {
            if (text.Length > 0)
                return char.ToUpper(text[0]) + text.Substring(1);
            return text;
        }

        private static string[] DescriptorPathParts(string descriptorPath)
        {
            var descriptorPathParts = descriptorPath.TrimStart('/').Split('/', 3);
            return descriptorPathParts;
        }
    }
}