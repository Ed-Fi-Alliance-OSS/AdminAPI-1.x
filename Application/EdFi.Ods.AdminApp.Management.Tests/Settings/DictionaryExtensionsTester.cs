// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Settings;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Settings
{
    [TestFixture]
    public class DictionaryExtensionsTester
    {
        [Test]
        public void GetRequiredStringValueShouldThrowIfKeyDoesNotExist()
        {
            var sut = new Dictionary<string, string>
            {
                {"test", "test"}
            };

            Assert.Throws<KeyNotFoundException>(() => sut.GetRequiredStringValue("foo"));
        }

        [Test]
        public void GetRequiredStringValueSuccessfullyRetrieveSetting()
        {
            var sut = new Dictionary<string, string>
            {
                {"test", "test"}
            };

            sut.GetRequiredStringValue("test").ShouldBe("test");
        }

        [Test]
        public void GetRequiredIntValueShouldThrowIfKeyDoesNotExist()
        {
            var sut = new Dictionary<string, string>
            {
                {"test", "test"}
            };

            Assert.Throws<KeyNotFoundException>(() => sut.GetRequiredStringValue("foo"));
        }

        [Test]
        public void GetRequiredIntValueShouldSuccessfullyRetrieveSetting()
        {
            var sut = new Dictionary<string, string>
            {
                {"test", "1"}
            };

            sut.GetRequiredIntValue("test").ShouldBe(1);
        }

        [Test]
        public void GetRequiredIntValueShouldThrowIfValueNotFormattedProperly()
        {
            var sut = new Dictionary<string, string>
            {
                {"test", "NotAnInteger"}
            };

            Assert.Throws<FormatException>(() => sut.GetRequiredIntValue("test"));
        }
    }
}
