// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Common.Extensions;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.LearningStandards.Core.Configuration;
using EdFi.Admin.LearningStandards.Core.Services;
using EdFi.Admin.LearningStandards.Core.Services.Interfaces;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Web.Hubs;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Common.Security;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using EdFi.Security.DataAccess.Contexts;
using Hangfire;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace EdFi.Ods.AdminApp.Web._Installers
{
    public abstract class CommonConfigurationInstaller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommonConfigurationInstaller));

        public void Install(IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IFileUploadHandler, LocalFileSystemFileUploadHandler>();

            services.AddScoped<ISecurityContext>(x =>
            {
                var connectionStrings = x.GetService<IOptions<ConnectionStrings>>();

                if (appSettings.DatabaseEngine.EqualsIgnoreCase("SqlServer"))
                    return new SqlServerSecurityContext(connectionStrings.Value.Security);

                return new PostgresSecurityContext(connectionStrings.Value.Security);
            });

            services.AddScoped<IUsersContext>(x =>
            {
                var connectionStrings = x.GetService<IOptions<ConnectionStrings>>();

                if (appSettings.DatabaseEngine.EqualsIgnoreCase("SqlServer"))
                    return new SqlServerUsersContext(connectionStrings.Value.Admin);

                return new PostgresUsersContext(connectionStrings.Value.Admin);
            });

            services.AddSingleton(TokenCache.DefaultShared);

            services.AddScoped<AdminAppUserContext>();

            services.AddTransient<ICloudOdsAdminAppSettingsApiModeProvider, CloudOdsAdminAppSettingsApiModeProvider>();

            services.AddTransient<IOdsApiConnectionInformationProvider, CloudOdsApiConnectionInformationProvider>();

            services.AddTransient<BulkUploadHub>();
            services.AddTransient<ProductionLearningStandardsHub>();
            services.AddTransient<BulkImportService>();
            services.AddTransient<IBackgroundJobClient, BackgroundJobClient>();

            services.AddSingleton<IBulkUploadJob, BulkUploadJob>();
            services.AddSingleton(x => (WorkflowJob<BulkUploadJobContext, BulkUploadHub>)x.GetService<IBulkUploadJob>());//Resolve previously queued job.

            services.AddSingleton<IProductionLearningStandardsJob, ProductionLearningStandardsJob>();
            services.AddSingleton(x => (WorkflowJob<LearningStandardsJobContext, ProductionLearningStandardsHub>) x.GetService<IProductionLearningStandardsJob>());//Resolve previously queued job.

            services.AddSingleton<ISecureHasher, Pbkdf2HmacSha1SecureHasher>();
            services.AddSingleton<IPackedHashConverter, PackedHashConverter>();

            services.AddSingleton<ISecureHasherProvider, SecureHasherProvider>(
                x => new SecureHasherProvider(new List<ISecureHasher> { x.GetService<ISecureHasher>() }));

            services.AddSingleton<ISecurePackedHashProvider, SecurePackedHashProvider>();
            services.AddSingleton<IHashConfigurationProvider, DefaultHashConfigurationProvider>();

            InstallHostingSpecificClasses(services);

            services.AddTransient<IHomeScreenDisplayService, HomeScreenDisplayService>();

            services.AddScoped<InstanceContext>();

            services.AddTransient<ITelemetry, Telemetry>();
            services.AddTransient<IProductRegistration, ProductRegistration>();

            services.AddTransient<ApplicationConfigurationService>();

            services.AddTransient<IClaimSetCheckService, ClaimSetCheckService>();

            foreach (var type in typeof(IMarkerForEdFiOdsAdminAppManagement).Assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && (type.IsPublic || type.IsNestedPublic))
                {
                    var concreteClass = type;

                    if (concreteClass == typeof(OdsApiFacade))
                        continue; //IOdsApiFacade is never resolved. Instead, classes inject IOdsApiFacadeFactory.

                    if (concreteClass == typeof(OdsRestClient))
                        continue; //IOdsRestClient is never resolved. Instead, classes inject IOdsRestClientFactory.

                    if (concreteClass == typeof(TokenRetriever))
                        continue; //ITokenRetriever is never resolved. Instead, other dependencies construct TokenRetriever directly.

                    var interfaces = concreteClass.GetInterfaces().ToArray();

                    if (interfaces.Length == 1)
                    {
                        var serviceType = interfaces.Single();

                        if (serviceType.FullName == $"{concreteClass.Namespace}.I{concreteClass.Name}")
                            services.AddTransient(serviceType, concreteClass);
                    }
                    else if (interfaces.Length == 0)
                    {
                        if (concreteClass.Name.EndsWith("Command") ||
                            concreteClass.Name.EndsWith("Query"))
                            services.AddTransient(concreteClass);
                    }
                }
            }

            services.AddSingleton<IStringEncryptorService, AESEncryptorService>(
                    x => new AESEncryptorService(appSettings.EncryptionKey));
        }

        protected abstract void InstallHostingSpecificClasses(IServiceCollection services);

        public static async Task ConfigureLearningStandards(IServiceCollection services)
        {
            var serviceProvider = (IServiceProvider)services.BuildServiceProvider();

            var learningStandardsMaxSimultaneousRequests = await GetLearningStandardsMaxSimultaneousRequests(serviceProvider);

            var config = new EdFiOdsApiClientConfiguration(
                maxSimultaneousRequests: learningStandardsMaxSimultaneousRequests);

            var serviceCollection = new ServiceCollection();

            var pluginConnector = new LearningStandardsCorePluginConnector(
                serviceCollection,
                ServiceProviderFunc,
                new LearningStandardLogProvider(),
                config
            );

            services.AddSingleton<ILearningStandardsCorePluginConnector>(pluginConnector);
        }

        private static async Task<int> GetLearningStandardsMaxSimultaneousRequests(IServiceProvider serviceProvider)
        {
            const int IdealSimultaneousRequests = 4;
            const int PessimisticSimultaneousRequests = 1;

            try
            {
                var inferOdsApiVersion = serviceProvider.GetRequiredService<IInferOdsApiVersion>();
                var odsApiVersion = await inferOdsApiVersion.Version(CloudOdsAdminAppSettings.Instance.ProductionApiUrl);

                return odsApiVersion.StartsWith("3.") ? PessimisticSimultaneousRequests : IdealSimultaneousRequests;
            }
            catch (Exception e)
            {
                Logger.Warn(
                    "Failed to infer ODS / API version to determine Learning Standards " +
                    $"MaxSimultaneousRequests. Assuming a max of {PessimisticSimultaneousRequests}.", e);

                return PessimisticSimultaneousRequests;
            }
        }

        private static IServiceProvider ServiceProviderFunc(IServiceCollection collection)
        {
            return collection.BuildServiceProvider();
        }
    }
}
