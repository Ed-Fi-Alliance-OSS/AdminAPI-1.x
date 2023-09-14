// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

public interface IEditOdsInstanceDerivativeCommand
{
    OdsInstanceDerivative Execute(IEditOdsInstanceDerivativeModel changedOdsInstanceDerivativeData);
}

public class EditOdsInstanceDerivativeCommand : IEditOdsInstanceDerivativeCommand
{
    private readonly IUsersContext _context;

    public EditOdsInstanceDerivativeCommand(IUsersContext context)
    {
        _context = context;
    }

    public OdsInstanceDerivative Execute(IEditOdsInstanceDerivativeModel changedOdsInstanceDerivativeData)
    {
        var odsInstanceDerivative = _context.OdsInstanceDerivatives.SingleOrDefault(v => v.OdsInstanceDerivativeId == changedOdsInstanceDerivativeData.Id) ??
            throw new NotFoundException<int>("odsInstanceDerivative", changedOdsInstanceDerivativeData.Id);

        odsInstanceDerivative.DerivativeType = changedOdsInstanceDerivativeData.DerivativeType;
        odsInstanceDerivative.OdsInstanceId = changedOdsInstanceDerivativeData.OdsInstanceId;
        odsInstanceDerivative.ConnectionString = changedOdsInstanceDerivativeData.ConnectionString;

        _context.SaveChanges();
        return odsInstanceDerivative;
    }
}

public interface IEditOdsInstanceDerivativeModel
{
    public int Id { get; set; }
    public int OdsInstanceId { get; set; }
    public string? DerivativeType { get; set; }
    public string? ConnectionString { get; set; }
}
