// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Management.User
{
    public class EditUserCommand
    {
        public async Task<IdentityResult> Execute(IEditUserModel userModel, UserManager<AdminAppUser> userManager)
        {
            var existingUser = await userManager.FindByIdAsync(userModel.UserId);

            existingUser.Email = userModel.Email;
            existingUser.UserName = userModel.Email;

            var result = await userManager.UpdateAsync(existingUser);

            return result;
        }
    }

    public interface IEditUserModel
    {
        string Email { get; }
        string UserId { get; }
    }
}
