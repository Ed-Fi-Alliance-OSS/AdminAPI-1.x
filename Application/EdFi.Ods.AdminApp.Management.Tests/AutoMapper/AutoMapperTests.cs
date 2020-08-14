// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using AutoMapper;

namespace EdFi.Ods.AdminApp.Management.Tests.AutoMapper
{
    [TestFixture]
    public class AutoMapperTests
    {
        [Test]
        public void ConfigurationShouldBeValid()
        {
            var profiles = typeof(CloudOdsAdminApp).Assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Profile)))
                .Select(Activator.CreateInstance)
                .Cast<Profile>()
                .ToArray();

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                    cfg.AddProfile(profile);
            });

            config.AssertConfigurationIsValid();
        }
    }
}
