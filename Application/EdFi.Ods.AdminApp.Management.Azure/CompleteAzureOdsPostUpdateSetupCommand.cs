// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class CompleteAzureOdsPostUpdateSetupCommand : ICompleteOdsPostUpdateSetupCommand
    {
        private readonly IGetCloudOdsInstanceQuery _getCloudOdsInstanceQuery;
        private readonly IAssessmentVendorAdjustment _assessmentVendorAdjustment;
        private readonly ILearningStandardsSetup _learningStandardsSetup;
        private readonly IRestartAppServicesCommand _restartAppServicesCommand;
        private readonly IUsersContext _usersContext;

        public CompleteAzureOdsPostUpdateSetupCommand(
            IGetCloudOdsInstanceQuery getCloudOdsInstanceQuery,
            IAssessmentVendorAdjustment assessmentVendorAdjustment,
            ILearningStandardsSetup learningStandardsSetup,
            IRestartAppServicesCommand restartAppServicesCommand, 
            IUsersContext usersContext)
        {
            _getCloudOdsInstanceQuery = getCloudOdsInstanceQuery;
            _assessmentVendorAdjustment = assessmentVendorAdjustment;
            _learningStandardsSetup = learningStandardsSetup;
            _restartAppServicesCommand = restartAppServicesCommand;
            _usersContext = usersContext;
        }

        public async Task Execute(string odsInstanceName)
        {
            var defaultInstance = await _getCloudOdsInstanceQuery.Execute(odsInstanceName);

            var configuration = new OdsPostUpdateSetupConfiguration
            {
                Instance = defaultInstance
            };

            //in the Azure Cloud ODS implementation we don't actually have access to the SQL Server
            //so this command stamps the correct version in the Admin database, based on the version
            //number that was set in the Azure Resource Group tags during the upgrade operation

            var instance = await _usersContext.OdsInstances.Include(i => i.OdsInstanceComponents).SingleOrDefaultAsync(i => i.Name == configuration.Instance.FriendlyName);
            instance.Version = configuration.Instance.Version;

            foreach (var component in instance.OdsInstanceComponents)
            {
                component.Version = configuration.Instance.Version;
            }

            _learningStandardsSetup.SetupLearningStandardsClaims();

            _assessmentVendorAdjustment.ReadAndCreatePerformanceLevelDescriptors();

            await _usersContext.SaveChangesAsync();

            await _restartAppServicesCommand.Execute(new CloudOdsApiOperationContext(defaultInstance));
        }
    }
}
