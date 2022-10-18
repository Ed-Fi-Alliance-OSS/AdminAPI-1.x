// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.Admin.Api.Infrastructure
{
    public static class AdminModelExtensions
    {
        public static string? ContactName(this Vendor vendor)
        {
            return vendor?.Users?.FirstOrDefault()?.FullName;
        }

        public static string? ContactEmail(this Vendor vendor)
        {
            return vendor?.Users?.FirstOrDefault()?.Email;
        }

        public static string? ProfileName(this Application application)
        {
            return application?.Profiles?.FirstOrDefault()?.ProfileName;
        }
    }
}
