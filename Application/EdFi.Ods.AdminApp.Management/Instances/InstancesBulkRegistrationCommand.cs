// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using log4net;

namespace EdFi.Ods.AdminApp.Management.Instances
{
    public class BulkRegisterOdsInstancesCommand
    {
        private readonly RegisterOdsInstanceCommand _registerOdsInstanceCommand;
        private readonly ILog _logger = LogManager.GetLogger(typeof(BulkRegisterOdsInstancesCommand));

        public BulkRegisterOdsInstancesCommand(RegisterOdsInstanceCommand registerOdsInstanceCommand)
        {
            _registerOdsInstanceCommand = registerOdsInstanceCommand;
        }

        public async Task<IEnumerable<BulkRegisterOdsInstancesResult>> Execute(IEnumerable<IRegisterOdsInstanceModel> odsInstances, ApiMode mode, string userId, CloudOdsClaimSet cloudOdsClaimSet = null)
        {
            var results = new List<BulkRegisterOdsInstancesResult>();
            foreach (var instance in odsInstances)
            {
                try
                {
                    await _registerOdsInstanceCommand.Execute(instance, mode, userId, cloudOdsClaimSet);
                    results.Add(new BulkRegisterOdsInstancesResult
                    {
                        NumericSuffix = instance.NumericSuffix.ToString(),
                        Description = instance.Description,
                        Success = true
                    });
                    _logger.Info($"Ods instance({instance.NumericSuffix.ToString()}) registered successfully.");
                }
                catch (Exception ex)
                {
                    results.Add(new BulkRegisterOdsInstancesResult
                    {
                        NumericSuffix = instance.NumericSuffix.ToString(),
                        Description = instance.Description,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                    _logger.Error($"Ods instance({instance.NumericSuffix.ToString()}) registration failed. Error: {ex.Message}");
                }
            }

            return results;
        }
    }
    public interface IBulkRegisterOdsInstancesModel
    {
        HttpPostedFileBase OdsInstancesFile { get; }
    }

    public class BulkRegisterOdsInstancesResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string NumericSuffix { get; set; }
        public string Description { get; set; }
    }
}
