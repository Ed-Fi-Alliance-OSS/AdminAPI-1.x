// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;

public interface ICommandRepository<T> : IBaseRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
}

public class CommandRepository<T> : BaseRepository<T>, ICommandRepository<T> where T : class
{
    public CommandRepository(IDbContext context)
        : base(context)
    { }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }
}
