// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;
using EdFi.Ods.AdminApi.Common.Settings;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants;

public interface IAdminConsoleTenantsService
{
    Task InitializeTenantsAsync();
    Task<List<TenantModel>> GetTenantsAsync(bool fromCache);
    Task<TenantModel?> GetTenantByTenantIdAsync(int tenantId);
}

public class TenantService : IAdminConsoleTenantsService
{
    private const string ADMIN_DB_KEY = "EdFi_Admin";
    private readonly IOptions<AppSettingsFile> _options;
    protected AppSettingsFile _appSettings;
    private readonly IMemoryCache _memoryCache;
    private static readonly ILog _log = LogManager.GetLogger(typeof(TenantService));

    public TenantService(IOptionsSnapshot<AppSettingsFile> options,
        IMemoryCache memoryCache)
    {
        _options = options;
        _memoryCache = memoryCache;
        _appSettings = _options.Value;
    }

    public async Task InitializeTenantsAsync()
    {
        var tenants = await GetTenantsAsync();
        //store it in memorycache
        await Task.FromResult(_memoryCache.Set(AdminConsoleConstants.TenantsCacheKey, tenants));
    }

    public async Task<List<TenantModel>> GetTenantsAsync(bool fromCache = false)
    {
        List<TenantModel> results = new List<TenantModel>();

        if (fromCache)
        {
            results = await GetTenantsFromCacheAsync();
            if (results.Count > 0)
            {
                return results;
            }
        }

        results = new List<TenantModel>();
        //check multitenancy
        if (_appSettings.AppSettings.MultiTenancy)
        {
            var ordinalId = 1;
            foreach (var tenantConfig in _appSettings.Tenants)
            {
                var connectionString = tenantConfig.Value.ConnectionStrings.First(p => p.Key == ADMIN_DB_KEY).Value;
                if (!ConnectionStringHelper.ValidateConnectionString(_appSettings.AppSettings.DatabaseEngine!, connectionString))
                {
                    _log.Warn($"Tenant {tenantConfig.Key} has a wrong connection string for database {ADMIN_DB_KEY}");

                }
                dynamic document = new ExpandoObject();
                document.edfiApiDiscoveryUrl = tenantConfig.Value.EdFiApiDiscoveryUrl;
                document.name = tenantConfig.Key;
                results.Add(new TenantModel()
                {
                    TenantId = ordinalId,
                    Document = document,
                });
                ordinalId++;
            }
        }
        else
        {
            dynamic document = new ExpandoObject();
            document.edfiApiDiscoveryUrl = _appSettings.EdFiApiDiscoveryUrl;
            document.name = "default";
            results.Add(new TenantModel()
            {
                TenantId = 1,
                Document = document,
            });
        }
        return results;
    }

    public async Task<TenantModel?> GetTenantByTenantIdAsync(int tenantId)
    {
        var tenants = await GetTenantsAsync();
        var tenant = tenants.FirstOrDefault(p => p.TenantId == tenantId);
        return tenant;
    }

    private async Task<List<TenantModel>> GetTenantsFromCacheAsync()
    {
        var tenants = await Task.FromResult(_memoryCache.Get<List<TenantModel>>(AdminConsoleConstants.TenantsCacheKey));
        return tenants ?? new List<TenantModel>();
    }
}

