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
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using System.Net;
using EdFi.Security.DataAccess.Contexts;

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

            Scoped<IGetClaimSetByIdQuery>(query =>
            {
                var result = query.Execute(testClaimSet.ClaimSetId);

                result.Name.ShouldBe(testClaimSet.ClaimSetName);
                result.Id.ShouldBe(testClaimSet.ClaimSetId);
            });
        }

        [Test]
        public void ShouldThrowExceptionForNonExistingClaimSetId()
        {
            EnsureZeroClaimSets();

            const int NonExistingClaimSetId = 1;

            var adminAppException = Assert.Throws<AdminAppException>(() => Scoped<IGetClaimSetByIdQuery>(query =>
            {
                query.Execute(NonExistingClaimSetId);
            }));
            adminAppException.ShouldNotBeNull();
            adminAppException.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            adminAppException.Message.ShouldBe("No such claim set exists in the database.");

            void EnsureZeroClaimSets()
            {
                Scoped<ISecurityContext>(database =>
                {
                    foreach (var entity in database.ClaimSets)
                        database.ClaimSets.Remove(entity);
                    database.SaveChanges();
                });
            }
        }
    }
}
