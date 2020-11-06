// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
#if NET48
using System.Data.Entity;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public class AdminAppDbContext : DbContext
    {
        static AdminAppDbContext()
        {
            // We pass in null here because we want to suppress the overly-aggressive initializer provided by EF.
            // Instead, we trust that the EdFi_Admin database was created/populated through some other means, like DbUp.
            System.Data.Entity.Database.SetInitializer<AdminAppDbContext>(null);
        }

        public AdminAppDbContext()
            : base(CloudOdsDatabaseNames.Admin)
        {
        }

#if !NET48
        public AdminAppDbContext(string connectionString)
            : base(connectionString)
        {
        }
#endif

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("adminapp");
            modelBuilder.ApplyDatabaseServerSpecificConventions();
        }

        public DbSet<ApplicationConfiguration> ApplicationConfigurations { get; set; }
        public DbSet<SecretConfiguration> SecretConfigurations { get; set; }
        public DbSet<AzureSqlConfiguration> AzureSqlConfigurations { get; set; }
        public DbSet<OdsInstanceRegistration> OdsInstanceRegistrations { get; set; }

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
#endif
