// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Reflection;
using EdFi.Common.Extensions;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AspNetCore;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging;
using NUglify.Css;
using NUglify.JavaScript;

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

            if(databaseEngine.EqualsIgnoreCase("PostgreSql"))
            {
                DbConfiguration.SetConfiguration(new PostgreSqlDbConfiguration());
            }
            else
            {
                DbConfiguration.SetConfiguration(
                    new Admin.DataAccess.DbConfigurations
                        .DatabaseEngineDbConfiguration(Common.Configuration.DatabaseEngine.SqlServer));

            }

       
            services.AddControllersWithViews(options =>
                    {
                        options.Filters.Add(new AuthorizeFilter("UserMustExistPolicy"));
                        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                       })
                    .AddFluentValidation(
                        opt =>
                        {
                            opt.RegisterValidatorsFromAssembly(executingAssembly);

                            opt.ValidatorOptions.DisplayNameResolver = (type, memberInfo, expression)
                                => memberInfo?
                                    .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();
                        });

            services.AddWebOptimizer(
                pipeline =>
                {
                    var minifyJsSettings = new CodeSettings
                    {
                        LocalRenaming = LocalRenaming.CrunchAll,
                        MinifyCode = true
                    };

                    var minifyCssSettings = new CssSettings
                    {
                        MinifyExpressions = true
                    };

                    pipeline.AddCssBundle("/bundles/bootstrap-multiselect.min.css", minifyCssSettings, "/content/css/bootstrap-multiselect.css");
                    pipeline.AddCssBundle("/bundles/site.min.css", minifyCssSettings, "/content/css/Site.css");
                    pipeline.AddJavaScriptBundle("/bundles/bootstrap-multiselect.min.js", minifyJsSettings, "/Scripts/bootstrap-multiselect.js");
                    pipeline.AddJavaScriptBundle("/bundles/modernizr.min.js", minifyJsSettings, "/Scripts/modernizr-2.8.3.js");
                    pipeline.AddJavaScriptBundle("/bundles/site.min.js", minifyJsSettings, "/Scripts/site.js", "/Scripts/site-form-handlers.js", "/Scripts/signalr-progress.js");
                    pipeline.AddJavaScriptBundle("/bundles/claimset.min.js", minifyJsSettings, "/Scripts/resource-editor.js");
                    pipeline.AddJavaScriptBundle("/bundles/authstrategy.min.js", minifyJsSettings, "/Scripts/auth-editor.js");
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Login";
                options.LogoutPath = "/Identity/LogOut";
                options.AccessDeniedPath = "/Identity/Login";
            });

            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.AddSignalR();

            services.AddHttpClient();

      
            var connectionStrings = new ConnectionStrings();
            Configuration.GetSection("ConnectionStrings").Bind(connectionStrings);
            ConfigurationConnectionStrings = connectionStrings;

        
    
            services.AddHealthCheck(Configuration.GetConnectionString("Admin"), IsSqlServer(databaseEngine));

            }

        private void ConfigureForAdminDatabase(DbContextOptionsBuilder options)
        {
            var connectionString = Configuration.GetConnectionString("Admin");
            var databaseEngine = Configuration["AppSettings:DatabaseEngine"];

            if (IsSqlServer(databaseEngine))
                options.UseSqlServer(connectionString);
            else
                options.UseNpgsql(connectionString);
        }

        private static bool IsSqlServer(string databaseEngine) => "SqlServer".Equals(databaseEngine, StringComparison.InvariantCultureIgnoreCase);

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var loggingOptions = Configuration.GetSection("Log4NetCore")
                .Get<Log4NetProviderOptions>();

            var pathBase = Configuration.GetValue<string>("AppSettings:PathBase");
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase("/" + pathBase.Trim('/'));
            }

            loggerFactory.AddLog4Net(loggingOptions);

            if (env.IsProduction())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebOptimizer();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
      
                if (!env.IsProduction())
                {
                    endpoints.MapPost(
                        "/development-product-registration-sink", async context =>
                        {
                            using var reader = new StreamReader(context.Request.Body);

                            var body = await reader.ReadToEndAsync();

                            var logger = LogManager.GetLogger(typeof(Startup));
                            logger.Debug($"Development Product Registration Sink Received Message: {Environment.NewLine}{body}");

                   
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                        });
                }

                endpoints.MapHealthChecks("/health");
            });
        }

        public static ConnectionStrings ConfigurationConnectionStrings { get; set; }
    }
}
