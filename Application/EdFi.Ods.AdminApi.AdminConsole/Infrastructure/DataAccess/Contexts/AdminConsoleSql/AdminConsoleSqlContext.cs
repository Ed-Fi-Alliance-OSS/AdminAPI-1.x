// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.ModelConfiguration;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsoleSql;

public class AdminConsoleSqlContext : DbContext, IDbContext
{
    public AdminConsoleSqlContext(DbContextOptions<AdminConsoleSqlContext> options) : base(options) { }

    public DbSet<HealthCheck> HealthChecks { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<Instance> Instances { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        const string DbProvider = DbProviders.SqlServer;
        modelBuilder.ApplyConfiguration(new HealthCheckConfiguration(DbProvider));
        modelBuilder.ApplyConfiguration(new TenantConfiguration(DbProvider));
        modelBuilder.ApplyConfiguration(new InstanceConfiguration(DbProvider));
        modelBuilder.ApplyConfiguration(new PermissionConfiguration(DbProvider));
    }
}
