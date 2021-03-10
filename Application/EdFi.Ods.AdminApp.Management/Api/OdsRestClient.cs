// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public class OdsRestClient : IOdsRestClient
    {
        private readonly OdsApiConnectionInformation _connectionInformation;
        private readonly ITokenRetriever _tokenRetriever;
        private string _bearerToken;
        private readonly IRestClient _restClient;
        private static ILog _logger;

        public OdsRestClient(OdsApiConnectionInformation connectionInformation, IRestClient restClient, ITokenRetriever tokenRetriever)
        {
            _connectionInformation = connectionInformation;
            _tokenRetriever = tokenRetriever;
            _restClient = restClient;
            _logger = LogManager.GetLogger(typeof(OdsRestClient));
        }

        private string GetAuthHeaderValue(bool refreshToken = false)
        {
            if (_bearerToken == null || refreshToken)
            {
                _bearerToken = _tokenRetriever.ObtainNewBearerToken();
            }

            return "Bearer " + _bearerToken;
        }

        private RestRequest OdsRequest(string resource)
        {
            var request = new RestRequest(resource) {RequestFormat = DataFormat.Json};
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", GetAuthHeaderValue());
            return request;
        }

        private static void HandleErrorResponse(IRestResponse response)
        {
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                _logger.Debug("*** Status:");
                _logger.Debug(response.StatusCode);

                _logger.Debug("*** Content:");
                _logger.Debug(response.Content);

                _logger.Debug("*** ErrorException:");
                _logger.Debug(response.ErrorException);

                throw new Exception(response.ErrorMessage);
            }
        }

        public IReadOnlyList<T> GetAll<T>(string elementPath, int offset, int limit = 50) where T : class
        {
            var request = OdsRequest(elementPath);
            request.AddParameter("offset", offset);
            request.AddParameter("limit", limit);

            var responseList = new List<T>();
            List<T> pageItems;

            var restResponse = _restClient.Execute(request);
            HandleErrorResponse(restResponse);

            pageItems = JsonConvert.DeserializeObject<List<T>>(restResponse.Content);
            responseList.AddRange(pageItems);

            return responseList; 
        }

        public IReadOnlyList<T> GetAll<T>(string elementPath) where T : class
        {
            var offset = 0;
            const int limit = 50;

            var request = OdsRequest(elementPath);
            request.AddParameter("offset", offset);
            request.AddParameter("limit", limit);

            var responseList = new List<T>();
            List<T> pageItems;

            do
            {
                var restResponse = _restClient.Execute(request);
                HandleErrorResponse(restResponse);

                pageItems = JsonConvert.DeserializeObject<List<T>>(restResponse.Content);
                responseList.AddRange(pageItems);

                offset += limit;
                request.Parameters.Single(x => x.Name == "offset").Value = offset;
            }
            while (pageItems.Count >= limit);

            return responseList; 
        }

        public T GetById<T>(string elementPath, string id) where T : class
        {
            var request = OdsRequest(elementPath);
            request.AddUrlSegment("id", id);
            var response = _restClient.Execute(request);
            HandleErrorResponse(response);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public OdsApiResult PostResource<T>(T resource, string elementPath, bool refreshToken = false)
        {
            var result = new OdsApiResult();
            try
            {
                var request = OdsRequest(elementPath);
                request.Method = Method.POST;
                var jsonInput = JsonConvert.SerializeObject(resource);
                request.AddParameter("application/json; charset=utf-8", jsonInput, ParameterType.RequestBody);
                var response = _restClient.Execute(request);
                if (response != null && response.StatusCode != HttpStatusCode.Created &&
                                        response.StatusCode != HttpStatusCode.OK)
                {
                    result.ErrorMessage = response.ErrorMessage ?? $"ODS/API returned status code '{response.StatusCode}'";
                }

                return result;
            }
            catch (Exception ex)
            {
                return new OdsApiResult
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public OdsApiResult PutResource<T>(T resource, string elementPath, string id, bool refreshToken = false)
        {
            try
            {
                var result = new OdsApiResult();
                var request = OdsRequest($"{elementPath}/{id}");
                request.Method = Method.PUT;
                var jsonInput = JsonConvert.SerializeObject(resource);
                request.AddParameter("application/json; charset=utf-8", jsonInput, ParameterType.RequestBody);
                var response = _restClient.Execute(request);
                if (response != null && (!response.StatusCode.Equals(HttpStatusCode.Created) ||
                                         !response.StatusCode.Equals(HttpStatusCode.NoContent)))
                {
                    result.ErrorMessage = response.ErrorMessage;
                }

                return result;
            }
            catch (Exception ex)
            {
                return new OdsApiResult
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public IReadOnlyList<string> GetAllDescriptors()
        {
            _restClient.BaseUrl = new Uri(_connectionInformation.DescriptorsUrl);
            var request = OdsRequest("swagger.json");
            var response = _restClient.Execute(request);
            HandleErrorResponse(response);
            var swaggerDocument = JsonConvert.DeserializeObject<JObject>(response.Content);
            var descriptorPaths = swaggerDocument["paths"].ToObject<Dictionary<string, JObject>>();

            var descriptorsList = new SortedSet<string>();

            if (descriptorPaths != null)
            {
                foreach (var descriptorPath in descriptorPaths.Keys)
                {
                    //Paths take the form /extension/name, /extension/name/{id}, /extension/name/deletes, etc.
                    //Here we extract distinct /extension/name.
                    var resourceParts = descriptorPath.TrimStart('/').Split('/').Take(2).ToArray();

                    if (resourceParts.Length >= 2)
                        descriptorsList.Add($"/{resourceParts[0]}/{resourceParts[1]}");
                }
            }

            return descriptorsList.ToList();
        }

        public OdsApiResult DeleteResource(string elementPath, string id, bool refreshToken = false)
        {
            try
            {
                var request = OdsRequest(elementPath);
                request.Method = Method.DELETE;
                request.AddUrlSegment("id", id);
                _restClient.Execute(request);
                return new OdsApiResult();
            }
            catch (Exception ex)
            {
                return new OdsApiResult
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
