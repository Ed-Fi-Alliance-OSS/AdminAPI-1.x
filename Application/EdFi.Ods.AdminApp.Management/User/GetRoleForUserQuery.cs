// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
#if NET48
using Microsoft.AspNet.Identity.EntityFramework;
#endif

namespace EdFi.Ods.AdminApp.Management.User
{
    public class GetRoleForUserQuery
    {
        public Role Execute(string userId)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                var userRoleId = database.Set<IdentityUserRole>().SingleOrDefault(x => x.UserId == userId)?.RoleId;
                return userRoleId != null ? Role.GetAll().Single(x => x.Value.Equals(int.Parse(userRoleId))) : null;
            }
                
        }
    }
}
