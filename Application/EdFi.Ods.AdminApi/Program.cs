// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AspNetCoreRateLimit;
using EdFi.Ods.AdminApi.Features;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;
using log4net;

var builder = WebApplication.CreateBuilder(args);

//Rate Limit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();
// logging
var _logger = LogManager.GetLogger("Program");
_logger.Info("Starting Admin API");
// Read CORS settings from configuration
var corsSettings = builder.Configuration.GetSection("AdminConsole");
var enableCors = corsSettings.GetValue<bool>("CorsSettings:EnableCors");
string allowAllCorsPolicyName = "allowAllCorsPolicyName";
var allowedOrigins = corsSettings.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
if (enableCors && allowedOrigins != null)
{
    if (allowedOrigins != null && allowedOrigins.Length > 0)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(allowAllCorsPolicyName, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }
    else
    {
        // Handle the case where allowedOrigins is null or empty
        _logger.Warn("CORS is enabled, but no allowed origins are specified.");
    }
}
builder.AddServices();

var app = builder.Build();
if (enableCors
    && allowedOrigins != null
    && allowedOrigins.Length > 0)
{
    app.UseCors(allowAllCorsPolicyName);
}

var pathBase = app.Configuration.GetValue<string>("AppSettings:PathBase");
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase($"/{pathBase.Trim('/')}");
    app.UseForwardedHeaders();
}

AdminApiVersions.Initialize(app);

app.UseIpRateLimiting();
//The ordering here is meaningful: Logging -> Routing -> Auth -> Endpoints
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TenantResolverMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapFeatureEndpoints();
//Map AdminConsole endpoints if the flag is enable
if (app.Configuration.GetValue<bool>("AppSettings:EnableAdminConsoleAPI"))
{
    app.MapAdminConsoleFeatureEndpoints();
}
app.MapControllers();
app.UseHealthChecks("/health");

if (app.Configuration.GetValue<bool>("SwaggerSettings:EnableSwagger"))
{
    app.UseSwagger();
    app.DefineSwaggerUIWithApiVersions(AdminApiVersions.GetAllVersionStrings());
}

app.Run();
