// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.TestingHelper;

namespace EdFi.Ods.AdminApp.Management.Tests.Configuration.Configuration
{
    public class OdsSecretConfigurationProviderTests : AdminAppDataTestBase
    {
        private static async Task<OdsSecretConfiguration> GetSecretConfiguration(int? instanceRegistrationId = null)
        {
            return await ScopedAsync<IOdsSecretConfigurationProvider, OdsSecretConfiguration>(
                async provider => await provider.GetSecretConfiguration(instanceRegistrationId));
        }

        private static async Task SetSecretConfiguration(OdsSecretConfiguration configuration, int? instanceRegistrationId = null)
        {
            await ScopedAsync<IOdsSecretConfigurationProvider>(
                async provider => await provider.SetSecretConfiguration(configuration, instanceRegistrationId));
        }

        private readonly ObjectCache _cache = MemoryCache.Default;

        [Test]
        public async Task ShouldReturnNullWhenThereAreZeroSecretConfigurations()
        {
            ClearSecretConfigurationCache();
            EnsureZeroSecretConfigurations();

            var secretConfiguration = await GetSecretConfiguration();

            secretConfiguration.ShouldBe(null);
        }

        [Test]
        public async Task ShouldRetrieveUnencryptedSecretConfiguration_SingleInstance()
        {
            const string JsonConfiguration =
                "{\"ProductionApiKeyAndSecret\":{\"Key\":\"productionKey\",\"Secret\":\"productionSecret\"}," +
                "\"BulkUploadCredential\":{\"ApiKey\":\"bulkKey\",\"ApiSecret\":\"bulkSecret\"}," +
                "\"LearningStandardsCredential\":null," +
                "\"ProductionAcademicBenchmarkApiClientKeyAndSecret\":null}";


            ClearSecretConfigurationCache();

            var odsInstanceRegistration = CreateOdsInstanceRegistration("SingleInstance_" + Guid.NewGuid());

            odsInstanceRegistration.ShouldNotBeNull();

            AddSecretConfiguration(JsonConfiguration, odsInstanceRegistration.Id);

            var secretConfiguration = await GetSecretConfiguration(odsInstanceRegistration.Id);

            var record = GetSecretConfigurationRecord(odsInstanceRegistration.Id);
            record.IsEncrypted.ShouldBe(false);
            record.EncryptedData.ShouldBe(JsonConfiguration);

            secretConfiguration.ProductionApiKeyAndSecret.Key.ShouldBe("productionKey");
            secretConfiguration.ProductionApiKeyAndSecret.Secret.ShouldBe("productionSecret");
            secretConfiguration.BulkUploadCredential.ApiKey.ShouldBe("bulkKey");
            secretConfiguration.BulkUploadCredential.ApiSecret.ShouldBe("bulkSecret");
            secretConfiguration.LearningStandardsCredential.ShouldBe(null);
            secretConfiguration.ProductionAcademicBenchmarkApiClientKeyAndSecret.ShouldBe(null);
        }

        [Test]
        public async Task ShouldRetrieveUnencryptedSecretConfiguration_MultiInstance()
        {
            const string JsonConfiguration1 =
                "{\"ProductionApiKeyAndSecret\":{\"Key\":\"productionKey1\",\"Secret\":\"productionSecret1\"}," +
                "\"BulkUploadCredential\":{\"ApiKey\":\"bulkKey1\",\"ApiSecret\":\"bulkSecret1\"}," +
                "\"LearningStandardsCredential\":null," +
                "\"ProductionAcademicBenchmarkApiClientKeyAndSecret\":null}";
            const string JsonConfiguration2 =
                "{\"ProductionApiKeyAndSecret\":{\"Key\":\"productionKey2\",\"Secret\":\"productionSecret2\"}," +
                "\"BulkUploadCredential\":{\"ApiKey\":\"bulkKey2\",\"ApiSecret\":\"bulkSecret2\"}," +
                "\"LearningStandardsCredential\":null," +
                "\"ProductionAcademicBenchmarkApiClientKeyAndSecret\":null}";

            var odsInstanceRegistration1 = CreateOdsInstanceRegistration("MultiInstance1_" + Guid.NewGuid());
            var odsInstanceRegistration2 = CreateOdsInstanceRegistration("MultiInstance2_" + Guid.NewGuid());

            ClearSecretConfigurationCache();

            odsInstanceRegistration1.ShouldNotBeNull();
            odsInstanceRegistration2.ShouldNotBeNull();

            AddSecretConfiguration(JsonConfiguration1, odsInstanceRegistration1.Id);

            var secretConfigurationForInstance1 =
                await GetSecretConfiguration(odsInstanceRegistration1.Id);

            var record = GetSecretConfigurationRecord(odsInstanceRegistration1.Id);
            record.IsEncrypted.ShouldBe(false);
            record.EncryptedData.ShouldBe(JsonConfiguration1);

            secretConfigurationForInstance1.ProductionApiKeyAndSecret.Key.ShouldBe("productionKey1");
            secretConfigurationForInstance1.ProductionApiKeyAndSecret.Secret.ShouldBe("productionSecret1");
            secretConfigurationForInstance1.BulkUploadCredential.ApiKey.ShouldBe("bulkKey1");
            secretConfigurationForInstance1.BulkUploadCredential.ApiSecret.ShouldBe("bulkSecret1");
            secretConfigurationForInstance1.LearningStandardsCredential.ShouldBe(null);
            secretConfigurationForInstance1.ProductionAcademicBenchmarkApiClientKeyAndSecret.ShouldBe(null);

            AddSecretConfiguration(JsonConfiguration2, odsInstanceRegistration2.Id);
            var secretConfigurationForInstance2 =
                await GetSecretConfiguration(odsInstanceRegistration2.Id);

            record = GetSecretConfigurationRecord(odsInstanceRegistration2.Id);
            record.IsEncrypted.ShouldBe(false);
            record.EncryptedData.ShouldBe(JsonConfiguration2);

            secretConfigurationForInstance2.ProductionApiKeyAndSecret.Key.ShouldBe("productionKey2");
            secretConfigurationForInstance2.ProductionApiKeyAndSecret.Secret.ShouldBe("productionSecret2");
            secretConfigurationForInstance2.BulkUploadCredential.ApiKey.ShouldBe("bulkKey2");
            secretConfigurationForInstance2.BulkUploadCredential.ApiSecret.ShouldBe("bulkSecret2");
            secretConfigurationForInstance2.LearningStandardsCredential.ShouldBe(null);
            secretConfigurationForInstance2.ProductionAcademicBenchmarkApiClientKeyAndSecret.ShouldBe(null);
        }

        [Test]
        public async Task ShouldEditSecretConfiguration_SingleInstance()
        {
            ClearSecretConfigurationCache();

            var productionApiKey = Guid.NewGuid().ToString();
            var productionApiSecret = Guid.NewGuid().ToString();

            var newProductionApiKey = Guid.NewGuid().ToString();
            var newProductionApiSecret = Guid.NewGuid().ToString();

            var secretConfiguration = new OdsSecretConfiguration
            {
                ProductionApiKeyAndSecret = new OdsApiCredential
                {
                    Key = productionApiKey,
                    Secret = productionApiSecret
                }
            };

            var editSecretConfiguration = new OdsSecretConfiguration
            {
                ProductionApiKeyAndSecret = new OdsApiCredential
                {
                    Key = newProductionApiKey,
                    Secret = newProductionApiSecret
                }
            };

            var odsInstanceRegistration = CreateOdsInstanceRegistration("SingleInstance-EditConfig_" + Guid.NewGuid());

            odsInstanceRegistration.ShouldNotBeNull();
            
            await SetSecretConfiguration(secretConfiguration, odsInstanceRegistration.Id);
            var createdSecretConfiguration = await GetSecretConfiguration(odsInstanceRegistration.Id);

            var record = GetSecretConfigurationRecord(odsInstanceRegistration.Id);
            record.IsEncrypted.ShouldBe(true);
            record.EncryptedData.ShouldEndWith("=");

            createdSecretConfiguration.ProductionApiKeyAndSecret.Key.ShouldBe(productionApiKey);
            createdSecretConfiguration.ProductionApiKeyAndSecret.Secret.ShouldBe(productionApiSecret);
            createdSecretConfiguration.BulkUploadCredential.ShouldBe(null);
            createdSecretConfiguration.LearningStandardsCredential.ShouldBe(null);
            createdSecretConfiguration.ProductionAcademicBenchmarkApiClientKeyAndSecret.ShouldBe(null);

            await SetSecretConfiguration(editSecretConfiguration, odsInstanceRegistration.Id);

            var editedSecretConfiguration = await GetSecretConfiguration(odsInstanceRegistration.Id);

            record = GetSecretConfigurationRecord(odsInstanceRegistration.Id);
            record.IsEncrypted.ShouldBe(true);
            record.EncryptedData.ShouldEndWith("=");

            editedSecretConfiguration.ProductionApiKeyAndSecret.Key.ShouldBe(newProductionApiKey);
            editedSecretConfiguration.ProductionApiKeyAndSecret.Secret.ShouldBe(newProductionApiSecret);
            editedSecretConfiguration.BulkUploadCredential.ShouldBe(null);
            editedSecretConfiguration.LearningStandardsCredential.ShouldBe(null);
            editedSecretConfiguration.ProductionAcademicBenchmarkApiClientKeyAndSecret.ShouldBe(null);

        }

        [Test]
        public async Task ShouldEditSecretConfiguration_MultiInstance()
        {
            ClearSecretConfigurationCache();

            var productionApiKey = Guid.NewGuid().ToString();
            var productionApiSecret = Guid.NewGuid().ToString();

            var newProductionApiKey = Guid.NewGuid().ToString();
            var newProductionApiSecret = Guid.NewGuid().ToString();

            var secretConfiguration = new OdsSecretConfiguration
            {
                ProductionApiKeyAndSecret = new OdsApiCredential
                {
                    Key = productionApiKey,
                    Secret = productionApiSecret
                }
            };

            var editSecretConfiguration = new OdsSecretConfiguration
            {
                ProductionApiKeyAndSecret = new OdsApiCredential
                {
                    Key = newProductionApiKey,
                    Secret = newProductionApiSecret
                }
            };

            var odsInstanceRegistration1 = CreateOdsInstanceRegistration("MultiInstance1-EditConfig_" + Guid.NewGuid());
            var odsInstanceRegistration2 = CreateOdsInstanceRegistration("MultiInstance2-EditConfig_" + Guid.NewGuid());

            odsInstanceRegistration1.ShouldNotBeNull();
            odsInstanceRegistration2.ShouldNotBeNull();

            // Set secret configuration for instance 1
            await SetSecretConfiguration(secretConfiguration, odsInstanceRegistration1.Id);

            // Set secret configuration for instance 2
            await SetSecretConfiguration(secretConfiguration, odsInstanceRegistration2.Id);

            // Edit the secret configuration for instance 1
            await SetSecretConfiguration(editSecretConfiguration, odsInstanceRegistration1.Id);

            // Verify the secret configuration for instance 2
            var createdSecretConfigurationForInstance2 = await GetSecretConfiguration(odsInstanceRegistration2.Id);
            var record = GetSecretConfigurationRecord(odsInstanceRegistration2.Id);
            record.IsEncrypted.ShouldBe(true);
            record.EncryptedData.ShouldEndWith("=");
            createdSecretConfigurationForInstance2.ProductionApiKeyAndSecret.Key.ShouldBe(productionApiKey);
            createdSecretConfigurationForInstance2.ProductionApiKeyAndSecret.Secret.ShouldBe(productionApiSecret);
            createdSecretConfigurationForInstance2.BulkUploadCredential.ShouldBe(null);
            createdSecretConfigurationForInstance2.LearningStandardsCredential.ShouldBe(null);
            createdSecretConfigurationForInstance2.ProductionAcademicBenchmarkApiClientKeyAndSecret.ShouldBe(null);

            // Verify the edited secret configuration for instance 1
            var editedSecretConfigurationForInstance1 = await GetSecretConfiguration(odsInstanceRegistration1.Id);
            record = GetSecretConfigurationRecord(odsInstanceRegistration1.Id);
            record.IsEncrypted.ShouldBe(true);
            record.EncryptedData.ShouldEndWith("=");
            editedSecretConfigurationForInstance1.ProductionApiKeyAndSecret.Key.ShouldBe(newProductionApiKey);
            editedSecretConfigurationForInstance1.ProductionApiKeyAndSecret.Secret.ShouldBe(newProductionApiSecret);
            editedSecretConfigurationForInstance1.BulkUploadCredential.ShouldBe(null);
            editedSecretConfigurationForInstance1.LearningStandardsCredential.ShouldBe(null);
            editedSecretConfigurationForInstance1.ProductionAcademicBenchmarkApiClientKeyAndSecret.ShouldBe(null);
        }

        private void EnsureZeroSecretConfigurations()
        {
            Scoped<AdminAppDbContext>(database =>
            {
                foreach (var entity in database.SecretConfigurations)
                    database.SecretConfigurations.Remove(entity);

                database.SaveChanges();
            });
        }

        private void AddSecretConfiguration(string jsonConfigurationPlainText, int odsInstanceRegistrationId)
        {
            Scoped<AdminAppDbContext>(database =>
            {
                database.SecretConfigurations.Add(
                    new SecretConfiguration
                    {
                        EncryptedData = jsonConfigurationPlainText,
                        OdsInstanceRegistrationId = odsInstanceRegistrationId,
                        IsEncrypted = false
                    });

                database.SaveChanges();
            });
        }

        private OdsInstanceRegistration CreateOdsInstanceRegistration(string instanceName)
        {
            OdsInstanceRegistration createdOdsInstanceRegistration = null;
            Scoped<AdminAppDbContext>(database =>
            {
                database.OdsInstanceRegistrations.Add(new OdsInstanceRegistration {Name = instanceName, DatabaseName = instanceName});
                database.SaveChanges();

                createdOdsInstanceRegistration =
                    database.OdsInstanceRegistrations.FirstOrDefault(x => x.Name == instanceName);
            });

            return createdOdsInstanceRegistration;
        }

        private void ClearSecretConfigurationCache()
        {
            var cacheKeys = _cache.Select(k => k.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
        }

        private static SecretConfiguration GetSecretConfigurationRecord(int odsInstanceRegistrationId)
            => Query(db => db.SecretConfigurations.Single(x => x.OdsInstanceRegistrationId == odsInstanceRegistrationId));
    }
}
