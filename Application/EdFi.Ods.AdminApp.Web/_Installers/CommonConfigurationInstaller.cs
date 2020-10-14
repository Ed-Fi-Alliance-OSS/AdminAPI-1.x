// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Web.Hubs;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.Common.Configuration;
using EdFi.Ods.Common.InversionOfControl;
using EdFi.Ods.Common.Security;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation;
using Hangfire;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EdFi.Ods.AdminApp.Web._Installers
{
    // Suppressing this ReSharper warning because these methods are called by Castle Windsor through
    // the [Preregister] attribute, and  thus appear to the compiler to never be used.
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class CommonConfigurationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterFileUploadHandler(container);
            RegisterIMapper(container);
            RegisterQueries(container);
            RegisterSecurityContextAndUserContext(container);
            RegisterTokenCache(container);

            RegisterAdminAppDbContext(container);
            RegisterAdminAppUserContext(container);
            RegisterApiModeProvider(container);
            RegisterAppSettings(container);
            RegisterCaches(container);
            RegisterConnectionInformationProvider(container);
            RegisterControllers(container);
            RegisterEncryptionConfigurationProviderService(container);
            RegisterHangfireServices(container);
            RegisterHostingConfigSpecificClassesAndDependencies(container);
            RegisterInstanceContext(container);
            RegisterLearningStandardsSetupCommand(container);
            RegisterMarkerForEdFiOdsAdminAppManagement(container);
            RegisterUpdateCheckService(container);
            RegisterValidators(container);
        }

        [Preregister]
        private static void RegisterFileUploadHandler(IWindsorContainer container)
        {
            container.Register(
                Component.For<IFileUploadHandler>()
                    .ImplementedBy<LocalFileSystemFileUploadHandler>()
                    .LifestyleTransient());
        }

        [Preregister]
        private static void RegisterIMapper(IWindsorContainer container)
        {
            container.Register(
                Component.For<IMapper>()
                    .Instance(AutoMapperBootstrapper.CreateMapper())
                    .LifestyleSingleton());
        }

        [Preregister]
        private static void RegisterQueries(IWindsorContainer container)
        {
            container.Register(
                Component.For<IGetVendorByIdQuery>()
                    .ImplementedBy<GetVendorByIdQuery>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IGetVendorsQuery>()
                    .ImplementedBy<GetVendorsQuery>()
                    .LifestyleTransient());
        }

        [Preregister]
        private static void RegisterSecurityContextAndUserContext(IWindsorContainer container)
        {
            container.Register(
                Component.For<IApiConfigurationProvider>().ImplementedBy<ApiConfigurationProvider>(),

                Component.For<IConfigValueProvider>().ImplementedBy<AppConfigValueProvider>().IsFallback(),

                Component.For<IDatabaseEngineProvider>().ImplementedBy<DatabaseEngineProvider>().IsFallback(),

                Component.For<IConfigConnectionStringsProvider>().ImplementedBy<AppConfigConnectionStringsProvider>()
                    .IsFallback());

            container.Register(
                Component.For<ISecurityContextFactory>()
                    .ImplementedBy<SecurityContextFactory>(),
                Component.For<ISecurityContext>()
                    .UsingFactoryMethod(k => k.Resolve<ISecurityContextFactory>().CreateContext())
                    .LifestylePerWebRequest());

            container.Register(
                Component.For<IUsersContextFactory>()
                    .ImplementedBy<UsersContextFactory>(),
                Component.For<IUsersContext>()
                    .UsingFactoryMethod(k => k.Resolve<IUsersContextFactory>().CreateContext())
                    .LifestylePerWebRequest());
        }

        [Preregister]
        private static void RegisterTokenCache(IWindsorContainer container)
        {
            container.Register(
                Component.For<TokenCache>()
                    .Instance(TokenCache.DefaultShared));
        }

        private static void RegisterCaches(IWindsorContainer container)
        {
            container.Register(
                Component.For<ICachedItems>()
                    .ImplementedBy<CachedItems>()
                    .LifestyleSingleton());
        }

        private static void RegisterConnectionInformationProvider(IWindsorContainer container)
        {
            container.Register(
                Component.For<IOdsApiConnectionInformationProvider>()
                    .ImplementedBy<CloudOdsApiConnectionInformationProvider>()
                    .LifestyleTransient());
        }

        private static void RegisterControllers(IWindsorContainer container)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IController>()
                    .LifestyleTransient());
        }

        private static void RegisterHangfireServices(IWindsorContainer container)
        {
            container.Register(
                Component.For<ProductionSetupHub>()
                    .ImplementedBy<ProductionSetupHub>()
                    .LifestyleTransient());

            container.Register(
                Component.For<BulkUploadHub>()
                    .ImplementedBy<BulkUploadHub>()
                    .LifestyleTransient());

            container.Register(
                Component.For<ProductionLearningStandardsHub>()
                    .ImplementedBy<ProductionLearningStandardsHub>()
                    .LifestyleTransient());

            container.Register(
                Component.For<BulkImportService>()
                    .ImplementedBy<BulkImportService>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IBackgroundJobClient>()
                    .ImplementedBy<BackgroundJobClient>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IProductionSetupJob, WorkflowJob<int, ProductionSetupHub>>()
                    .ImplementedBy<ProductionSetupJob>()
                    .LifestyleSingleton());

            container.Register(
                Component.For<IBulkUploadJob, WorkflowJob<BulkUploadJobContext, BulkUploadHub>>()
                    .ImplementedBy<BulkUploadJob>()
                    .LifestyleSingleton());

            container.Register(
                Component
                    .For<IProductionLearningStandardsJob, LearningStandardsJob<ProductionLearningStandardsHub>,
                        WorkflowJob<LearningStandardsJobContext, ProductionLearningStandardsHub>>()
                    .ImplementedBy<ProductionLearningStandardsJob>()
                    .LifestyleSingleton());
        }

        protected abstract void InstallHostingSpecificClasses(IWindsorContainer container);

        private static void InstallSecretHashingSupport(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISecureHasher>()
                    .ImplementedBy<Pbkdf2HmacSha1SecureHasher>());

            container.Register(
                Component.For<IPackedHashConverter>()
                    .ImplementedBy<PackedHashConverter>());

            container.Register(
                Component.For<ISecurePackedHashProvider>()
                    .ImplementedBy<SecurePackedHashProvider>());

            container.Register(
                Component.For<IHashConfigurationProvider>()
                    .ImplementedBy<DefaultHashConfigurationProvider>());
        }

        private void RegisterHostingConfigSpecificClassesAndDependencies(IWindsorContainer container)
        {
            InstallSecretHashingSupport(container);
            InstallHostingSpecificClasses(container);

            container.Register(
                Component.For<IOdsSecretConfigurationProvider>()
                    .ImplementedBy<OdsSecretConfigurationProvider>()
                    .LifestyleSingleton());

            container.Register(
                Component.For<IGetOdsSqlConfigurationQuery>()
                    .ImplementedBy<GetOdsSqlConfigurationQuery>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IGetOdsAdminAppApiCredentialsQuery>()
                    .ImplementedBy<GetOdsAdminAppApiCredentialsQuery>()
                    .LifestyleTransient());
        }

        private static void RegisterMarkerForEdFiOdsAdminAppManagement(IWindsorContainer container)
        {
            container.Register(
                Classes.FromAssemblyContaining<IMarkerForEdFiOdsAdminAppManagement>()
                    .Pick()
                    .WithServiceAllInterfaces()
                    .LifestyleTransient());
        }

        private static void RegisterUpdateCheckService(IWindsorContainer container)
        {
            container.Register(
                Component.For<CloudOdsUpdateService>()
                    .ImplementedBy<CloudOdsUpdateService>()
                    .LifestyleSingleton());
        }

        private static void RegisterValidators(IWindsorContainer container)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn(typeof(IValidator<>))
                    .WithService
                    .Base()
                    .LifestyleTransient());
        }

        private static void RegisterEncryptionConfigurationProviderService(IWindsorContainer container)
        {
            container.Register(
                Component.For<IEncryptionConfigurationProviderService>()
                    .ImplementedBy<EncryptionConfigurationProviderService>()
                    .LifestyleTransient());
        }

        private static void RegisterLearningStandardsSetupCommand(IWindsorContainer container)
        {
            container.Register(
                Component.For<IEnableLearningStandardsSetupCommand>()
                    .ImplementedBy<EnableLearningStandardsSetupCommand>()
                    .LifestyleTransient());

            container.Register(
                Component.For<ISetupAcademicBenchmarksConnectService>()
                    .ImplementedBy<SetupAcademicBenchmarksConnectService>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IModifyClaimSetsService>()
                    .ImplementedBy<ModifyClaimSetsService>()
                    .LifestyleTransient());
        }

        private static void RegisterAdminAppDbContext(IWindsorContainer container)
        {
            container.Register(
                Component.For<AdminAppDbContext>()
                    .ImplementedBy<AdminAppDbContext>()
                    .LifestylePerWebRequest());
        }

        private static void RegisterInstanceContext(IWindsorContainer container)
        {
            container.Register(
                Component.For<InstanceContext>()
                    .ImplementedBy<InstanceContext>()
                    .LifestylePerWebRequest());
        }

        private static void RegisterAdminAppUserContext(IWindsorContainer container)
        {
            container.Register(
                Component.For<AdminAppUserContext>()
                    .ImplementedBy<AdminAppUserContext>()
                    .LifestylePerWebRequest());
        }

        private static void RegisterApiModeProvider(IWindsorContainer container)
        {
            container.Register(
                Component.For<ICloudOdsAdminAppSettingsApiModeProvider>()
                    .ImplementedBy<CloudOdsAdminAppSettingsApiModeProvider>()
                    .LifestyleTransient());
        }

        private static void RegisterAppSettings(IWindsorContainer container)
        {
            container.Register(
                Component.For<IOptions<AppSettings>>()
                    .Instance(new Net48Options<AppSettings>(ConfigurationHelper.GetAppSettings()))
                    .LifestyleSingleton());
        }
    }
}
