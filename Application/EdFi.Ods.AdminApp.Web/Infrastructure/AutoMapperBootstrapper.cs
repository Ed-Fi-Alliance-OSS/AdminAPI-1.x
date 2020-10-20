// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if NET48
using AutoMapper;
using EdFi.Ods.AdminApp.Management;
using System;
using System.Linq;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public static class AutoMapperBootstrapper
    {
        public static IMapper CreateMapper()
        {
            var profiles = typeof(AutoMapperBootstrapper).Assembly.GetTypes()
                .Union(typeof(CloudOdsAdminApp).Assembly.GetTypes())
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
            return new Mapper(config);
        }
    }
}
#endif
