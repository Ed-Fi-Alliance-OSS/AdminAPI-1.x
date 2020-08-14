// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using NUnit.Framework;
using Shouldly;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using Application = EdFi.Security.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class GetClaimSetByIdQueryTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldGetClaimSetById()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet {ClaimSetName = "TestClaimSet", Application = testApplication};
            Save(testClaimSet);

            var query = new GetClaimSetByIdQuery(TestContext);
            var result = query.Execute(testClaimSet.ClaimSetId);

            result.Name.ShouldBe(testClaimSet.ClaimSetName);
            result.Id.ShouldBe(testClaimSet.ClaimSetId);
        }
    }
}
