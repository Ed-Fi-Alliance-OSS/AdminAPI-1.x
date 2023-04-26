// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.ErrorHandling;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public class GetApplicationByIdQuery
    {
        private readonly IUsersContext _context;

        public GetApplicationByIdQuery(IUsersContext context)
        {
            _context = context;
        }

        public Application Execute(int applicationId)
        {
            var application = _context.Applications.SingleOrDefault(app => app.ApplicationId == applicationId);
            if (application == null)
            {
                throw new NotFoundException<int>("application", applicationId);
            }

            return application;
        }
    }
}
