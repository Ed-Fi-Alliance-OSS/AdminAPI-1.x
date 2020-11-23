// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public class AdminAppIdentityDbContext : IdentityDbContext<AdminAppUser>
    {
        private readonly IOptions<AppSettings> _appSettings;

        public AdminAppIdentityDbContext(DbContextOptions<AdminAppIdentityDbContext> options, IOptions<AppSettings> appSettings)
            : base(options)
        {
            _appSettings = appSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("adminapp");

            modelBuilder.Entity<AdminAppUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            modelBuilder.Entity<AdminAppUser>().Property(x => x.Id).HasMaxLength(225);

            modelBuilder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(225);

            modelBuilder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(225);

            modelBuilder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasMaxLength(225);

            modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(225);

            modelBuilder.Entity<IdentityUserRole<string>>().Property(x => x.UserId).HasMaxLength(225);
            modelBuilder.Entity<IdentityUserRole<string>>().Property(x => x.RoleId).HasMaxLength(225);

            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(225);
            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(112);
            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.Name).HasMaxLength(112);

            modelBuilder.Entity<UserOdsInstanceRegistration>()
                .HasKey(k => new { k.UserId, k.OdsInstanceRegistrationId });

            modelBuilder.ApplyDatabaseServerSpecificConventions(_appSettings.Value.DatabaseEngine);
        }

        public DbSet<UserOdsInstanceRegistration> UserOdsInstanceRegistrations { get; set; }
    }
}
