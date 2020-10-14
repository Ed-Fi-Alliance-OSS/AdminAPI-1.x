// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Mvc;
using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
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
    public abstract class CommonConfigurationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer services, IConfigurationStore store)
        {
            services.AddTransient<IFileUploadHandler, LocalFileSystemFileUploadHandler>();

            services.Register(
                Component.For<IMapper>()
                    .Instance(AutoMapperBootstrapper.CreateMapper())
                    .LifestyleSingleton());

            services.Register(
                Component.For<IApiConfigurationProvider>().ImplementedBy<ApiConfigurationProvider>(),

                Component.For<IConfigValueProvider>().ImplementedBy<AppConfigValueProvider>().IsFallback(),

                Component.For<IDatabaseEngineProvider>().ImplementedBy<DatabaseEngineProvider>().IsFallback(),

                Component.For<IConfigConnectionStringsProvider>().ImplementedBy<AppConfigConnectionStringsProvider>()
                    .IsFallback());

            services.Register(
                Component.For<ISecurityContextFactory>()
                    .ImplementedBy<SecurityContextFactory>(),
                Component.For<ISecurityContext>()
                    .UsingFactoryMethod(k => k.Resolve<ISecurityContextFactory>().CreateContext())
                    .LifestylePerWebRequest());

            services.Register(
                Component.For<IUsersContextFactory>()
                    .ImplementedBy<UsersContextFactory>(),
                Component.For<IUsersContext>()
                    .UsingFactoryMethod(k => k.Resolve<IUsersContextFactory>().CreateContext())
                    .LifestylePerWebRequest());

            services.Register(
                Component.For<TokenCache>()
                    .Instance(TokenCache.DefaultShared));

            services.Register(
                Component.For<AdminAppDbContext>()
                    .ImplementedBy<AdminAppDbContext>()
                    .LifestylePerWebRequest());

            services.Register(
                Component.For<AdminAppUserContext>()
                    .ImplementedBy<AdminAppUserContext>()
                    .LifestylePerWebRequest());

            services.AddTransient<ICloudOdsAdminAppSettingsApiModeProvider, CloudOdsAdminAppSettingsApiModeProvider>();

            services.Register(
                Component.For<IOptions<AppSettings>>()
                    .Instance(new Net48Options<AppSettings>(ConfigurationHelper.GetAppSettings()))
                    .LifestyleSingleton());

            services.Register(
                Component.For<ICachedItems>()
                    .ImplementedBy<CachedItems>()
                    .LifestyleSingleton());

            services.AddTransient<IOdsApiConnectionInformationProvider, CloudOdsApiConnectionInformationProvider>();

            services.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IController>()
                    .LifestyleTransient());

            services.AddTransient<ProductionSetupHub>();
            services.AddTransient<BulkUploadHub>();
            services.AddTransient<ProductionLearningStandardsHub>();
            services.AddTransient<BulkImportService>();
            services.AddTransient<IBackgroundJobClient, BackgroundJobClient>();

            services.Register(
                Component.For<IProductionSetupJob, WorkflowJob<int, ProductionSetupHub>>()
                    .ImplementedBy<ProductionSetupJob>()
                    .LifestyleSingleton());

            services.Register(
                Component.For<IBulkUploadJob, WorkflowJob<BulkUploadJobContext, BulkUploadHub>>()
                    .ImplementedBy<BulkUploadJob>()
                    .LifestyleSingleton());

            services.Register(
                Component
                    .For<IProductionLearningStandardsJob, LearningStandardsJob<ProductionLearningStandardsHub>,
                        WorkflowJob<LearningStandardsJobContext, ProductionLearningStandardsHub>>()
                    .ImplementedBy<ProductionLearningStandardsJob>()
                    .LifestyleSingleton());

            services.Register(
                Component.For<ISecureHasher>()
                    .ImplementedBy<Pbkdf2HmacSha1SecureHasher>());

            services.Register(
                Component.For<IPackedHashConverter>()
                    .ImplementedBy<PackedHashConverter>());

            services.Register(
                Component.For<ISecurePackedHashProvider>()
                    .ImplementedBy<SecurePackedHashProvider>());

            services.Register(
                Component.For<IHashConfigurationProvider>()
                    .ImplementedBy<DefaultHashConfigurationProvider>());

            InstallHostingSpecificClasses(services);

            services.Register(
                Component.For<IOdsSecretConfigurationProvider>()
                    .ImplementedBy<OdsSecretConfigurationProvider>()
                    .LifestyleSingleton());

            services.Register(
                Component.For<InstanceContext>()
                    .ImplementedBy<InstanceContext>()
                    .LifestylePerWebRequest());

             services.Register(
                Classes.FromAssemblyContaining<IMarkerForEdFiOdsAdminAppManagement>()
                    .Pick()
                    .WithServiceAllInterfaces()
                    .LifestyleTransient());

            services.Register(
                Component.For<CloudOdsUpdateService>()
                    .ImplementedBy<CloudOdsUpdateService>()
                    .LifestyleSingleton());

            services.Register(
                Classes.FromThisAssembly()
                    .BasedOn(typeof(IValidator<>))
                    .WithService
                    .Base()
                    .LifestyleTransient());
        }

        protected abstract void InstallHostingSpecificClasses(IWindsorContainer services);
    }
}
