// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;

public interface IDeleteInstanceCommand
{
    Task Execute(int OdsInstanceId);
}

public class DeleteInstanceCommand : IDeleteInstanceCommand
{
    private readonly ICommandRepository<Instance> _instanceCommand;
    private readonly IQueriesRepository<Instance> _instanceQuery;

    public DeleteInstanceCommand(ICommandRepository<Instance> instanceCommand, IQueriesRepository<Instance> instanceQuery)
    {
        _instanceCommand = instanceCommand;
        _instanceQuery = instanceQuery;
    }

    public async Task Execute(int OdsInstanceId)
    {
        var instance = await _instanceQuery.Query().SingleOrDefaultAsync(w => w.OdsInstanceId == OdsInstanceId) ?? throw new NotFoundException<int>("Instance", OdsInstanceId);
        await _instanceCommand.DeleteAsync(instance);
    }
}
