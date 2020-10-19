// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Web.Mvc;
#if NET48
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
#else
using Microsoft.Extensions.DependencyInjection;
#endif
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Web.Hubs;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.Common.Configuration;
using EdFi.Ods.Common.Security;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation;
using Hangfire;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace EdFi.Ods.AdminApp.Web._Installers
{
    public abstract class CommonConfigurationInstaller
#if NET48
        : IWindsorInstaller
#endif
    {
#if NET48
        public void Install(IWindsorContainer services, IConfigurationStore store)
#else
        public void Install(IServiceCollection services)
#endif
        {
            services.AddTransient<IFileUploadHandler, LocalFileSystemFileUploadHandler>();

#if NET48
            services.AddSingleton(AutoMapperBootstrapper.CreateMapper());
#endif

            services.AddSingleton<IApiConfigurationProvider, ApiConfigurationProvider>();
            services.AddSingleton<IConfigValueProvider, AppConfigValueProvider>();
            services.AddSingleton<IDatabaseEngineProvider, DatabaseEngineProvider>();
            services.AddSingleton<IConfigConnectionStringsProvider, AppConfigConnectionStringsProvider>();
            services.AddSingleton<ISecurityContextFactory, SecurityContextFactory>();
            services.AddSingleton<IUsersContextFactory, UsersContextFactory>();
            services.AddScoped(x => x.GetService<ISecurityContextFactory>().CreateContext());
            services.AddScoped(x => x.GetService<IUsersContextFactory>().CreateContext());

            services.AddSingleton(TokenCache.DefaultShared);

            services.AddScoped<AdminAppDbContext>();
            services.AddScoped<AdminAppUserContext>();

            services.AddTransient<ICloudOdsAdminAppSettingsApiModeProvider, CloudOdsAdminAppSettingsApiModeProvider>();

            services.AddSingleton<ICachedItems, CachedItems>();

            services.AddTransient<IOdsApiConnectionInformationProvider, CloudOdsApiConnectionInformationProvider>();

#if NET48
            services.AddSingleton<IOptions<AppSettings>>(
                new Net48Options<AppSettings>(ConfigurationHelper.GetAppSettings()));

            services.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IController>()
                    .LifestyleTransient());
#endif

            services.AddTransient<ProductionSetupHub>();
            services.AddTransient<BulkUploadHub>();
            services.AddTransient<ProductionLearningStandardsHub>();
            services.AddTransient<BulkImportService>();
            services.AddTransient<IBackgroundJobClient, BackgroundJobClient>();

            services.AddSingleton<IProductionSetupJob, ProductionSetupJob>();
            services.AddSingleton(x => (WorkflowJob<int, ProductionSetupHub>)x.GetService<IProductionSetupJob>());//Resolve previously queued job.

            services.AddSingleton<IBulkUploadJob, BulkUploadJob>();
            services.AddSingleton(x => (WorkflowJob<BulkUploadJobContext, BulkUploadHub>)x.GetService<IBulkUploadJob>());//Resolve previously queued job.

            services.AddSingleton<IProductionLearningStandardsJob, ProductionLearningStandardsJob>();
            services.AddSingleton(x => (WorkflowJob<LearningStandardsJobContext, ProductionLearningStandardsHub>) x.GetService<IProductionLearningStandardsJob>());//Resolve previously queued job.

            services.AddSingleton<ISecureHasher, Pbkdf2HmacSha1SecureHasher>();
            services.AddSingleton<IPackedHashConverter, PackedHashConverter>();
            services.AddSingleton<ISecurePackedHashProvider, SecurePackedHashProvider>();
            services.AddSingleton<IHashConfigurationProvider, DefaultHashConfigurationProvider>();

            InstallHostingSpecificClasses(services);

            services.AddSingleton<IOdsSecretConfigurationProvider, OdsSecretConfigurationProvider>();

            services.AddScoped<InstanceContext>();

            services.AddTransient<ApplicationConfigurationService>();
            services.AddTransient<CloudOdsUpdateCheckService>();

            foreach (var type in typeof(IMarkerForEdFiOdsAdminAppManagement).Assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && (type.IsPublic || type.IsNestedPublic))
                {
                    var concreteClass = type;

                    if (concreteClass == typeof(OdsSecretConfigurationProvider))
                        continue; //Singleton registered above.

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

            services.AddSingleton<CloudOdsUpdateService>();

#if NET48
            services.Register(
                Classes.FromThisAssembly()
                    .BasedOn(typeof(IValidator<>))
                    .WithService
                    .Base()
                    .LifestyleTransient());
#endif
        }

#if NET48
        protected abstract void InstallHostingSpecificClasses(IWindsorContainer services);
#else
        protected abstract void InstallHostingSpecificClasses(IServiceCollection services);
#endif
    }
}
