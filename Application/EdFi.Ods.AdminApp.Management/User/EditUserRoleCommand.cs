// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
#if NET48
using Microsoft.AspNet.Identity.EntityFramework;
#else
using EdFi.Ods.AdminApp.Management.Database.Models;
#endif

namespace EdFi.Ods.AdminApp.Management.User
{
    public class EditUserRoleCommand
    {
        public void Execute(IEditUserRoleModel model)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                var newUserRole = new IdentityUserRole
                {
                    UserId = model.UserId,
                    RoleId = model.RoleId
                };

                var currentUserRole = database.Set<IdentityUserRole>().SingleOrDefault(x => x.UserId == model.UserId);

                if (currentUserRole != null)
                {
                    database.Set<IdentityUserRole>().Remove(currentUserRole);
                }

                database.Set<IdentityUserRole>().Add(newUserRole);
                database.SaveChanges();
            }
        }
    }

    public interface IEditUserRoleModel
    {
        string UserId { get; }
        string RoleId { get; }
    }
}
