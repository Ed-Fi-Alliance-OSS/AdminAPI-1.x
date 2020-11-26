// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using log4net;

namespace EdFi.Ods.AdminApp.Management.Instances
{
    public class BulkRegisterOdsInstancesCommand
    {
        private readonly RegisterOdsInstanceCommand _registerOdsInstanceCommand;
        private readonly ILog _logger = LogManager.GetLogger("BulkRegisterOdsInstancesLog");

        public BulkRegisterOdsInstancesCommand(RegisterOdsInstanceCommand registerOdsInstanceCommand)
        {
            _registerOdsInstanceCommand = registerOdsInstanceCommand;
        }

        public async Task<IEnumerable<BulkRegisterOdsInstancesResult>> Execute(IEnumerable<IRegisterOdsInstanceModel> odsInstances, ApiMode mode, string userId, CloudOdsClaimSet cloudOdsClaimSet = null)
        {
            var results = new List<BulkRegisterOdsInstancesResult>();

            var odsInstancesRecords = odsInstances;
            var newOdsInstanceToRegister = _registerOdsInstanceCommand.PreExistingOdsInstanceRegistrations(odsInstancesRecords, mode);
            var skippedOdsInstances = odsInstancesRecords.Where(p => !newOdsInstanceToRegister.Any(p2 => p2.NumericSuffix == p.NumericSuffix));

            foreach (var skippedInstance in skippedOdsInstances)
            {
                results.Add(new BulkRegisterOdsInstancesResult
                {
                    NumericSuffix = skippedInstance.NumericSuffix.ToString(),
                    Description = skippedInstance.Description,
                    IndividualInstanceResult = IndividualInstanceResult.Skipped
                });
                _logger.Info($"Ods instance({skippedInstance.NumericSuffix.ToString()}) was skipped because it was previously registered.");
            }

            foreach (var instance in newOdsInstanceToRegister)
            {
                try
                {
                    await _registerOdsInstanceCommand.Execute(instance, mode, userId, cloudOdsClaimSet);
                    results.Add(new BulkRegisterOdsInstancesResult
                    {
                        NumericSuffix = instance.NumericSuffix.ToString(),
                        Description = instance.Description,
                        IndividualInstanceResult = IndividualInstanceResult.Succeded
                    });
                    _logger.Info($"Ods instance({instance.NumericSuffix.ToString()}) registered successfully.");
                }
                catch (Exception ex)
                {
                    results.Add(new BulkRegisterOdsInstancesResult
                    {
                        NumericSuffix = instance.NumericSuffix.ToString(),
                        Description = instance.Description,
                        IndividualInstanceResult = IndividualInstanceResult.Failed,
                        ErrorMessage = ex.Message
                    });
                    _logger.Error($"Ods instance({instance.NumericSuffix.ToString()}) registration failed. Error: {ex.Message}");
                }
            }

            return results;
        }
    }

    public enum IndividualInstanceResult
    {
        Succeded,
        Skipped,
        Failed
    }

    public class BulkRegisterOdsInstancesResult
    {
        public IndividualInstanceResult IndividualInstanceResult { get; set; }
        public string ErrorMessage { get; set; }
        public string NumericSuffix { get; set; }
        public string Description { get; set; }
    }
}
