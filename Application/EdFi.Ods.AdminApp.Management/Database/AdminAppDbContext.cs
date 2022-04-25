// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public class AdminAppDbContext : DbContext
    {
        private readonly IOptions<AppSettings> _appSettings;

        public AdminAppDbContext(DbContextOptions<AdminAppDbContext> options, IOptions<AppSettings> appSettings)
            : base(options)
        {
            _appSettings = appSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("adminapp");
            modelBuilder.ApplyDatabaseServerSpecificConventions(_appSettings.Value.DatabaseEngine);
            modelBuilder.Entity<DatabaseVersion>().HasNoKey();
        }

        public DbSet<ApplicationConfiguration> ApplicationConfigurations { get; set; }
        public DbSet<SecretConfiguration> SecretConfigurations { get; set; }
        public DbSet<OdsInstanceRegistration> OdsInstanceRegistrations { get; set; }
        public DbSet<DatabaseVersion> DatabaseVersionView { get; set; }

        public TEntity EnsureSingle<TEntity>() where TEntity : Entity, new()
        {
            var single = Set<TEntity>().SingleOrDefault();

            if (single == null)
            {
                single = new TEntity();
                Set<TEntity>().Add(single);
            }

            return single;
        }
    }
}
