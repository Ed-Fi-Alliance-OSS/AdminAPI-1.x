// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Security.Authentication;
using System.Security.Claims;
using EdFi.Ods.Admin.Api.Infrastructure.Security;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenIddict.Abstractions;

namespace EdFi.Ods.Admin.Api.Features.Connect;

public interface ITokenService
{
    Task<ClaimsPrincipal> Handle(OpenIddictRequest request);
}

public class TokenService : ITokenService
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public TokenService(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<ClaimsPrincipal> Handle(OpenIddictRequest request)
    {
        if (!request.IsClientCredentialsGrantType())
        {
            throw new NotImplementedException("The specified grant type is not implemented");
        }

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
            throw new NotFoundException<string?>("Admin API Client", request.ClientId);

        if (!await _applicationManager.ValidateClientSecretAsync(application, request.ClientSecret!))
        {
            throw new AuthenticationException("Invalid Admin API Client key and secret");
        }

        var displayName = await _applicationManager.GetDisplayNameAsync(application);

        var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
        identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId!, OpenIddictConstants.Destinations.AccessToken);
        identity.AddClaim(OpenIddictConstants.Claims.Name, displayName!, OpenIddictConstants.Destinations.AccessToken);
        identity.AddClaim(OpenIddictConstants.Claims.Scope, SecurityConstants.ApiFullAccessScope, OpenIddictConstants.Destinations.AccessToken);

        return new ClaimsPrincipal(identity);
    }
}
