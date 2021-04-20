// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public class AdminAppDataProtectionKeysDbContext : DbContext, IDataProtectionKeyContext
    {
        private readonly IOptions<AppSettings> _appSettings;

        public AdminAppDataProtectionKeysDbContext(DbContextOptions<AdminAppDataProtectionKeysDbContext> options, IOptions<AppSettings> appSettings)
            : base(options)
        {
            _appSettings = appSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("adminapp");
            modelBuilder.ApplyDatabaseServerSpecificConventions(_appSettings.Value.DatabaseEngine);
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
