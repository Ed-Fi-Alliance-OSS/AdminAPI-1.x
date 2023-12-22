// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

//using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using NUnit.Framework;
using EdFi.Admin.DataAccess.Models;
using Shouldly;
using EdFi.Security.DataAccess.Models;
using Application = EdFi.Admin.DataAccess.Models.Application;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetResourceClaimByResourceClaimIdQueryTests : SecurityDataTestBase
{
    [Test]
    public void ShouldGetResourceClaimByResourceClaimId()
    {
        Transaction(usersContext =>
        {
            var testResourceClaims = SetupResourceClaims(usersContext);
            var query = new GetResourceClaimByResourceClaimIdQuery(usersContext);

            var result = query.Execute(testResourceClaims.ResourceClaimId);

            result.Name.ShouldBe(testResourceClaims.ResourceName);
            
        });
       
        
    }

    [Test]
    public void ShouldGetResourceClaimChildrenByResourceClaimId()
    {
        Transaction(usersContext =>
        {
            var testResourceClaims = SetupResourceClaims(usersContext);
            var query = new GetResourceClaimByResourceClaimIdQuery(usersContext);

            var result = query.Execute(testResourceClaims.ResourceClaimId);

            result.Children.Count.ShouldBe(1);
            result.Children.ShouldAllBe(p => p.ParentId == testResourceClaims.ResourceClaimId);
        });


    }

    private ResourceClaim SetupResourceClaims(ISecurityContext usersContext)
    {
        var testApplication = new Application
        {
            ApplicationName = "TestApplicationName"
        };

        SaveAdminContext(testApplication);
        var parentResourceClaim = new ResourceClaim()
        {
            ClaimName = $"ParentTestResourceClaim",
            ResourceName = $"ParentTestResourceClaim",
        };
        usersContext.ResourceClaims.Add(parentResourceClaim);
        Save(parentResourceClaim);

        var resourceClaim = new ResourceClaim()
        {
            ClaimName = $"ChildrenTestResourceClaim",
            ResourceName = $"ChildrenTestResourceClaim",
            ParentResourceClaimId = parentResourceClaim.ResourceClaimId,
        };
        Save(resourceClaim);

        return parentResourceClaim;
    }
}
