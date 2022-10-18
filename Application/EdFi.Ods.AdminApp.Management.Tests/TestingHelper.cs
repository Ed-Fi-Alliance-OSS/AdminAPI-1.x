// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    using System;

    public static class TestingHelper
    {
        public static TResult Query<TResult>(Func<AdminAppDbContext, TResult> query)
        {
            TResult result = default(TResult);

            Scoped<AdminAppDbContext>(database =>
            {
                result = query(database);
            });

            return result;
        }

        public static TEntity Query<TEntity>(int id) where TEntity : Entity
        {
            return Query(database => database.Set<TEntity>().Find(id));
        }

        public static void Save<TEntity>(TEntity value) where TEntity : Entity
        {
            Scoped<AdminAppDbContext>(database =>
            {
                database.Set<TEntity>().Add(value);
                database.SaveChanges();
            });
        }

        public static void Save<TEntity>(List<TEntity> values) where TEntity : Entity
        {
            Scoped<AdminAppDbContext>(database =>
            {
                database.Set<TEntity>().AddRange(values);
                database.SaveChanges();
            });
        }

        public static void ShouldBeNull<TEntity>(Func<TEntity, bool> booleanQueryExpression) where TEntity : Entity
        {
            Query(database => database.Set<TEntity>()
                .FirstOrDefault(booleanQueryExpression)).ShouldBeNull();
        }

        public static void ShouldNotBeNull<TEntity>(Func<TEntity, bool> booleanQueryExpression) where TEntity : Entity
        {
            Query(database => database.Set<TEntity>()
                .FirstOrDefault(booleanQueryExpression)).ShouldNotBeNull();
        }

        public static string Sample(string prefix)
            => prefix + "-" + Guid.NewGuid();

        public static string Sample(string prefix, int length)
        {
            var sampleString = prefix + "-";

            while (sampleString.Length < length)
            {
                sampleString += Guid.NewGuid();
            }

            return sampleString.Substring(0, length);
        }
    }
}
