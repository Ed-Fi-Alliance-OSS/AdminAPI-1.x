// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Management
{
    public class AdminAppUserContext
    {
        public AdminAppUser User { get; set; }
        public IReadOnlyList<Permission> Permissions { get; set; }
        public bool Has(Permission permission)
            => Permissions.Contains(permission);

        public AdminAppUserContext()
        {
            Permissions = new Permission[] { };
        }
    }
}