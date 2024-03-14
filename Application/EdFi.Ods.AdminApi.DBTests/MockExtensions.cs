// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EdFi.Ods.AdminApi.DBTests
{
    public static class MockExtensions
    {
        public static Mock<DbSet<T>> MockDbSet<T>(List<T> underlyingData) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            var cancellationToken = CancellationToken.None;
            mockSet.ConfigureDbSetWithData(underlyingData, cancellationToken);

            return mockSet;
        }

        public static Mock<DbSet<T>> EmptyMockDbSet<T>() where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            var underlyingData = new List<T>();
            var cancellationToken = CancellationToken.None;

            ConfigureDbSetWithData(mockSet, underlyingData, cancellationToken);

            return mockSet;
        }

        public static Mock<DbSet<T>> ConfigureDbSetWithData<T>(this Mock<DbSet<T>> mockSet, List<T> underlyingData, CancellationToken cancellationToken) where T : class
        {
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(cancellationToken))
                .Returns(() => new TestDbAsyncEnumerator<T>(underlyingData.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(() => new TestDbAsyncQueryProvider<T>(underlyingData.AsQueryable().Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(() => underlyingData.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(() => underlyingData.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => underlyingData.AsQueryable().GetEnumerator());

            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback((T x) => underlyingData.Add(x));
            mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<T>>())).Callback((IEnumerable<T> x) => underlyingData.AddRange(x));
            mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback((T x) => underlyingData.Remove(x));
            mockSet.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<T>>())).Callback((IEnumerable<T> x) => underlyingData.RemoveAll(x.Contains));

            return mockSet;
        }
    }
}
