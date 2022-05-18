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

                if (webHostEnvironment.IsDevelopment())
                {
                    opt.AddEphemeralEncryptionKey();
                    opt.AddEphemeralSigningKey();
                }
                else
                {
                    var encryptionKey = configuration.GetValue<string>("AppSettings:EncryptionKey");
                    var signingKey = configuration.GetValue<string>("Authentication:SigningKey");

                    opt.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String(encryptionKey)));
                    opt.AddSigningKey(new SymmetricSecurityKey(Convert.FromBase64String(signingKey)));
                }

                opt.RegisterScopes();
                opt.UseAspNetCore().EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        //Application Security
        var issuer = configuration.GetValue<string>("Authentication:IssuerUrl");
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt => {
            opt.Authority = issuer;
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = issuer,
            };
        });
        services.AddAuthorization(opt =>
        {
            opt.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim(OpenIddictConstants.Claims.Scope, SecurityConstants.ApiFullAccessScope)
                .Build();
        });

        //Security Endpoints
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IRegisterService, RegisterService>();
        services.AddControllers();
    }
}
