// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApi.Features.Vendors;
using EdFi.Ods.AdminApi.Features;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.UnitTests.Features
{
    [TestFixture]
    public class SortableListTests
    {
        [Test]
        public void Should_retrieve_vendors_with_sorting()
        {
            var vendors = new SortableList<VendorModel>();

            vendors.AddRange(new List<VendorModel>{ new VendorModel
        {
            Company = "test vendor 2",
            NamespacePrefixes = "http://testvendor2.net",
            ContactName = "test user 2",
            ContactEmailAddress = "testuser@test.com"
        }, { new VendorModel
        {
            Company = "test vendor 3",
            NamespacePrefixes = "http://testvendor3.net",
            ContactName = "test user 3",
            ContactEmailAddress = "testuser@test.com" }
         }, { new VendorModel
        {
            Company = "test vendor 1",
            NamespacePrefixes = "http://testvendor1.net",
            ContactName = "test user 1",
            ContactEmailAddress = "testuser@test.com" }
         }});

            /// ContactName
            string orderBy = "ContactName";
            var sortedList = vendors.Sort(orderBy);

            sortedList.ElementAt(0).Company.ShouldBe("test vendor 1");
            sortedList.ElementAt(1).Company.ShouldBe("test vendor 2");
            sortedList.ElementAt(2).Company.ShouldBe("test vendor 3");
        }

        [Test]
        public void Should_retrieve_vendors_with_descending_sorting()
        {
            var vendors = new SortableList<VendorModel>();

            vendors.AddRange(new List<VendorModel>{ new VendorModel
        {
            Company = "test vendor 2",
            NamespacePrefixes = "http://testvendor2.net",
            ContactName = "test user 2",
            ContactEmailAddress = "testuser@test.com"
        }, { new VendorModel
        {
            Company = "test vendor 3",
            NamespacePrefixes = "http://testvendor3.net",
            ContactName = "test user 3",
            ContactEmailAddress = "testuser@test.com" }
         }, { new VendorModel
        {
            Company = "test vendor 1",
            NamespacePrefixes = "http://testvendor1.net",
            ContactName = "test user 1",
            ContactEmailAddress = "testuser@test.com" }
         }});

            /// ContactName
            string orderBy = "ContactName";
            var sortedList = vendors.Sort(orderBy, SortingDirection.SortDirection.Descending.ToString());

            sortedList.ElementAt(0).Company.ShouldBe("test vendor 3");
            sortedList.ElementAt(1).Company.ShouldBe("test vendor 2");
            sortedList.ElementAt(2).Company.ShouldBe("test vendor 1");
        }

        [Test]
        public void Should_retrieve_vendors_with_sorting_by_default_column()
        {
            var vendors = new SortableList<VendorModel>();

            vendors.AddRange(new List<VendorModel>{ new VendorModel
        {
            Company = "test vendor 2",
            NamespacePrefixes = "http://testvendor2.net",
            ContactName = "test user 2",
            ContactEmailAddress = "testuser@test.com"
        }, { new VendorModel
        {
            Company = "test vendor 3",
            NamespacePrefixes = "http://testvendor3.net",
            ContactName = "test user 3",
            ContactEmailAddress = "testuser@test.com" }
         }, { new VendorModel
        {
            Company = "test vendor 1",
            NamespacePrefixes = "http://testvendor1.net",
            ContactName = "test user 1",
            ContactEmailAddress = "testuser@test.com" }
         }});

            /// ContactName
            string orderBy = "notExistingColumn";
            var sortedList = vendors.Sort(orderBy);

            sortedList.ElementAt(0).Company.ShouldBe("test vendor 1");
            sortedList.ElementAt(1).Company.ShouldBe("test vendor 2");
            sortedList.ElementAt(2).Company.ShouldBe("test vendor 3");
        }
    }
}
