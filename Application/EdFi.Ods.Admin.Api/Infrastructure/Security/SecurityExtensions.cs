// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Features.Connect;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace EdFi.Ods.Admin.Api.Infrastructure.Security;

public static class SecurityExtensions
{
    public static void AddSecurityUsingOpenIddict(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        //OpenIddict Server
        var signingKeyValue = configuration.GetValue<string>("Authentication:SigningKey");
        var signingKey = string.IsNullOrEmpty(signingKeyValue) ? null : new SymmetricSecurityKey(Convert.FromBase64String(signingKeyValue));

        services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore().UseDbContext<AdminApiDbContext>()
                    .ReplaceDefaultEntities<ApiApplication, ApiAuthorization, ApiScope, ApiToken, int>();
            })
            .AddServer(opt =>
            {
                opt.AllowClientCredentialsFlow();

                opt.SetTokenEndpointUris(SecurityConstants.TokenEndpointUri);

                opt.AddEphemeralEncryptionKey();
                opt.AddEphemeralSigningKey();
                opt.DisableAccessTokenEncryption();

                if (!webHostEnvironment.IsDevelopment()) //Keys below will override Ephemeral / Dev Keys
                {
                    if (signingKey == null)
                    {
                        throw new Exception("Invalid Configuration: Authentication:SigningKey is required.");
                    }
                    opt.AddSigningKey(signingKey);
                }

                opt.RegisterScopes(SecurityConstants.Scopes.AdminApiFullAccess);
                opt.UseAspNetCore().EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
                options.Configure(options => options.TokenValidationParameters.IssuerSigningKey = signingKey);
            });

        //Application Security
        var issuer = configuration.GetValue<string>("Authentication:IssuerUrl");
        var authority = configuration.GetValue<string>("Authentication:Authority");
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt => {
            opt.Authority = authority;
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = issuer,
                IssuerSigningKey = signingKey
            };
        });
        services.AddAuthorization(opt =>
        {
            opt.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim(OpenIddictConstants.Claims.Scope, SecurityConstants.Scopes.AdminApiFullAccess)
                .Build();
        });

        //Security Endpoints
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IRegisterService, RegisterService>();
        services.AddControllers();
    }
}
