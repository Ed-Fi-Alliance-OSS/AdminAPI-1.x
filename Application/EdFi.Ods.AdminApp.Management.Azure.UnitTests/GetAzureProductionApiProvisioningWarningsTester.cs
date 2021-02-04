// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.UnitTests
{
    [TestFixture]
    public class GetAzureProductionApiProvisioningWarningsTester
    {
        private static AzureCloudOdsInstance GetAzureCloudOdsInstance()
        {
            return new AzureCloudOdsInstance
            {
                Edition = "",
                FriendlyName = "",
                SystemId = "",
                SystemName = "",
                Version = ""
            };
        }

        [Test]
        public async Task ShouldNotProduceWarningWhenRunningAgainstNonAzureSql()
        {
            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var sut = new GetAzureProductionApiProvisioningWarningsQuery();
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.ShouldBeEmpty();
        }

        [Test]
        public async Task ShouldNotProduceWarningWhenRunningAgainstExpectedAzureSqlTier()
        {
            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var sut = new GetAzureProductionApiProvisioningWarningsQuery();
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.ShouldBeEmpty();
        }

        [Test]
        public async Task ShouldProduceWarningWhenRunningAgainstLowerThanRecommendedAzureSqlTier()
        {
            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var sut = new GetAzureProductionApiProvisioningWarningsQuery();
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.Count().ShouldBe(1);
            result.Warnings.First().ShouldBe("Azure Sql performance tier 'P1' or above is recommended");
        }

        [Test]
        public async Task ShouldProduceWarningWhenWebsiteLowerThanRecommendedTier()
        {
            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var sut = new GetAzureProductionApiProvisioningWarningsQuery();
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.Count().ShouldBe(1);
            result.Warnings.First().ShouldBe("Azure Website performance tier 'S3' or above is recommended");
        }
    }
}
