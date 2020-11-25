// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using Owin;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.ApplicationInsights.Extensibility;

namespace EdFi.Ods.AdminApp.Web
{
    public abstract class StartupBase
    {
        public virtual void Configuration(IAppBuilder appBuilder)
        {
            ConfigureLogging();
            ConfigureTls();
        }

        /// <summary>
        /// Explicitly configures all outgoing network calls -- particularly to the ODS API(s) -- to use
        /// the latest version of TLS where possible. Due to our reliance on some older libraries, the .NET framework won't necessarily default
        /// to the latest unless we explicitly request it. Some hosting environments will not allow older versions
        /// of TLS, and thus calls can fail without this extra configuration.
        /// </summary>
        private static void ConfigureTls()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        protected void ConfigureLogging()
        {
            if (AppSettings.AppStartup == "Azure")
            {
                var applicationInsightsInstrumentationKey = AppSettings.ApplicationInsightsInstrumentationKey;

                if (applicationInsightsInstrumentationKey != null)
                {
                    TelemetryConfiguration.Active.InstrumentationKey = applicationInsightsInstrumentationKey;
                    Logger.DebugFormat("Found AppInsights instrumentation key in Web.config -- AppInsights will capture traces");
                }
                else
                {
                    Logger.DebugFormat("No instrumentation key found in Web.config -- AppInsights will NOT capture traces");
                }
            }
        }
    }
}
