// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.Features.OdsInstanceContext;
using EdFi.Ods.AdminApi.Features.OdsInstanceDerivative;
using EdFi.Ods.AdminApi.Features.ODSInstances;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static EdFi.Ods.AdminApi.AdminConsole.Features.Instances.AddInstance;

namespace EdFi.Ods.AdminApi.AdminConsole;

public interface IAdminConsoleInstancesService
{
    Task InitializeInstancesAsync(int tenantId, int applicationId);
}
public class InstanceService : IAdminConsoleInstancesService
{
    private readonly IGetOdsInstancesQuery _getOdsInstancesQuery;
    private readonly IGetOdsInstanceContextsQuery _getOdsInstanceContextsQuery;
    private readonly IGetOdsInstanceDerivativesQuery _getOdsInstanceDerivativesQuery;
    private readonly IAddInstanceCommand _addInstanceCommand;
    private readonly IAddApiClientOdsInstanceCommand _addApiClientOdsInstanceCommand;
    private readonly IGetInstancesQuery _getInstancesQuery;
    private readonly IGetApiClientIdByApplicationIdQuery _getApiClientIdByApplicationIdQuery;
    private readonly IGetApiClientOdsInstanceQuery _getApiClientOdsInstanceQuery;
    private readonly IMapper _mapper;

    private static readonly ILog _log = LogManager.GetLogger(typeof(InstanceService));


    public InstanceService(IGetOdsInstancesQuery getOdsInstancesQuery,
        IGetOdsInstanceContextsQuery getOdsInstanceContextsQuery,
        IGetOdsInstanceDerivativesQuery getOdsInstanceDerivativesQuery,
        IAddInstanceCommand addInstanceCommand,
        IAddApiClientOdsInstanceCommand addApiClientOdsInstanceCommand,
        IGetInstancesQuery getInstancesQuery,
        IGetApiClientIdByApplicationIdQuery getApiClientIdByApplicationIdQuery,
        IGetApiClientOdsInstanceQuery getApiClientOdsInstanceQuery,
        IMapper mapper)
    {
        _getOdsInstancesQuery = getOdsInstancesQuery;
        _getOdsInstanceContextsQuery = getOdsInstanceContextsQuery;
        _getOdsInstanceDerivativesQuery = getOdsInstanceDerivativesQuery;
        _addInstanceCommand = addInstanceCommand;
        _getInstancesQuery = getInstancesQuery;
        _getApiClientIdByApplicationIdQuery = getApiClientIdByApplicationIdQuery;
        _getApiClientOdsInstanceQuery = getApiClientOdsInstanceQuery;
        _addApiClientOdsInstanceCommand = addApiClientOdsInstanceCommand;
        _mapper = mapper;
    }

    public async Task InitializeInstancesAsync(int tenantId, int applicationId)
    {
        //get instances in adminconsole
        var instancesAdminConsole = await _getInstancesQuery.Execute();
        var apiClient = _mapper.Map<ApiClient>(_getApiClientIdByApplicationIdQuery.Execute(applicationId));
        //get odsinstances
        var odsInstancesList = _getOdsInstancesQuery.Execute();
        var odsInstances = _mapper.Map<List<OdsInstanceModel>>(odsInstancesList);
        var odsInstanceContexts = _mapper.Map<List<OdsInstanceContextModel>>(_getOdsInstanceContextsQuery.Execute());
        var odsInstanceDerivatives = _mapper.Map<List<OdsInstanceDerivativeModel>>(_getOdsInstanceDerivativesQuery.Execute());
        foreach (var odsInstance in odsInstances)
        {
            //check if exist
            if (!instancesAdminConsole.Any(x => x.OdsInstanceId == odsInstance.OdsInstanceId))
            {
                var apiClientOdsInstance = _getApiClientOdsInstanceQuery.Execute(apiClient.ApiClientId, odsInstance.OdsInstanceId);
                if (apiClientOdsInstance == null)
                {
                    var odsInstanceValue = odsInstancesList.SingleOrDefault(o => o.OdsInstanceId == odsInstance.OdsInstanceId);
                    ApiClientOdsInstance newApiClientOdsInstance = new ApiClientOdsInstance
                    {
                        ApiClient = apiClient,
                        OdsInstance = odsInstanceValue
                    };
                    apiClientOdsInstance = _addApiClientOdsInstanceCommand.Execute(newApiClientOdsInstance);
                }
                //create
                AddInstanceRequest addInstanceRequest = new AddInstanceRequest();
                addInstanceRequest.OdsInstanceId = odsInstance.OdsInstanceId;
                addInstanceRequest.TenantId = tenantId;
                var odsContexts = odsInstanceContexts.Where(x => x.OdsInstanceId == odsInstance.OdsInstanceId);
                var odsDerivatives = odsInstanceDerivatives.Where(x => x.OdsInstanceId == odsInstance.OdsInstanceId);
                dynamic document = new ExpandoObject();
                document.name = odsInstance.Name;
                document.instanceType = odsInstance.InstanceType;
                document.odsInstanceContexts = odsContexts;
                document.odsInstanceDerivatives = odsDerivatives;
                document.apiClientOdsInstance = new
                {
                    ApiClientOdsInstanceId = apiClientOdsInstance.ApiClientOdsInstanceId,
                    ApiClient_ApiClientId = apiClientOdsInstance.ApiClient.ApiClientId,
                    OdsInstance_OdsInstanceId = apiClientOdsInstance.OdsInstance.OdsInstanceId
                };
                addInstanceRequest.Document = document;
                dynamic apiCredentials = new ExpandoObject();
                apiCredentials.ClientId = apiClient.Key;
                apiCredentials.Secret = apiClient.Secret;
                addInstanceRequest.ApiCredentials = apiCredentials;
                await _addInstanceCommand.Execute(addInstanceRequest);

            }
        }
        _log.Info("Instances have been created in the AdminConsole tables");
    }
}


