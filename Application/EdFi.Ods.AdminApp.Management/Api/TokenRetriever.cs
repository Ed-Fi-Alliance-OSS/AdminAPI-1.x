// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using System.Security.Authentication;
using Newtonsoft.Json;
using RestSharp;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface ITokenRetriever
    {
        string ObtainNewBearerToken();
    }

    public class TokenRetriever : ITokenRetriever
    {
        private readonly OdsApiConnectionInformation _connectionInformation;

        /// <summary>
        /// </summary> 
        /// <param name="connectionInformation"></param>
        public TokenRetriever(OdsApiConnectionInformation connectionInformation)
        {
            _connectionInformation = connectionInformation;
        }

        public string ObtainNewBearerToken()
        {
            var oauthClient = new RestClient(_connectionInformation.OAuthUrl);

            try
            {
                return GetBearerToken(oauthClient);
            }
            catch (TokenRetrieverException)
            {
                throw;
            }
            catch (JsonException exception)
            {
                throw new TokenRetrieverException(
                    "Unexpected response format from API. Please verify the address ({0}) is configured correctly.",
                    exception.Message, exception, _connectionInformation.OAuthUrl);
            }
            catch (Exception exception)
            {
                throw new TokenRetrieverException("Unexpected error while connecting to API.", exception.Message, exception);
            }
        }

        private string GetBearerToken(IRestClient oauthClient)
        {
            var bearerTokenRequest = new RestRequest("oauth/token", Method.POST);
            bearerTokenRequest.AddParameter("Client_id", _connectionInformation.ClientKey);
            bearerTokenRequest.AddParameter("Client_secret", _connectionInformation.ClientSecret);
            bearerTokenRequest.AddParameter("Grant_type", "client_credentials");

            var bearerTokenResponse = oauthClient.Execute<BearerTokenResponse>(bearerTokenRequest);

            switch (bearerTokenResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case 0:
                    throw new TokenRetrieverException("Unable to connect to API. Please verify the API ({0}) is running.", bearerTokenResponse.ErrorMessage, null, _connectionInformation.ApiServerUrl);
                case HttpStatusCode.NotFound:
                    throw new TokenRetrieverException("Unable to connect to API: API not found. Please verify the address ({0}) is configured correctly.", bearerTokenResponse.ErrorMessage, null, _connectionInformation.ApiServerUrl);
                case HttpStatusCode.ServiceUnavailable:
                    throw new TokenRetrieverException("API Service is unavailable. Please verify the API ({0}) hosting configuration is correct.", bearerTokenResponse.ErrorMessage, null, _connectionInformation.ApiServerUrl);
                default:
                    throw new TokenRetrieverException("Unexpected response from API.", bearerTokenResponse.ErrorMessage, null);
            }

            if (bearerTokenResponse.Data.Error != null || bearerTokenResponse.Data.TokenType != "bearer")
            {
                throw new TokenRetrieverException("Please verify that your application secret is correct.", "", null);
            }

            return bearerTokenResponse.Data.AccessToken;
        }

        private class TokenRetrieverException : AuthenticationException
        {
            public TokenRetrieverException() { }

            public TokenRetrieverException(string helpText, string error, Exception innerException, params object[] formatArgs)
            : base(string.Format("Unable to retrieve an access token. " + helpText + " Error message: " + error, formatArgs)
            , innerException)
            { }
        }
    }

    internal class BearerTokenResponse
    {
        public string AccessToken { get; set; }
        public string ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string Error { get; set; }
    }
}
