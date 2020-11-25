// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Management.Identity
{
    public class RegisterCommand
    {
        public async Task<(AdminAppUser user, IdentityResult result)> Execute(IRegisterModel userModel, UserManager<AdminAppUser> userManager)
        {
            var user = new AdminAppUser { UserName = userModel.Email, Email = userModel.Email};
            var result = await userManager.CreateAsync(user, userModel.Password);
            return (user: user, result: result);
        }
    }

    public interface IRegisterModel
    {
        string Email { get; }
        string Password { get; }
    }
}

