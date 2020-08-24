// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IdentityModel.Claims;
using System.Net;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Admin.LearningStandards.Core.Configuration;
using EdFi.Admin.LearningStandards.Core.Services;
using EdFi.Admin.LearningStandards.Core.Services.Interfaces;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.App_Start;
using EdFi.Ods.AdminApp.Web.Infrastructure.HangFire;
using EdFi.Ods.AdminApp.Web.Infrastructure.IoC;
using EdFi.Ods.Common.ChainOfResponsibility;
using EdFi.Ods.Common.InversionOfControl;
using FluentValidation.Mvc;
using Hangfire;
using Hangfire.Windsor;
using Microsoft.Extensions.DependencyInjection;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using log4net;

namespace EdFi.Ods.AdminApp.Web
{
    public abstract class StartupBase : IDisposable
    {
        protected readonly IWindsorContainer Container = new WindsorContainerEx();
        private static ILog Logger;

        public virtual void Configuration(IAppBuilder appBuilder)
        {
            ConfigureLogging(appBuilder);
            Logger = LogManager.GetLogger(typeof(StartupBase));
            try
            {
                InitializeContainer(Container);
                InstallConfigurationSpecificInstaller(Container);
                FinalizeContainer(Container);

                ConfigureAspNetIdentityAuthentication(appBuilder);

                ConfigureSignalR(appBuilder);
                ConfigureHangfire(appBuilder);

                ConfigureAspNet(appBuilder);

                ConfigureTls();

                ConfigureLearningStandards();
            }
            catch (Exception e)
            {
                Logger.Error("Startup Failed", e);
                throw;
            }            
        }

        private IServiceProvider ServiceProviderFunc(IServiceCollection collection)
        {
            return collection.BuildServiceProvider();
        }

        private void ConfigureLearningStandards()
        {
            var config = new EdFiOdsApiClientConfiguration(
                maxSimultaneousRequests: GetLearningStandardsMaxSimultaneousRequests());

            var serviceCollection = new ServiceCollection();

            var pluginConnector = new LearningStandardsCorePluginConnector(
                serviceCollection,
                ServiceProviderFunc,
                new LearningStandardLogProvider(),
                config
            );

            Container.Register(Component.For<ILearningStandardsCorePluginConnector>().Instance(pluginConnector));
        }

        private static int GetLearningStandardsMaxSimultaneousRequests()
        {
            const int IdealSimultaneousRequests = 4;
            const int PessimisticSimultaneousRequests = 1;

            try
            {
                var odsApiVersion = new InferOdsApiVersion().Version(CloudOdsAdminAppSettings.Instance.ProductionApiUrl);

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

        private static void InitializeContainer(IWindsorContainer container)
        {
            container.AddFacility<ChainOfResponsibilityFacility>();
        }

        private static void FinalizeContainer(IWindsorContainer container)
        {
            container.Kernel.GetFacilities().OfType<ChainOfResponsibilityFacility>().SingleOrDefault()?.FinalizeChains();
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

        protected abstract void InstallConfigurationSpecificInstaller(IWindsorContainer container);

        protected virtual void ConfigureLogging(IAppBuilder appBuilder)
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        protected virtual void ConfigureAspNet(IAppBuilder appBuilder)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, Container);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            ModelBinderConfig.RegisterModelBinder();

            DependencyResolver.SetResolver(new WindsorDependencyResolver(Container));

            var controllerFactory = new CastleWindsorControllerFactory(Container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            var fluentValidationModelValidatorProvider = new FluentValidationModelValidatorProvider(new CastleWindsorFluentValidatorFactory(Container.Kernel));
            ModelValidatorProviders.Providers.Add(fluentValidationModelValidatorProvider);
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }

        protected virtual void ConfigureSignalR(IAppBuilder appBuilder)
        {
            appBuilder.MapSignalR();
        }

        protected virtual void ConfigureHangfire(IAppBuilder appBuilder)
        {
            GlobalConfiguration.Configuration.UseActivator(new WindsorJobActivator(Container.Kernel));

            //Before running Admin App, we run a set of sql scripts using DbDeploy,
            //one of which creates the Hangfire tables under the special adminapp_Hangfire
            //schema. Since we can safely assume these tables will exist, we can enable
            //Hangfire without having to perform the schema initialization

            HangFireInstance.EnableWithoutSchemaMigration();
            appBuilder.UseHangfireServer();
        }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAspNetIdentityAuthentication(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(AdminAppIdentityDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Identity/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AdminAppUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: ApplicationUserManager.GenerateUserIdentityAsync)
                }
            });
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
