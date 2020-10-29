// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.Automapper;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Web._Installers;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AspNetCore;
using Hangfire;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Mvc;

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

            // Ultimately, we wish to pivot AdminAppDbContext to an EF Core context
            // with setup similar to that of AdminAppIdentityDbContext below.
            // Until then, account for the lack of a .NET Framework-style app.config
            // file by providing the connection string directly.
            services.AddScoped(options =>
            {
                var connectionString = Configuration.GetConnectionString("Admin");
                return new AdminAppDbContext(connectionString);
            });

            services.AddDbContext<AdminAppIdentityDbContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("Admin");
                var databaseEngine = Configuration["AppSettings:DatabaseEngine"];

                if ("SqlServer".Equals(databaseEngine, StringComparison.InvariantCultureIgnoreCase))
                    options.UseSqlServer(connectionString);
                else
                    options.UseNpgsql(connectionString);
            });

            services.AddControllersWithViews(options =>
                    {
                        options.Filters.Add<JsonValidationFilter>();
                        options.Filters.Add<SetupRequiredFilter>();
                        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                    })
                    .AddFluentValidation(opt =>
                    {
                        opt.RegisterValidatorsFromAssembly(executingAssembly);
                    });

            services.AddAutoMapper(executingAssembly, typeof(AdminManagementMappingProfile).Assembly);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.AddSignalR();

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings());

            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);
            ConfigurationAppSettings = appSettings;

            var connectionStrings = new ConnectionStrings();
            Configuration.GetSection("ConnectionStrings").Bind(connectionStrings);
            ConfigurationConnectionStrings = connectionStrings;

            var appStartup = appSettings.AppStartup;

            if (appStartup == "OnPrem")
                new OnPremInstaller().Install(services);
            else if (appStartup == "Azure")
                new AzureInstaller().Install(services);
            CommonConfigurationInstaller.ConfigureLearningStandards(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureLogging()
        {
            var assembly = typeof(Program).GetTypeInfo().Assembly;

            var configPath = Path.Combine(Path.GetDirectoryName(assembly.Location) ?? string.Empty, Configuration["AppSettings:Log4NetConfigPath"]);

            XmlConfigurator.Configure(LogManager.GetRepository(assembly), new FileInfo(configPath));
        }

        public static AppSettings ConfigurationAppSettings { get; set; }
        public static ConnectionStrings ConfigurationConnectionStrings { get; set; }
    }
}
