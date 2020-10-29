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
        private readonly AdminAppIdentityDbContext _identity;

        public EditUserRoleCommand(AdminAppIdentityDbContext identity)
        {
            _identity = identity;
        }

        public void Execute(IEditUserRoleModel model)
        {
            var newUserRole = new IdentityUserRole
            {
                UserId = model.UserId,
                RoleId = model.RoleId
            };

            var currentUserRole =
                _identity.Set<IdentityUserRole>().SingleOrDefault(x => x.UserId == model.UserId);

            if (currentUserRole != null)
            {
                _identity.Set<IdentityUserRole>().Remove(currentUserRole);
            }

            _identity.Set<IdentityUserRole>().Add(newUserRole);
            _identity.SaveChanges();
        }
    }

    public interface IEditUserRoleModel
    {
        string UserId { get; }
        string RoleId { get; }
    }
}
