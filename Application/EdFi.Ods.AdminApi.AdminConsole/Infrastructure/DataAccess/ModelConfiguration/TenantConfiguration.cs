// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.ModelConfiguration
{
    internal class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        private readonly string _dbProvider;
        public TenantConfiguration(string dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public void Configure(EntityTypeBuilder<Tenant> entity)
        {
            entity.ToTable("Tenants", "adminconsole");
            entity.HasKey(e => e.DocId);
            switch (_dbProvider) // Add this line to get the database provider
            {
                case DbProviders.PostgreSql:
                    entity.Property(e => e.Document).HasColumnType("jsonb");
                    break;
                case DbProviders.SqlServer:
                    entity.Property(e => e.Document).HasColumnType("nvarchar(max)");
                    break;
            }

            entity.HasIndex(e => e.InstanceId);
            entity.HasIndex(e => e.TenantId).IsUnique();
            entity.HasIndex(e => e.EdOrgId);
        }
    }
}
