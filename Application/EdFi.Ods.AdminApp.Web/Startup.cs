// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity;
using System.Reflection;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.Automapper;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Web._Installers;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Hubs;
using EdFi.Ods.AdminApp.Web.Infrastructure.HangFire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging;

namespace EdFi.Ods.AdminApp.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var databaseEngine = Configuration["AppSettings:DatabaseEngine"];
            DbConfiguration.SetConfiguration(new DatabaseEngineDbConfiguration(databaseEngine));

            services.AddDbContext<AdminAppDbContext>(ConfigureForAdminDatabase);
            services.AddDbContext<AdminAppIdentityDbContext>(ConfigureForAdminDatabase);

            services.AddIdentity<AdminAppUser, IdentityRole>()
                .AddEntityFrameworkStores<AdminAppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews(options =>
                    {
                        options.Filters.Add(new AuthorizeFilter());
                        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                        options.Filters.Add<JsonValidationFilter>();
                        options.Filters.Add<HandleAjaxErrorAttribute>();
                        options.Filters.Add<SetupRequiredFilter>();
                        options.Filters.Add<UserContextFilter>();
                        options.Filters.Add<PasswordChangeRequiredFilter>();
                        options.Filters.Add<InstanceContextFilter>();
                    })
                    .AddFluentValidation(
                        opt =>
                        {
                            opt.RegisterValidatorsFromAssembly(executingAssembly);

                            opt.ValidatorOptions.DisplayNameResolver = (type, memberInfo, expression)
                                => memberInfo?
                                    .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();
                        });

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Login";
                options.LogoutPath = "/Identity/LogOut";
                options.AccessDeniedPath = "/Identity/Login";
            });

            services.AddAutoMapper(executingAssembly, typeof(AdminManagementMappingProfile).Assembly);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.AddSignalR();

            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);
            ConfigurationAppSettings = appSettings;

            var connectionStrings = new ConnectionStrings();
            Configuration.GetSection("ConnectionStrings").Bind(connectionStrings);
            ConfigurationConnectionStrings = connectionStrings;

            var appStartup = appSettings.AppStartup;

            if (appStartup == "OnPrem")
                new OnPremInstaller().Install(services, appSettings);
            else if (appStartup == "Azure")
                new AzureInstaller().Install(services, appSettings);


            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings());
            HangFireInstance.EnableWithoutSchemaMigration();
            services.AddHangfireServer();

            CommonConfigurationInstaller.ConfigureLearningStandards(services);
        }

        private void ConfigureForAdminDatabase(DbContextOptionsBuilder options)
        {
            var connectionString = Configuration.GetConnectionString("Admin");
            var databaseEngine = Configuration["AppSettings:DatabaseEngine"];

            if ("SqlServer".Equals(databaseEngine, StringComparison.InvariantCultureIgnoreCase))
                options.UseSqlServer(connectionString);
            else
                options.UseNpgsql(connectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var loggingOptions = Configuration.GetSection("Log4NetCore")
                .Get<Log4NetProviderOptions>();

            loggerFactory.AddLog4Net(loggingOptions);

            if (env.IsProduction())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ProductionLearningStandardsHub>("/productionLearningStandardsHub");
                endpoints.MapHub<BulkUploadHub>("/bulkUploadHub");
            });
        }

        public static AppSettings ConfigurationAppSettings { get; set; }
        public static ConnectionStrings ConfigurationConnectionStrings { get; set; }
    }
}
