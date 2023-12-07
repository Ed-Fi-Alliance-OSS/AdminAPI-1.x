// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;

public interface ITenantConfigurationProvider
{
    IDictionary<string, TenantConfiguration> Get();
}

public class TenantConfigurationProvider : ITenantConfigurationProvider
{
    private readonly IOptionsMonitor<TenantsSection> _tenantsConfigurationOptions;

    private IDictionary<string, TenantConfiguration> _tenantConfigurationByIdentifier;

    public TenantConfigurationProvider(IOptionsMonitor<TenantsSection> tenantsConfigurationOptions)
    {
        _tenantsConfigurationOptions = tenantsConfigurationOptions;
        _tenantConfigurationByIdentifier = InitializeTenantsConfiguration(_tenantsConfigurationOptions.CurrentValue);

        _tenantsConfigurationOptions.OnChange(config =>
        {
            var newMap = InitializeTenantsConfiguration(config);
            Interlocked.Exchange(ref _tenantConfigurationByIdentifier, newMap);
        });
    }

    private static Dictionary<string, TenantConfiguration> InitializeTenantsConfiguration(TenantsSection config)
    {
        return config.Tenants.ToDictionary(
            t => t.Key,
            t => new TenantConfiguration
            {
                TenantIdentifier = t.Key,
                AdminConnectionString = t.Value.ConnectionStrings.GetValueOrDefault("EdFi_Admin"),
                SecurityConnectionString = t.Value.ConnectionStrings.GetValueOrDefault("EdFi_Security"),
            },
            StringComparer.OrdinalIgnoreCase);
    }

    public IDictionary<string, TenantConfiguration> Get() => _tenantConfigurationByIdentifier;
}

