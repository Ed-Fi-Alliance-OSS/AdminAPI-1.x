// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net.Http;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public interface ISimpleGetRequest
    {
        public Task<string> DownloadString(string address);
    }

    public class SimpleGetRequest : ISimpleGetRequest
    {
        private readonly IHttpClientFactory _clientFactory;

        public SimpleGetRequest(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> DownloadString(string address)
        {
            var httpClient = _clientFactory.CreateClient();

            using (var response = await httpClient.GetAsync(address))
            {
                using (var content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }
    }
}
