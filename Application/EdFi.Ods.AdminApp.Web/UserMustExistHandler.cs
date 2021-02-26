// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Web
{
    public class UserMustExistRequirement : IAuthorizationRequirement { }

    public class UserMustExistHandler : AuthorizationHandler<UserMustExistRequirement>
    {
        private readonly AdminAppIdentityDbContext _identity;
        private readonly SignInManager<AdminAppUser> _signInManager;

        public UserMustExistHandler(AdminAppIdentityDbContext identity, SignInManager<AdminAppUser> signInManager)
        {
            _identity = identity;
            _signInManager = signInManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserMustExistRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                if (_identity.Users.SingleOrDefault(x => x.Id == userId) == null)
                {
                    await _signInManager.SignOutAsync();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
