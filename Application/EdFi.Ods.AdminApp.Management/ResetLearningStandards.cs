// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management
{
    public class ResetLearningStandards : IResetLearningStandards
    {
        private readonly IOdsSecretConfigurationProvider _secretConfiguration;
        private readonly InstanceContext _instanceContext;

        public ResetLearningStandards(IOdsSecretConfigurationProvider secretConfiguration, InstanceContext instanceContext)
        {
            _secretConfiguration = secretConfiguration;
            _instanceContext = instanceContext;
        }

        public async Task Execute()
        {
            var learningStandardsCredential = (await _secretConfiguration.GetSecretConfiguration(_instanceContext.Id)).LearningStandardsCredential;

            if (learningStandardsCredential == null)
            {
                return;
            }

            learningStandardsCredential.ApiKey = string.Empty;
            learningStandardsCredential.ApiSecret = string.Empty;
            learningStandardsCredential.SynchronizationWasSuccessful = false;
        }
    }
}