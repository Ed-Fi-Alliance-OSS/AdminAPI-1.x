// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
extern alias Compatability;

using ClaimSetEntity = Compatability::EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using ISecurityContext = Compatability::EdFi.SecurityCompatiblity53.DataAccess.Contexts.ISecurityContext;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class AddClaimSetCommandV53Service
{
    private readonly ISecurityContext _context;

    public AddClaimSetCommandV53Service(ISecurityContext context)
    {
        _context = context;
    }

    public int Execute(IAddClaimSetModel claimSet)
    {
        var newClaimSet = new ClaimSetEntity
        {
            ClaimSetName = claimSet.ClaimSetName,
            Application = _context.Applications.Single()
        };
        _context.ClaimSets.Add(newClaimSet);
        _context.SaveChanges();

        return newClaimSet.ClaimSetId;
    }
}
