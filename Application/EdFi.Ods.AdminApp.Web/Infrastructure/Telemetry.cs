// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Helpers;
using GoogleAnalyticsTracker.Simple;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static System.Environment;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public interface ITelemetry
    {
        Task Event(string category, string action, string label);
    }

    public class Telemetry : ITelemetry
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Telemetry));

        private readonly AdminAppUserContext _userContext;
        private readonly AdminAppDbContext _database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInferOdsApiVersion _inferOdsApiVersion;
        private const string NotSet = "(not set)";

        private readonly string _measurementId;
        private readonly string _informationalVersion;
        private readonly SimpleTrackerEnvironment _environment;

        public Telemetry(IOptions<AppSettings> appSettingsAccessor, AdminAppUserContext userContext, AdminAppDbContext database, IHttpContextAccessor httpContextAccessor,
            IInferOdsApiVersion inferOdsApiVersion)
        {
            _userContext = userContext;
            _database = database;
            _httpContextAccessor = httpContextAccessor;
            _inferOdsApiVersion = inferOdsApiVersion;

            var appSettings = appSettingsAccessor.Value;
            _measurementId = appSettings.GoogleAnalyticsMeasurementId;

            _informationalVersion = Version.InformationalVersion;

            _environment = new SimpleTrackerEnvironment(
                OSVersion.Platform.ToString(),
                OSVersion.Version.ToString(),
                OSVersion.VersionString
            );
        }

        public async Task Event(string category, string action, string label)
        {
            //NOTE: Custom Dimension numbers are meaningful, but defined within Google Analytics.
            string odsApiVersion = null;
            try
            {
                odsApiVersion = await InMemoryCache.Instance.GetOrSet("OdsApiVersion", async () => await _inferOdsApiVersion.Version(
                            CloudOdsAdminAppSettings.Instance.ProductionApiUrl));
            }
            catch (Exception e)
            {
                _logger.Error("Google Analytics Telemetry failed for ODS API version", e);
            }

            var databaseVersion = InMemoryCache.Instance
                .GetOrSet("DatabaseVersion", DatabaseVersion);

            var userName = _userContext?.User?.UserName;
            string domainName = null;
            if(userName != null && userName.Contains('@'))
                domainName = userName.Split('@',2)[1];

            var hostName = _httpContextAccessor.HttpContext.Request.Host.Value;

            var customDimensions = new Dictionary<int, string>
            {
                [1] = ExplicitWhenNotSet(_informationalVersion),
                [2] = ExplicitWhenNotSet(odsApiVersion),
                [3] = ExplicitWhenNotSet(databaseVersion),
                [4] = ExplicitWhenNotSet(domainName),
                [5] = ExplicitWhenNotSet(hostName)
            };

            using (var tracker = new SimpleTracker(_measurementId, _environment))
                await tracker.TrackEventAsync(
                    ExplicitWhenNotSet(category),
                    ExplicitWhenNotSet(action),
                    ExplicitWhenNotSet(label),
                    customDimensions);

            string DatabaseVersion()
            {
                return "SqlServer".Equals(CloudOdsAdminAppSettings.Instance.DatabaseEngine, StringComparison.InvariantCultureIgnoreCase)
                    ? _database.DatabaseVersionView.FromSqlRaw("SELECT @@VERSION as VersionString").First().VersionString
                    : _database.DatabaseVersionView.FromSqlRaw("SELECT version() as VersionString").First().VersionString;
            }
        }

        private static string ExplicitWhenNotSet(string customDimensionValue)
            => string.IsNullOrEmpty(customDimensionValue) ? NotSet : customDimensionValue;
    }
}
