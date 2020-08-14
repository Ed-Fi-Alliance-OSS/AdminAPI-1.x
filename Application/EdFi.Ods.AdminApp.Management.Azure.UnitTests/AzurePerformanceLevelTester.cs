// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.UnitTests
{
    [TestFixture]
    public class AzurePerformanceLevelTester
    {
        [Test]
        public void ShouldBeValidOnlyIfEditionAndServiceObjectiveAreNotEmpty()
        {
            new AzurePerformanceLevel("", "").IsValid().ShouldBeFalse();
            new AzurePerformanceLevel(null, "").IsValid().ShouldBeFalse();
            new AzurePerformanceLevel("", null).IsValid().ShouldBeFalse();
            new AzurePerformanceLevel(null, null).IsValid().ShouldBeFalse();
            new AzurePerformanceLevel("Standard", "").IsValid().ShouldBeFalse();
            new AzurePerformanceLevel("Standard", null).IsValid().ShouldBeFalse();
            new AzurePerformanceLevel("", "S1").IsValid().ShouldBeFalse();
            new AzurePerformanceLevel(null, "S1").IsValid().ShouldBeFalse();
            new AzurePerformanceLevel("Standard", "S1").IsValid().ShouldBeTrue();
        }

        [Test]
        public void ShouldCalculateServiceOrdering()
        {
            new AzurePerformanceLevel("", "").ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("", null).ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("", "S").ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("", "SS").ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("", "S1").ServiceOrdering.ShouldBe(1);
            new AzurePerformanceLevel("", "S10").ServiceOrdering.ShouldBe(10);
            new AzurePerformanceLevel("Standard", "").ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("Standard", null).ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("Standard", "S").ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("Standard", "SS").ServiceOrdering.ShouldBe(0);
            new AzurePerformanceLevel("Standard", "S1").ServiceOrdering.ShouldBe(1);
            new AzurePerformanceLevel("Standard", "S10").ServiceOrdering.ShouldBe(10);
        }

        [Test]
        public void ShouldCalculateEqualityProperly()
        {
            new AzurePerformanceLevel("", "").Equals(new AzurePerformanceLevel("", "")).ShouldBeTrue();
            new AzurePerformanceLevel("Standard", "").Equals(new AzurePerformanceLevel("", "")).ShouldBeFalse();
            new AzurePerformanceLevel("", "S1").Equals(new AzurePerformanceLevel("", "")).ShouldBeFalse();
            new AzurePerformanceLevel("Standard", "S1").Equals(new AzurePerformanceLevel("", "")).ShouldBeFalse();
            new AzurePerformanceLevel("Standard", "S1").Equals(new AzurePerformanceLevel("Standard", "")).ShouldBeFalse();
            new AzurePerformanceLevel("Standard", "S1").Equals(new AzurePerformanceLevel("", "S1")).ShouldBeFalse();
            new AzurePerformanceLevel("Standard", "S1").Equals(new AzurePerformanceLevel("Standard", "S2")).ShouldBeFalse();
            new AzurePerformanceLevel("Standard", "S1").Equals(new AzurePerformanceLevel("Standard", "S1")).ShouldBeTrue();
        }

        [Test]
        public void ShouldCompareProperly()
        {
            (new AzurePerformanceLevel("", "") < new AzurePerformanceLevel("", "")).ShouldBeFalse();
            (new AzurePerformanceLevel("", "S1") < new AzurePerformanceLevel("", "S1")).ShouldBeFalse();
            (new AzurePerformanceLevel("", "S1") < new AzurePerformanceLevel("", "S2")).ShouldBeTrue();
            (new AzurePerformanceLevel("Standard", "") < new AzurePerformanceLevel("Standard", "")).ShouldBeFalse();
            (new AzurePerformanceLevel("Standard", "S1") < new AzurePerformanceLevel("Standard", "S0")).ShouldBeFalse();
            (new AzurePerformanceLevel("Standard", "S1") < new AzurePerformanceLevel("Fake", "S1")).ShouldBeFalse();
            (new AzurePerformanceLevel("Standard", "S1") < new AzurePerformanceLevel("Fake", "S2")).ShouldBeFalse();
            (new AzurePerformanceLevel("Standard", "S1") < new AzurePerformanceLevel("Standard", "S2")).ShouldBeTrue();
            (new AzurePerformanceLevel("Fake", "S1") < new AzurePerformanceLevel("Fake", "S2")).ShouldBeTrue();

            (new AzurePerformanceLevel("", "") > new AzurePerformanceLevel("", "")).ShouldBeFalse();
            (new AzurePerformanceLevel("", "S1") > new AzurePerformanceLevel("", "S1")).ShouldBeFalse();
            (new AzurePerformanceLevel("", "S1") > new AzurePerformanceLevel("", "S2")).ShouldBeFalse();
            (new AzurePerformanceLevel("Standard", "") > new AzurePerformanceLevel("Standard", "")).ShouldBeFalse();
            (new AzurePerformanceLevel("Standard", "S1") > new AzurePerformanceLevel("Standard", "S0")).ShouldBeTrue();
            (new AzurePerformanceLevel("Standard", "S1") > new AzurePerformanceLevel("Fake", "S1")).ShouldBeTrue();
            (new AzurePerformanceLevel("Standard", "S1") > new AzurePerformanceLevel("Fake", "S2")).ShouldBeTrue();
            (new AzurePerformanceLevel("Standard", "S1") > new AzurePerformanceLevel("Standard", "S2")).ShouldBeFalse();
            (new AzurePerformanceLevel("Fake", "S1") > new AzurePerformanceLevel("Fake", "S2")).ShouldBeFalse();
        }

    }
}
