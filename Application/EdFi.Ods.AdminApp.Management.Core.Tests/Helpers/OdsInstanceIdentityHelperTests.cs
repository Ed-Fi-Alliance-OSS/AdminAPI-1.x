// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Helpers;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Helpers
{
    [TestFixture]
    public class OdsInstanceIdentityHelperTests
    {
        [Test]
        public void ShouldReturnExpectedEdOrgId()
        {
            const int expectedEdOrgId = 255901;

            var returnedEdOrgId = OdsInstanceIdentityHelper.GetIdentityValue($"EdFi_Ods_{expectedEdOrgId}");

            returnedEdOrgId.ShouldBe(expectedEdOrgId);
        }

        [Test]
        public void ShouldReturnExpectedYear()
        {
            const int expectedYearValue = 2019;

            var returnedYearValue = OdsInstanceIdentityHelper.GetIdentityValue($"EdFi_Ods_{expectedYearValue}");

            returnedYearValue.ShouldBe(expectedYearValue);
        }
    }
}