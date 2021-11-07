// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.EntityFrameworkCore;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.TestingHelper;

namespace EdFi.Ods.AdminApp.Management.Tests.Instance
{
    public static class InstanceTestSetup
    {
        public static void ResetOdsInstanceRegistrations()
        {
            Scoped<AdminAppDbContext>(dbContext =>
            {
                dbContext.Database.ExecuteSqlRaw("DELETE FROM [adminapp].[OdsInstanceRegistrations]");
            });
        }

        public static IEnumerable<OdsInstanceRegistration> SetupOdsInstanceRegistrations(int instanceCount = 5, bool useGuidName = false)
        {
            var testInstances = Enumerable.Range(1, instanceCount)
                .Select((x, index) => new OdsInstanceRegistration
                {
                    Name = useGuidName ? $"Ods{Guid.NewGuid():N}_{index:D4}" : $"Ods_{index:D4}",
                    DatabaseName = $"DatabaseName_{index}",
                    Description = Sample("Description")
                })
                .ToList();

            Save(testInstances);

            SetupOdsInstanceSecretConfigurations(testInstances);

            return testInstances;
        }

        public static OdsInstanceRegistration SetupOdsInstanceRegistration(string instanceName)
        {
            var testInstance = new OdsInstanceRegistration
            {
                Name = instanceName,
                DatabaseName = instanceName,
                Description = Sample("Description")
            };

            Save(testInstance);

            SetupOdsInstanceSecretConfigurations(new List<OdsInstanceRegistration>{testInstance});

            return testInstance;
        }

        private static void SetupOdsInstanceSecretConfigurations(IEnumerable<OdsInstanceRegistration> testInstances)
        {
            var secretConfigurations = new List<SecretConfiguration>();
            foreach (var testInstance in testInstances)
            {
                secretConfigurations.Add(new SecretConfiguration
                {
                    OdsInstanceRegistrationId = testInstance.Id,
                    EncryptedData = Sample("EncryptedData")
                });
            }

            Save(secretConfigurations);
        }

        public static IEnumerable<UserOdsInstanceRegistration> SetupUserWithOdsInstanceRegistrations(string userId, List<OdsInstanceRegistration> instanceRegistrations)
        {
            var userOdsInstanceRegistrations = new List<UserOdsInstanceRegistration>();

            foreach (var instanceRegistration in instanceRegistrations)
            {
                userOdsInstanceRegistrations.Add(new UserOdsInstanceRegistration()
                {
                    OdsInstanceRegistrationId = instanceRegistration.Id,
                    UserId = userId
                });
            }

            Scoped<AdminAppIdentityDbContext>(database =>
            {
                database.UserOdsInstanceRegistrations.AddRange(userOdsInstanceRegistrations);
                database.SaveChanges();
            });

            return userOdsInstanceRegistrations;
        }
    }
}
