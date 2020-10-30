// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
#if NET48
using System.Data.Entity;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public class AdminAppIdentityDbContext : IdentityDbContext<AdminAppUser>
    {
        static AdminAppIdentityDbContext()
        {
            // We pass in null here because we want to suppress the overly-aggressive initializer provided by EF.
            // Instead, we trust that the EdFi_Admin database was created/populated through some other means, like DbUp.
            System.Data.Entity.Database.SetInitializer<AdminAppIdentityDbContext>(null);
        }

        //Rather than construct AdminAppIdentityDbContext directly, most ASP.NET Identity user
        //management activities should take place through the ApplicationUserManager abstraction.
        public AdminAppIdentityDbContext()
            : base(CloudOdsDatabaseNames.Admin, throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("adminapp");

            modelBuilder.Entity<AdminAppUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<UserOdsInstanceRegistration>().ToTable("UserOdsInstanceRegistrations").HasKey(k => new { k.UserId, k.OdsInstanceRegistrationId });

            modelBuilder.ApplyDatabaseServerSpecificConventions();
        }

        public DbSet<UserOdsInstanceRegistration> UserOdsInstanceRegistrations { get; set; }
        public DbSet<IdentityUserRole> UserRoles { get; set; }

        public static AdminAppIdentityDbContext Create()
        {
            return new AdminAppIdentityDbContext();
        }
    }
}
#endif
