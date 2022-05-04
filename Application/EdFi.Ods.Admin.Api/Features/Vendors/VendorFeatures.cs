// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database;

namespace EdFi.Ods.Admin.Api.Features.Vendors;

public class VendorFeatures : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/vendors", GetVendors);
        endpoints.MapGet("/vendors/{id}", GetVendor);
        endpoints.MapPost("/vendors", AddVendor);
        endpoints.MapPut("/vendors", UpdateVendor);
        endpoints.MapDelete("/vendors/{id}", DeleteVendor);
    }

    internal Task<IResult> GetVendors(AdminAppDbContext dbContext)
    {
        var vendorsList = new[] { "Vendor1", "Vendor2" };
        return Task.FromResult(AdminApiResponse<string[]>.Ok(vendorsList));
    }

    internal Task<IResult> GetVendor(AdminAppDbContext dbContext, int id)
    {
        CheckIfExists(id);
        return Task.FromResult(AdminApiResponse<VendorModel>.Ok(new VendorModel { Id = id, Name = $"Vendor {id}", Description = "A vendor"}));
    }

    internal async Task<IResult> AddVendor(AdminAppDbContext dbContext, VendorModelValidator validator, VendorModel vendor)
    {
        await validator.GuardAsync(vendor);
        vendor.Id = 1;
        return AdminApiResponse<VendorModel>.Created(vendor, "Vendor", "/Vendors/1");
    }

    internal async Task<IResult> UpdateVendor(AdminAppDbContext dbContext, VendorModelValidator validator, VendorModel vendor)
    {
        await validator.GuardAsync(vendor);
        CheckIfExists(vendor.Id!.Value);
        return AdminApiResponse<VendorModel>.Updated(vendor, "Vendor");
    }

    internal Task<IResult> DeleteVendor(AdminAppDbContext dbContext, int id)
    {
        CheckIfExists(id);
        return  Task.FromResult(AdminApiResponse.Deleted("Vendor"));
    }

    private void CheckIfExists(int id)
    {
        if (id < 0)
            throw new NotFoundException<int>("Vendor", id);
    }
}
