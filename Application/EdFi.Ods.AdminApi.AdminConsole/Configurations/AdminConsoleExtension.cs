// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class AdminConsoleExtension
{
    public static void UseCorsForAdminConsole(this WebApplication app)
    {
        var adminConsoleSettings = app.Services.GetService<IOptions<AdminConsoleSettings>>();

        if (adminConsoleSettings != null && adminConsoleSettings.Value.CorsSettings.EnableCors)
            app.UseCors(AdminConsoleConstants.CorsPolicyName);
    }
}
