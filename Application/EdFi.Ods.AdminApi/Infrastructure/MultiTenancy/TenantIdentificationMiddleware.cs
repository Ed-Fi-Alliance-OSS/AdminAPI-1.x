// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Context;
using Microsoft.Extensions.Options;
using System.Collections;

namespace EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;

public class TenantResolverMiddleware : IMiddleware
{
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly IContextProvider<TenantConfiguration> _tenantConfigurationContextProvider;
    private readonly IOptions<AppSettings> _options;

    public TenantResolverMiddleware(
        ITenantConfigurationProvider tenantConfigurationProvider,
        IContextProvider<TenantConfiguration> tenantConfigurationContextProvider,
        IOptions<AppSettings> options)
    {
        _tenantConfigurationProvider = tenantConfigurationProvider;
        _tenantConfigurationContextProvider = tenantConfigurationContextProvider;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var multiTenancyEnabled = _options.Value.MultiTenancy;

        if (multiTenancyEnabled && context.Request.Headers.TryGetValue("tenant", out var tenantIdentifier)
            && !string.IsNullOrEmpty(tenantIdentifier))
        {
            if (_tenantConfigurationProvider.Get().TryGetValue((string) tenantIdentifier, out var tenantConfiguration))
            {  
                _tenantConfigurationContextProvider.Set(tenantConfiguration);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                return;
            }
        }

        await next.Invoke(context);
    }
}
