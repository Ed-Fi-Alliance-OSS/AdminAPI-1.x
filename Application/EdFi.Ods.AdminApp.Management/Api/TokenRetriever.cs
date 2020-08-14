// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using System.Security.Authentication;
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
            return GetBearerToken(oauthClient);
        }

        private string GetBearerToken(IRestClient oauthClient)
        {
            var bearerTokenRequest = new RestRequest("oauth/token", Method.POST);
            bearerTokenRequest.AddParameter("Client_id", _connectionInformation.ClientKey);
            bearerTokenRequest.AddParameter("Client_secret", _connectionInformation.ClientSecret);
            bearerTokenRequest.AddParameter("Grant_type", "client_credentials");

            var bearerTokenResponse = oauthClient.Execute<BearerTokenResponse>(bearerTokenRequest);
            if (bearerTokenResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new AuthenticationException("Unable to retrieve an access token. Error message: " +
                                                  bearerTokenResponse.ErrorMessage);
            }

            if (bearerTokenResponse.Data.Error != null || bearerTokenResponse.Data.TokenType != "bearer")
            {
                throw new AuthenticationException(
                    "Unable to retrieve an access token. Please verify that your application secret is correct.");
            }

            return bearerTokenResponse.Data.AccessToken;
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