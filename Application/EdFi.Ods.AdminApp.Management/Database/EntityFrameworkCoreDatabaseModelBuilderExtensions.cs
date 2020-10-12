// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if !NET48
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public static class EntityFrameworkCoreDatabaseModelBuilderExtensions
    {
        public static void ApplyDatabaseServerSpecificConventions(this ModelBuilder modelBuilder)
        {
            if (DatabaseProviderHelper.PgSqlProvider)
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    entity.SetTableName(entity.GetTableName().ToLowerInvariant());

                    foreach (var property in entity.GetProperties())
                        property.SetColumnName(property.GetColumnName().ToLowerInvariant());

                    foreach (var key in entity.GetKeys())
                        key.SetName(key.GetName().ToLowerInvariant());

                    foreach (var key in entity.GetForeignKeys())
                        key.SetConstraintName(key.GetConstraintName().ToLowerInvariant());

                    foreach (var index in entity.GetIndexes())
                        index.SetName(index.GetName().ToLowerInvariant());
                }
            }
        }
    }
}
#endif
