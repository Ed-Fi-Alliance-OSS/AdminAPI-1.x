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
    public class DeleteUserCommand
    {
        public void Execute(IDeleteUserModel userModel)
        {
            using (var dbContext = AdminAppIdentityDbContext.Create())
            {
                RemoveExistingUserRoles(userModel.UserId, dbContext);

                RemoveExistingUserOdsInstanceRegistrations(userModel.UserId, dbContext);

                dbContext.Users.Remove(dbContext.Users.Single(x => x.Id == userModel.UserId));

                dbContext.SaveChanges();
            }
        }

        private static void RemoveExistingUserOdsInstanceRegistrations(string userId, AdminAppIdentityDbContext dbContext)
        {
            var existingUserOdsInstanceRegistrations =
                dbContext.UserOdsInstanceRegistrations.Where(x => x.UserId == userId);

            if (existingUserOdsInstanceRegistrations.Any())
            {
                dbContext.UserOdsInstanceRegistrations.RemoveRange(existingUserOdsInstanceRegistrations);
            }

            dbContext.SaveChanges();
        }

        private static void RemoveExistingUserRoles(string userId, AdminAppIdentityDbContext dbContext)
        {
            var existingUserRoles =
                dbContext.Set<IdentityUserRole>().Where(x => x.UserId == userId);

            if (existingUserRoles.Any())
            {
                dbContext.Set<IdentityUserRole>().RemoveRange(existingUserRoles);
            }

            dbContext.SaveChanges();
        }
    }

    public interface IDeleteUserModel
    {
        string UserId { get; }
    }
}
