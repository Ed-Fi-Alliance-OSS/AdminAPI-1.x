// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using AutoMapper;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public class OdsApiFacadeFactory : IOdsApiFacadeFactory
    {
        private readonly IMapper _mapper;
        private readonly IOdsRestClientFactory _restClientFactory;

        public OdsApiFacadeFactory(IMapper mapper, IOdsRestClientFactory restClientFactory)
        {
            _mapper = mapper;
            _restClientFactory = restClientFactory;
        }

        public async Task<IOdsApiFacade> Create()
        {
            var restClient = await _restClientFactory.Create();
            return new OdsApiFacade(_mapper, restClient);
        }
    }
}
