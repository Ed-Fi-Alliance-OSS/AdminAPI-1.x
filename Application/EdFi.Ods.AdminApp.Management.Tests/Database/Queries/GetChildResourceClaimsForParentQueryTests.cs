// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Security.DataAccess.Contexts;
using NUnit.Framework;
using Shouldly;
using Application = EdFi.Security.DataAccess.Models.Application;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetChildResourceClaimsForParentQueryTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldGetResourceClaims()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };

            Save(testApplication);

            var testResourceClaims = SetupResourceClaims(testApplication);

            var testParentResource = testResourceClaims.Single(x => x.ResourceName == "TestParentResourceClaim1");

            ResourceClaim[] results = null;
            Scoped<ISecurityContext>(securityContext =>
            {
                var query = new GetChildResourceClaimsForParentQuery(securityContext);

                results = query.Execute(testParentResource.ResourceClaimId).ToArray();
            });

            Scoped<ISecurityContext>(securityContext =>
            {
                var testChildResourceClaims = securityContext.ResourceClaims.Where(x =>
                    x.ParentResourceClaimId == testParentResource.ResourceClaimId);

                results.Length.ShouldBe(testChildResourceClaims.Count());
                results.Select(x => x.Name).ShouldBe(testChildResourceClaims.Select(x => x.ResourceName), true);
                results.Select(x => x.Id).ShouldBe(testChildResourceClaims.Select(x => x.ResourceClaimId), true);
                results.All(x => x.Create == false).ShouldBe(true);
                results.All(x => x.Delete == false).ShouldBe(true);
                results.All(x => x.Update == false).ShouldBe(true);
                results.All(x => x.Read == false).ShouldBe(true);
                results.All(x => x.ParentId.Equals(testParentResource.ResourceClaimId)).ShouldBe(true);
            });
        }
    }
}
