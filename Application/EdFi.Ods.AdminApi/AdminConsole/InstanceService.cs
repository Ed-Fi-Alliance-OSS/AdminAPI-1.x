// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.Features.OdsInstanceContext;
using EdFi.Ods.AdminApi.Features.OdsInstanceDerivative;
using EdFi.Ods.AdminApi.Features.ODSInstances;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static EdFi.Ods.AdminApi.AdminConsole.Features.Instances.AddInstance;

namespace EdFi.Ods.AdminApi.AdminConsole;

public interface IAdminConsoleInstancesService
{
    Task InitializeIntancesAsync(int tenantId);
}
public class InstanceService : IAdminConsoleInstancesService
{
    private readonly IGetOdsInstancesQuery _getOdsInstancesQuery;
    private readonly IGetOdsInstanceContextsQuery _getOdsInstanceContextsQuery;
    private readonly IGetOdsInstanceDerivativesQuery _getOdsInstanceDerivativesQuery;
    private readonly IAddInstanceCommand _addInstanceCommand;
    private readonly IGetInstancesQuery _getInstancesQuery;
    private readonly IMapper _mapper;

    private static readonly ILog _log = LogManager.GetLogger(typeof(InstanceService));


    public InstanceService(IGetOdsInstancesQuery getOdsInstancesQuery,
        IGetOdsInstanceContextsQuery getOdsInstanceContextsQuery,
        IGetOdsInstanceDerivativesQuery getOdsInstanceDerivativesQuery,
        IAddInstanceCommand addInstanceCommand,
        IGetInstancesQuery getInstancesQuery,
        IMapper mapper)
    {
        _getOdsInstancesQuery = getOdsInstancesQuery;
        _getOdsInstanceContextsQuery = getOdsInstanceContextsQuery;
        _getOdsInstanceDerivativesQuery = getOdsInstanceDerivativesQuery;
        _addInstanceCommand = addInstanceCommand;
        _getInstancesQuery = getInstancesQuery;
        _mapper = mapper;
    }

    public async Task InitializeIntancesAsync(int tenantId)
    {
        //get instances in adminconsole
        var instancesAdminConsole = await _getInstancesQuery.Execute();
        //get odsinstances
        var odsInstances = _mapper.Map<List<OdsInstanceModel>>(_getOdsInstancesQuery.Execute());
        var odsInstanceContexts = _mapper.Map<List<OdsInstanceContextModel>>(_getOdsInstanceContextsQuery.Execute());
        var odsInstanceDerivatives = _mapper.Map<List<OdsInstanceDerivativeModel>>(_getOdsInstanceDerivativesQuery.Execute());
        foreach (var odsInstance in odsInstances)
        {
            //check if exist
            if (!instancesAdminConsole.Any(x => x.OdsInstanceId == odsInstance.OdsInstanceId))
            {
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
                addInstanceRequest.Document = JsonConvert.SerializeObject(document, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                await _addInstanceCommand.Execute(addInstanceRequest);
            }
        }
        _log.Info("Instances have been created in the AdminConsole tables");
    }
}


