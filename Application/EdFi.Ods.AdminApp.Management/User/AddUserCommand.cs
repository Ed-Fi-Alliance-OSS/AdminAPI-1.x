// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Management.User
{
    public class AddUserCommand
    {
        public async Task<(string userId, IdentityResult result)> Execute(IAddUserModel userModel, UserManager<AdminAppUser> userManager)
        {
            var user = new AdminAppUser { UserName = userModel.Email, Email = userModel.Email, RequirePasswordChange = true };
            var result = await userManager.CreateAsync(user, userModel.Password);
            return (userId: user.Id, result: result);
        }
    }

    public interface IAddUserModel
    {
        string Email { get; }
        string Password { get; }
    }
}
