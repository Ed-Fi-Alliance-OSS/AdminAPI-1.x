// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Web.Display.Pagination;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.Display.Pagination
{
    [TestFixture]
    public class PagedListAsyncTests
    {
        private static IReadOnlyList<object> ListOfObjects => new object[100];

        private static Task<IReadOnlyList<object>> GetListOfObjects()
        {
            return Task.FromResult(ListOfObjects);
        }

        private static async Task<PagedList<object>> FetchPagedObjects(int pageNumber) =>
            await Page<object>.FetchAsync(async (offset, size) => (await GetListOfObjects()).Skip(offset).Take(size).ToList(), pageNumber);

        private static async Task<PagedList<object>> FetchPagedObjects(int pageNumber, int pageSize) =>
            await Page<object>.FetchAsync(async (offset, size) => (await GetListOfObjects()).Skip(offset).Take(size).ToList(), pageNumber, pageSize);

        [Test]
        public async Task ShouldReturnHumanPageNumber()
        {
            var pageNumber = 0;

            var pagedList = await FetchPagedObjects(pageNumber);
            pagedList.PageNumber.ShouldBe(1);

            pageNumber = 1;

            pagedList = await FetchPagedObjects(pageNumber);
            pagedList.PageNumber.ShouldBe(1);

            pageNumber = 15;

            pagedList = await FetchPagedObjects(pageNumber);
            pagedList.PageNumber.ShouldBe(15);
        }

        [Test]
        public async Task ShouldCalculateOffsetCorrectlyForFirstPage()
        {
            const int pageNumber = 0;
            const int configuredPageSize = 30;

            await Page<object>.FetchAsync(async (offset, size) =>
            {
                offset.ShouldBe(0);

                return await GetListOfObjects();
            }, pageNumber);

            await Page<object>.FetchAsync(
                async (offset, size) =>
            {
                offset.ShouldBe(0);

                return await GetListOfObjects();
            }, pageNumber, configuredPageSize);
        }

        [Test]
        public async Task ShouldCalculateOffsetCorrectlyForNPage()
        {
            var pageNumber = 2;
            const int configuredPageSize = 25;

            await Page<object>.FetchAsync(async (offset, size) =>
            {
                offset.ShouldBe(Page<object>.DefaultPageSize);

                return await GetListOfObjects();
            }, pageNumber);

            pageNumber = 10;

            await Page<object>.FetchAsync(async (offset, size) =>
            {
                offset.ShouldBe(90);

                return await GetListOfObjects();
            }, pageNumber);

            await Page<object>.FetchAsync(async (offset, size) =>
            {
                var calculatedOffSet = (pageNumber - 1) * configuredPageSize;

                offset.ShouldBe(calculatedOffSet);

                return await GetListOfObjects();
            }, pageNumber, configuredPageSize);
        }

        [Test]
        public async Task ShouldRequestOneAdditionalRecordAsync()
        {
            await Page<object>.FetchAsync(async (offset, size) =>
            {
                size.ShouldBe(Page<object>.DefaultPageSize + 1);

                return await GetListOfObjects();
            }, 0);

            const int ConfiguredPageSize = 50;

            await Page<object>.FetchAsync(async (offset, size) =>
            {
                size.ShouldBe(51);

                return await GetListOfObjects();
            }, 0, ConfiguredPageSize);
        }

        [Test]
        public async Task ShouldDetectIfNextPageHasResults()
        {
            const int configuredPageSize = 50;
            var pageNumber = 1;

            var pagedObjects = await FetchPagedObjects(pageNumber, configuredPageSize);
            pagedObjects.NextPageHasResults.ShouldBeTrue();

            pageNumber = 2;

            pagedObjects = await FetchPagedObjects(pageNumber, configuredPageSize);
            pagedObjects.NextPageHasResults.ShouldBeFalse();
        }

        [Test]
        public async Task ShouldReturnSameAmountOfItemsAsPageSize()
        {
            const int configuredPageSize = 10;
            const int pageNumber = 1;

            var pagedObjects = await FetchPagedObjects(pageNumber, configuredPageSize);

            pagedObjects.Items.Count.ShouldBe(configuredPageSize);
        }

        [Test]
        public void ShouldNotAllowPageSizeOf100()
        {
            var pageSize = 99;

            Should.NotThrow(() => FetchPagedObjects(1, pageSize));

            pageSize = 100;

            Should.Throw<ArgumentException>(() => FetchPagedObjects(1, pageSize));
        }
    }
}
