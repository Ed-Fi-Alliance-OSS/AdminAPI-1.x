// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Management.User
{
    public class GetAdminAppUserByIdQuery
    {
        private readonly AdminAppIdentityDbContext _identity;

        public GetAdminAppUserByIdQuery(AdminAppIdentityDbContext identity)
        {
            _identity = identity;
        }

        public AdminAppUser Execute(string userId)
        {
            return _identity.Users.Single(x => x.Id == userId);
        }
    }
}
