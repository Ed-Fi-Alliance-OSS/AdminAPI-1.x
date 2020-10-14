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
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Services;
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
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IFileUploadHandler>()
                    .ImplementedBy<LocalFileSystemFileUploadHandler>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IMapper>()
                    .Instance(AutoMapperBootstrapper.CreateMapper())
                    .LifestyleSingleton());

            container.Register(
                Component.For<IGetVendorByIdQuery>()
                    .ImplementedBy<GetVendorByIdQuery>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IGetVendorsQuery>()
                    .ImplementedBy<GetVendorsQuery>()
                    .LifestyleTransient());

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

            container.Register(
                Component.For<TokenCache>()
                    .Instance(TokenCache.DefaultShared));

            container.Register(
                Component.For<AdminAppDbContext>()
                    .ImplementedBy<AdminAppDbContext>()
                    .LifestylePerWebRequest());

            container.Register(
                Component.For<AdminAppUserContext>()
                    .ImplementedBy<AdminAppUserContext>()
                    .LifestylePerWebRequest());

            container.Register(
                Component.For<ICloudOdsAdminAppSettingsApiModeProvider>()
                    .ImplementedBy<CloudOdsAdminAppSettingsApiModeProvider>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IOptions<AppSettings>>()
                    .Instance(new Net48Options<AppSettings>(ConfigurationHelper.GetAppSettings()))
                    .LifestyleSingleton());

            container.Register(
                Component.For<ICachedItems>()
                    .ImplementedBy<CachedItems>()
                    .LifestyleSingleton());

            container.Register(
                Component.For<IOdsApiConnectionInformationProvider>()
                    .ImplementedBy<CloudOdsApiConnectionInformationProvider>()
                    .LifestyleTransient());

            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IController>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IEncryptionConfigurationProviderService>()
                    .ImplementedBy<EncryptionConfigurationProviderService>()
                    .LifestyleTransient());

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

            container.Register(
                Component.For<InstanceContext>()
                    .ImplementedBy<InstanceContext>()
                    .LifestylePerWebRequest());

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

            container.Register(
                Classes.FromAssemblyContaining<IMarkerForEdFiOdsAdminAppManagement>()
                    .Pick()
                    .WithServiceAllInterfaces()
                    .LifestyleTransient());

            container.Register(
                Component.For<CloudOdsUpdateService>()
                    .ImplementedBy<CloudOdsUpdateService>()
                    .LifestyleSingleton());

            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn(typeof(IValidator<>))
                    .WithService
                    .Base()
                    .LifestyleTransient());
        }

        protected abstract void InstallHostingSpecificClasses(IWindsorContainer container);
    }
}
