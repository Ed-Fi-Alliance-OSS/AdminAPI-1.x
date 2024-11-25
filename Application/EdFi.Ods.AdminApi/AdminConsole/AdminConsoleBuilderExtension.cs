// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using log4net;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class AdminConsoleBuilderExtension
{
    public static void RegisterAdminConsoleDependencies(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.ConfigureAdminConsoleDatabase();
        webApplicationBuilder.AddAdminConsoleServices();
    }

    public static void RegisterAdminConsoleCorsDependencies(this WebApplicationBuilder webApplicationBuilder, ILog logger)
    {
        var corsSettings = webApplicationBuilder.Configuration.GetSection(Constants.ADMINCONSOLE_SETTINGS_KEY);
        var enableCors = corsSettings.GetValue<bool>(Constants.ENABLE_CORS_KEY);
        var allowedOrigins = corsSettings.GetSection(Constants.ALLOWED_ORIGINS_CORS_KEY).Get<string[]>();
        // Read CORS settings from configuration
        if (enableCors && allowedOrigins != null)
        {
            if (allowedOrigins.Length > 0)
            {
                webApplicationBuilder.Services.AddCors(options =>
                {
                    options.AddPolicy(Constants.CORS_POLICY_NAME, policy =>
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
                logger.Warn("CORS is enabled, but no allowed origins are specified.");
            }
        }
    }
}
