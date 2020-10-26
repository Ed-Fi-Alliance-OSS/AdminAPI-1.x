// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
#if !NET48
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public class AdminAppIdentityDbContext : IdentityDbContext<AdminAppUser>
    {
        public AdminAppIdentityDbContext(DbContextOptions<AdminAppIdentityDbContext> options)
            : base(options)
        {
        }

        private AdminAppIdentityDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("adminapp");

            modelBuilder.Entity<UserOdsInstanceRegistration>()
                .HasKey(k => new { k.UserId, k.OdsInstanceRegistrationId });

            modelBuilder.ApplyDatabaseServerSpecificConventions();
        }

        public DbSet<UserOdsInstanceRegistration> UserOdsInstanceRegistrations { get; set; }

        public static AdminAppIdentityDbContext Create()
        {
            return new AdminAppIdentityDbContext();
        }
    }
}
#endif
