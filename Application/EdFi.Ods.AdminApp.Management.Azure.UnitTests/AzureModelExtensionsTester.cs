// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using Microsoft.Azure.Management.WebSites.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.UnitTests
{
    [TestFixture]
    public class AzureModelExtensionsTester
    {
        [Test]
        public void HasTagShouldGracefullyHandleNullTagsObject()
        {
            Site sut = new Site
            {
                Tags = null
            };

            AzureModelExtensions.HasTag(sut, "test").ShouldBeFalse();
        }

        [Test]
        public void HasTagShouldFindTagWhenItExists()
        {
            Site sut = new Site
            {
                Tags = new Dictionary<string, string>
                {
                    {"test", "test"}
                }
            };

            AzureModelExtensions.HasTag(sut, "test").ShouldBeTrue();
        }

        [Test]
        public void HasTagShouldNotFindTagWhenItDoesNotExist()
        {
            Site sut = new Site
            {
                Tags = new Dictionary<string, string>
                {
                    {"test", "test"}
                }
            };

            AzureModelExtensions.HasTag(sut, "test1").ShouldBeFalse();
        }


        [Test]
        public void HasTagShouldBeCaseSensitive()
        {
            Site sut = new Site
            {
                Tags = new Dictionary<string, string>
                {
                    {"test", "test"}
                }
            };

            AzureModelExtensions.HasTag(sut, "TEST").ShouldBeFalse();
        }

        [Test]
        public void HasTagWithValueShouldGracefullyHandleNullTagsObject()
        {
            Site sut = new Site
            {
                Tags = null
            };

            AzureModelExtensions.HasTagWithValue(sut, "test", "test").ShouldBeFalse();
        }

        [Test]
        public void HasTagWithValueShouldFindTagWhenValueMatches()
        {
            Site sut = new Site
            {
                Tags = new Dictionary<string, string>
                {
                    {"test", "test"}
                }
            };

            AzureModelExtensions.HasTagWithValue(sut, "test", "test").ShouldBeTrue();
        }

        [Test]
        public void HasTagWithValueShouldNotFindTagWhenItDoesNotExist()
        {
            Site sut = new Site
            {
                Tags = new Dictionary<string, string>
                {
                    {"test", "test"}
                }
            };

            AzureModelExtensions.HasTagWithValue(sut, "test", "test1").ShouldBeFalse();
        }

        [Test]
        public void HasWithValueTagShouldBeCaseSensitiveForKey()
        {
            Site sut = new Site
            {
                Tags = new Dictionary<string, string>
                {
                    {"test", "test"}
                }
            };

            AzureModelExtensions.HasTagWithValue(sut, "TEST", "TEST").ShouldBeFalse();
        }

        [Test]
        public void HasWithValueTagShouldBeCaseInsensitiveForValue()
        {
            Site sut = new Site
            {
                Tags = new Dictionary<string, string>
                {
                    {"test", "test"}
                }
            };

            AzureModelExtensions.HasTagWithValue(sut, "test", "TEST").ShouldBeTrue();
        }
    }
}
