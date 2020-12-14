// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class AdminModelExtensions
    {
        public static string ContactName(this Vendor vendor)
        {
            return vendor?.Users?.FirstOrDefault()?.FullName;
        }

        public static string ContactEmail(this Vendor vendor)
        {
            return vendor?.Users?.FirstOrDefault()?.Email;
        }

        public static string ProfileName(this Application application)
        {
            return application?.Profiles?.FirstOrDefault()?.ProfileName;
        }

        //public static bool IsProductionApplication(this Application application)
        //{
        //    var prefix = CloudOdsApplicationName.GetPersistedNamePrefix();
        //    return application.ApplicationName.StartsWith(prefix);
        //}

        //public static string DisplayName(this Application application)
        //{
        //    return CloudOdsApplicationName.GetDisplayName(application.ApplicationName);
        //}
    }
}
