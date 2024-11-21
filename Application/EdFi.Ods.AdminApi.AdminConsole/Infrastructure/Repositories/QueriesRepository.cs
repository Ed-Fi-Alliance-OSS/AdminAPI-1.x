// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;

public interface IQueriesRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> Query();
}

public class QueriesRepository<T> : IQueriesRepository<T> where T : class
{
    private readonly IDbContext _context;
    private readonly DbSet<T> _dbSet;

    public QueriesRepository(IDbContext context)
    {
        _context = context;
        if (_context.DB.GetPendingMigrations().Count() > 0)
            _context.DB.Migrate();
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public IQueryable<T> Query()
    {
        return _dbSet;
    }
}
