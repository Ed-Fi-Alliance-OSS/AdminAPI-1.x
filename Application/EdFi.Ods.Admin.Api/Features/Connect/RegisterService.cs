// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.Admin.Api.Infrastructure.Security;
using FluentValidation;
using FluentValidation.Results;
using OpenIddict.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.Admin.Api.Features.Connect;

public interface IRegisterService
{
    Task<string> Handle(RegisterService.Request request);
}

public class RegisterService : IRegisterService
{
    private readonly IConfiguration _configuration;
    private readonly Validator _validator;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public RegisterService(IConfiguration configuration, Validator validator, IOpenIddictApplicationManager applicationManager)
    {
        _configuration = configuration;
        _validator = validator;
        _applicationManager = applicationManager;
    }

    public async Task<string> Handle(Request request)
    {
        if(!await RegistrationIsEnabledOrNecessary())
            throw new Exception("Application registration is disabled");

        await _validator.GuardAsync(request);

        var existingApp = await _applicationManager.FindByClientIdAsync(request.ClientId!);
        if (existingApp != null)
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.ClientId), $"ClientId {request.ClientId} already exists") });

        var application = new OpenIddictApplicationDescriptor
        {
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret,
            DisplayName = request.DisplayName,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.Prefixes.Scope + SecurityConstants.ApiFullAccessScope
            },
        };

        await _applicationManager.CreateAsync(application);
        return $"Registered client {request.ClientId} successfully.";
    }

    private async Task<bool> RegistrationIsEnabledOrNecessary()
    {
        var registrationIsEnabled = _configuration.GetValue<bool>("Authentication:AllowRegistration");
        var applicationCount = await _applicationManager.CountAsync();
        return registrationIsEnabled || applicationCount == 0;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.ClientId).NotEmpty();
            RuleFor(m => m.ClientSecret).NotEmpty();
            RuleFor(m => m.DisplayName).NotEmpty();
        }
    }

    [DisplaySchemaName("Register")]
    public class Request
    {
        [SwaggerRequired]
        [SwaggerSchema(Description = FeatureConstants.RegisterClientId, Nullable = false)]
        public string? ClientId { get; set; }
        [SwaggerRequired]
        [SwaggerSchema(Description = FeatureConstants.RegisterClientSecret, Nullable = false)]
        public string? ClientSecret { get; set; }
        [SwaggerRequired]
        [SwaggerSchema(Description = FeatureConstants.RegisterDisplayName, Nullable = false)]
        public string? DisplayName { get; set; }
    }
}
