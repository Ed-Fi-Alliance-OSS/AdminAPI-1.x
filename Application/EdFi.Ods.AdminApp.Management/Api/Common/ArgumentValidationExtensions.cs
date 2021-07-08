// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.Api.Common
{
    public static class ArgumentValidationExtensions
    {
        public static T IsRequired<T>(this T o, string paramName, string resource)
        {
            if (EqualityComparer<T>.Default.Equals(o, default(T)))
                throw new ArgumentValidationException($"{paramName} is a required property for {resource} and cannot be null");

            return o;
        }

        public static string MaxLength(this string o, int maxLength, string paramName, string resource)
        {
            if (o != null && o.Length > maxLength)
                throw new ArgumentValidationException($"Invalid value for {paramName} on {resource}, length must be less than {maxLength}");

            return o;
        }
    }
}