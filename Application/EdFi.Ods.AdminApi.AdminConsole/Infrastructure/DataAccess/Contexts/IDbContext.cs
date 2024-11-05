// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;

public interface IDbContext
{
    DbSet<HealthCheck> HealthChecks { get; set; }

    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
