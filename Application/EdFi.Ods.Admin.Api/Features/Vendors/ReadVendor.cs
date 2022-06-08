// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.ErrorHandling;

namespace EdFi.Ods.Admin.Api.Features.Vendors;

public class ReadVendor : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{FeatureConstants.Vendors}", GetVendors).RequireAuthorization()
            .WithTags(FeatureConstants.Vendors)
            .WithMetadata(new OperationOrderAttribute(1));
        endpoints.MapGet($"/{FeatureConstants.Vendors}"+"/{id}", GetVendor).RequireAuthorization()
            .WithTags(FeatureConstants.Vendors)
            .WithMetadata(new OperationOrderAttribute(2));
    }

    internal Task<IResult> GetVendors(IGetVendorsQuery getVendorsQuery, IMapper mapper)
    {
        var vendorList = mapper.Map<List<VendorModel>>(getVendorsQuery.Execute());
        return Task.FromResult(AdminApiResponse<List<VendorModel>>.Ok(vendorList));
    }

    internal Task<IResult> GetVendor(IGetVendorByIdQuery getVendorByIdQuery, IMapper mapper, int id)
    {
        var vendor = getVendorByIdQuery.Execute(id);
        if (vendor == null)
        {
            throw new NotFoundException<int>("vendor", id);
        }
        var model = mapper.Map<VendorModel>(vendor);
        return Task.FromResult(AdminApiResponse<VendorModel>.Ok(model));
    }
}
