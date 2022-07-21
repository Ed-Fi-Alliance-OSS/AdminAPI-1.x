// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using EndpointExtensions = EdFi.Ods.Admin.Api.Infrastructure.EndpointRouteBuilderExtensions;

namespace EdFi.Ods.Admin.Api.Features.Vendors;

public class ReadVendor : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, $"{FeatureConstants.Vendors}", GetVendors)
            .WithRouteOptions(rhb => EndpointExtensions.DefaultGetOptions(rhb, FeatureConstants.Vendors))
            .BuildForVersions(AdminApiVersions.V1, AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, $"/{FeatureConstants.Vendors}" + "/{id}", GetVendor)
            .WithRouteOptions(rhb => EndpointExtensions.DefaultGetOptions(rhb, FeatureConstants.Vendors))
            .BuildForVersions(AdminApiVersions.V1, AdminApiVersions.V2);
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
