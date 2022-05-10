// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestSharp;
using static System.Environment;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public interface IProductRegistration
    {
        Task NotifyWhenEnabled(AdminAppUser user);
    }

    public class ProductRegistration : IProductRegistration
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ProductRegistration));

        private readonly AdminAppDbContext _database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationConfigurationService _applicationConfigurationService;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;
        private static IInferOdsApiVersion _inferOdsApiVersion;

        private readonly string _odsApiMode;
        private readonly string _productRegistrationUrl;

        public ProductRegistration(
            IOptions<AppSettings> appSettingsAccessor,
            AdminAppDbContext database,
            IHttpContextAccessor httpContextAccessor,
            ApplicationConfigurationService applicationConfigurationService,
            IGetOdsInstanceRegistrationsQuery getOdsInstanceRegistrationsQuery,
            IInferOdsApiVersion inferOdsApiVersion)
        {
            _database = database;
            _httpContextAccessor = httpContextAccessor;
            _applicationConfigurationService = applicationConfigurationService;
            _getOdsInstanceRegistrationsQuery = getOdsInstanceRegistrationsQuery;
            _inferOdsApiVersion = inferOdsApiVersion;

            var appSettings = appSettingsAccessor.Value;
            _odsApiMode = appSettings.ApiStartupType;
            _productRegistrationUrl = appSettings.ProductRegistrationUrl;
        }

        public async Task NotifyWhenEnabled(AdminAppUser user)
        {
            try
            {
                var productImprovementEnabled = _applicationConfigurationService.IsProductImprovementEnabled(out var productRegistrationId);
                var productRegistrationUrlAvailable = !string.IsNullOrEmpty(_productRegistrationUrl);
                var productRegistrationIdAvailable = !string.IsNullOrEmpty(productRegistrationId);

                if (productImprovementEnabled && productRegistrationUrlAvailable && productRegistrationIdAvailable)
                {
                    await Notify(await BuildProductRegistrationModel(productRegistrationId, user));
                }
            }
            catch (Exception exception)
            {
                _logger.Warn("Could not submit product registration. This notice is merely diagnostic " +
                             "and does not limit Admin App functionality.", exception);
            }
        }

        private async Task Notify(ProductRegistrationModel model)
        {
            var client = new RestClient(_productRegistrationUrl);

            var request = new RestRequest
            {
                Method = Method.POST,

                //For absolute control of this externally-facing JSON format,
                //we serialize ourselves below rather than relying on RestSharp's
                //internal serializer.
                RequestFormat = DataFormat.None
            };

            request.AddHeader("Accept", "application/json");
            request.AddParameter("", model.Serialize(), "application/json", ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                _logger.Info("Product registration submitted.");
            }
            else
            {
                _logger.Info(
                    "The product registration POST failed. This may happen if the product registration service " +
                    "is temporarily unavailable or blocked on your network. This notice is merely informational " +
                    "and does not limit Admin App functionality. The attempted POST returned with status code " +
                    $"{(int)response.StatusCode} ({response.StatusDescription}).");
            }
        }

        private async Task<ProductRegistrationModel> BuildProductRegistrationModel(string productRegistrationId, AdminAppUser user)
        {
            var singleOdsApiConnection = new ProductRegistrationModel.OdsApiConnection
            {
                OdsApiVersion = await OdsApiVersion(),
                OdsApiMode = _odsApiMode,
                InstanceCount = InstanceCount()
            };

            return new ProductRegistrationModel
            {
                ProductRegistrationId = productRegistrationId,

                // Naturally Admin App always has 1 connection. This property supports an array
                // in case other Ed-Fi applications need to report multiple connections using the
                // same JSON model.
                OdsApiConnections = new[] { singleOdsApiConnection },

                ProductVersion = ProductVersion(),
                OsVersion = OsVersion(),
                DatabaseVersion = DatabaseVersion(),
                HostName = HostName(),
                UserId = user?.Id ?? "",
                UserName = user?.UserName ?? "",
                UtcTimeStamp = DateTime.UtcNow
            };
        }

        private string HostName()
        {
            return Try(() =>
            {
                var requestHost = _httpContextAccessor.HttpContext.Request.Host;

                return requestHost.HasValue
                    ? requestHost.Host
                    : null;
            });
        }

        private static string ProductVersion()
        {
            return Try(() => Version.InformationalVersion);
        }

        private static Task<string> OdsApiVersion()
        {
            return Try(
                async () => await InMemoryCache.Instance.GetOrSet(
                "OdsApiVersion", async () => await _inferOdsApiVersion.Version(
                    CloudOdsAdminAppSettings.Instance.ProductionApiUrl)));
        }

        private int? InstanceCount()
        {
            return Try(() => _getOdsInstanceRegistrationsQuery.ExecuteCount());
        }

        private static string OsVersion()
        {
            return Try(() => OSVersion.VersionString);
        }

        private string DatabaseVersion()
        {
            return Try(() => InMemoryCache.Instance.GetOrSet("DatabaseVersion", SelectVersionString));

            string SelectVersionString()
            {
                return "SqlServer".Equals(CloudOdsAdminAppSettings.Instance.DatabaseEngine, StringComparison.InvariantCultureIgnoreCase)
                    ? _database.DatabaseVersionView.FromSqlRaw("SELECT @@VERSION as VersionString").First().VersionString
                    : _database.DatabaseVersionView.FromSqlRaw("SELECT version() as VersionString").First().VersionString;
            }
        }

        private static string Try(Func<string> getValue, [CallerMemberName] string caller = null)
        {
            string value = null;

            try
            {
                value = getValue();
            }
            catch (Exception exception)
            {
                _logger.Error($"Could not determine '{caller}' when submitting product registration. " +
                              "This notice is merely diagnostic and does not limit Admin App functionality.", exception);
            }

            return value ?? "";
        }

        private static int? Try(Func<int> getValue, [CallerMemberName] string caller = null)
        {
            try
            {
                return getValue();
            }
            catch (Exception exception)
            {
                _logger.Error($"Could not determine '{caller}' when submitting product registration. " +
                              "This notice is merely diagnostic and does not limit Admin App functionality.", exception);
            }

            return null;
        }

        private static async Task<string> Try(Func<Task<string>> getValue, [CallerMemberName] string caller = null)
        {
            string value = null;

            try
            {
                value = await getValue();
            }
            catch (Exception exception)
            {
                _logger.Error($"Could not determine '{caller}' when submitting product registration. " +
                              "This notice is merely diagnostic and does not limit Admin App functionality.", exception);
            }

            return value ?? "";
        }
    }
}
