// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

public interface IDeleteApplicationCommand
{
    void Execute(int id);
}

public class DeleteApplicationCommand : IDeleteApplicationCommand
{
    private readonly IUsersContext _context;

    public DeleteApplicationCommand(IUsersContext context)
    {
        _context = context;
    }

    public void Execute(int id)
    {
        var application = _context.Applications
            .Include(a => a.ApiClients)
            .Include(a => a.ApiClients.Select(c => c.ClientAccessTokens))
            .Include(a => a.ApplicationEducationOrganizations)
            .SingleOrDefault(a => a.ApplicationId == id) ?? throw new NotFoundException<int>("application", id);

        if (application != null && application.Vendor.IsSystemReservedVendor())
        {
            throw new Exception("This Application is required for proper system function and may not be modified");
        }

        if (application == null)
        {
            return;
        }

        var currentOdsInstanceId = application.OdsInstanceId();
        
        application.ApiClients.ToList().ForEach(a =>
        {
            RemoveApiClientOdsInstanceAssociation(currentOdsInstanceId,a.ApiClientId);
            a.ClientAccessTokens.ToList().ForEach(t => _context.ClientAccessTokens.Remove(t));
            _context.Clients.Remove(a);
        });

        application.ApplicationEducationOrganizations.ToList().ForEach(o => _context.ApplicationEducationOrganizations.Remove(o));


        _context.Applications.Remove(application);
        _context.SaveChanges();
    }

    private void RemoveApiClientOdsInstanceAssociation(int? odsInstanceId, int apiClientId)
    {
        var apiClientOdsInstance = _context.ApiClientOdsInstances.FirstOrDefault(o => o.OdsInstance.OdsInstanceId == odsInstanceId && o.ApiClient.ApiClientId == apiClientId);
        if (apiClientOdsInstance != null)
        { 
            _context.ApiClientOdsInstances.Remove(apiClientOdsInstance);
        }
    }
}
