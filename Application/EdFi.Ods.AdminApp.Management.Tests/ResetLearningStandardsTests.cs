// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public class ResetLearningStandardsTests
    {
        private FakeOdsSecretConfigurationProvider _secretConfigProvider;
        private InstanceContext _instanceContext;

        [SetUp]
        public async Task Setup()
        {
            _secretConfigProvider = new FakeOdsSecretConfigurationProvider();
            _instanceContext = new InstanceContext{Id = 1, Name = "Test Instance" };

            await _secretConfigProvider.SetSecretConfiguration(new OdsSecretConfiguration
            {
                LearningStandardsCredential = new ABProxyApiCredential
                {
                    ApiKey = "ApiKey",
                    ApiSecret = "ApiSecret",
                    SynchronizationWasSuccessful = false
                }
            });
        }

        [Test]
        public async Task ShouldResetAcademicBenchmarkCredentials()
        {
            var service = new ResetLearningStandards(_secretConfigProvider, _instanceContext);

            await service.Execute();

            var secretConfig = await _secretConfigProvider.GetSecretConfiguration();

            secretConfig.LearningStandardsCredential.ApiSecret.ShouldBeEmpty();
            secretConfig.LearningStandardsCredential.ApiKey.ShouldBeEmpty();
        }

        [Test]
        public async Task ShouldResetSuccessfulSyncFlag()
        {
            var service = new ResetLearningStandards(_secretConfigProvider, _instanceContext);

            await service.Execute();

            var secretConfig = await _secretConfigProvider.GetSecretConfiguration();
            
            secretConfig.LearningStandardsCredential.SynchronizationWasSuccessful.ShouldBe(false);
        }

        [Test]
        public async Task ShouldNotResetDateOfLastSync()
        {
            var yesterday = DateTime.Today.AddDays(-1);

            await SetLearningStandardsSyncDate(yesterday);

            var service = new ResetLearningStandards(_secretConfigProvider, _instanceContext);

            await service.Execute();

            var secretConfig = await _secretConfigProvider.GetSecretConfiguration();

            secretConfig.LearningStandardsCredential.LastUpdatedDate.ShouldBe(yesterday);
        }

        private async Task SetLearningStandardsSyncDate(DateTime dateTime)
        {
            var newConfig = await _secretConfigProvider.GetSecretConfiguration();

            newConfig.LearningStandardsCredential.LastUpdatedDate = dateTime;

            await _secretConfigProvider.SetSecretConfiguration(newConfig);
        }
    }
}
