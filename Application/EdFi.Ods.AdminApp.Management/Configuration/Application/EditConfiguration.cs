// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Management.Configuration.Application
{
    public class EditConfiguration
    {
        public class Command
        {
            public bool AllowUserRegistration { get; set; }
        }

        public class QueryHandler
        {
            private readonly AdminAppDbContext _database;

            public QueryHandler(AdminAppDbContext database) => _database = database;

            public Command Execute()
            {
                return new Command
                {
                    AllowUserRegistration =
                        _database
                            .ApplicationConfigurations
                            .SingleOrDefault()?
                            .AllowUserRegistration ?? false
                };
            }
        }

        public class CommandHandler
        {
            private readonly AdminAppDbContext _database;

            public CommandHandler(AdminAppDbContext database) => _database = database;

            public void Execute(Command request)
            {
                var config = _database.EnsureSingle<ApplicationConfiguration>();

                config.AllowUserRegistration = request.AllowUserRegistration;

                _database.SaveChanges();
            }
        }
    }
}