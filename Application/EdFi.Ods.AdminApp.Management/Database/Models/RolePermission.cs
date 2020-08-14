// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Management.Database.Models
{
    public class RolePermission
    {
        private static readonly Permission[] SuperAdminPermissions =
        {
            Permission.AccessGlobalSettings
        };

        private static readonly Permission[] AdminPermissions =
        {

        };

        public static Permission[] GetPermissions(string roleId)
        {
            if (int.TryParse(roleId, out int value))
            {
                if (Role.TryParse(value, out Role role))
                {
                    if (role == Role.SuperAdmin)
                        return SuperAdminPermissions;
                    if (role == Role.Admin)
                        return AdminPermissions;
                }
            }

            return Array.Empty<Permission>();
        }
    }
}