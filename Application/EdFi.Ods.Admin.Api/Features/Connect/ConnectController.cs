// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.Admin.Api.Infrastructure.Security;
using FluentValidation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace EdFi.Ods.Admin.Api.Features.Connect;

[AllowAnonymous]
public class ConnectController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly IRegisterService _registerService;

    public ConnectController(ITokenService tokenService, IRegisterService registerService)
    {
        _tokenService = tokenService;
        _registerService = registerService;
    }

    [HttpPost("/connect/register")]
    [Consumes("application/x-www-form-urlencoded"), Produces("application/json")]
    [OperationDescription("Registers new client", "Registers new client")]
    public async Task<IActionResult> Register(RegisterService.Request request)
    {
        var message = await _registerService.Handle(request);
        return Ok(new { Title = message, Status = 200 });
    }

    [OperationDescription("Retrieves bearer token", "\nTo authenticate Swagger requests, execute using \"Authorize\" above, not \"Try It Out\" here.")]
    [HttpPost(SecurityConstants.TokenEndpointUri)]
    [Consumes("application/x-www-form-urlencoded"), Produces("application/json")]
    public async Task<ActionResult> Token()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
            throw new ValidationException("Failed to parse token request");

        var principal = await _tokenService.Handle(request);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
