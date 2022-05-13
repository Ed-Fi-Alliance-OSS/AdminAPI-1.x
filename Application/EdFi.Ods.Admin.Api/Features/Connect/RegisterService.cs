// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FluentValidation;
using FluentValidation.Results;
using OpenIddict.Abstractions;

namespace EdFi.Ods.Admin.Api.Features.Connect;

public interface IRegisterService
{
    Task<IResult> Handle(RegisterService.Request request);
}

public class RegisterService : IRegisterService
{
    private readonly Validator _validator;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public RegisterService(Validator validator, IOpenIddictApplicationManager applicationManager)
    {
        _validator = validator;
        _applicationManager = applicationManager;
    }

    public async Task<IResult> Handle(Request request)
    {
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

                OpenIddictConstants.Permissions.Prefixes.Scope + "api"
            },
        };

        await _applicationManager.CreateAsync(application);
        return AdminApiResponse.Ok($"Registered client {request.ClientId} successfully.");
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

    public class Request
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? DisplayName { get; set; }
    }
}
