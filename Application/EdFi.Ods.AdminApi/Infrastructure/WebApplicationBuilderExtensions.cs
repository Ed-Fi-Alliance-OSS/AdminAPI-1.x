// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Reflection;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.DbConfigurations;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using EdFi.Ods.AdminApi.Infrastructure.Security;
using EdFi.Ods.AdminApi.Infrastructure.Api;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Ods.AdminApi.Infrastructure.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using FluentValidation;
using EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;
using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Context;

namespace EdFi.Ods.AdminApi.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.Configure<AppSettings>(webApplicationBuilder.Configuration.GetSection("AppSettings"));
        EnableMultiTenancySupport(webApplicationBuilder);
        var executingAssembly = Assembly.GetExecutingAssembly();
        webApplicationBuilder.Services.AddAutoMapper(executingAssembly, typeof(AdminApiMappingProfile).Assembly);
        webApplicationBuilder.Services.AddScoped<InstanceContext>();

        foreach (var type in typeof(IMarkerForEdFiOdsAdminApiManagement).Assembly.GetTypes())
        {
            if (type.IsClass && !type.IsAbstract && (type.IsPublic || type.IsNestedPublic))
            {
                var concreteClass = type;

                var interfaces = concreteClass.GetInterfaces().ToArray();

                if (concreteClass.Namespace != null)
                {
                    if (concreteClass.Namespace.EndsWith("Database.Commands") || concreteClass.Namespace.EndsWith("Database.Queries")
                        || concreteClass.Namespace.EndsWith("ClaimSetEditor"))
                    {
                        if (interfaces.Length == 1)
                        {
                            var serviceType = interfaces.Single();
                            if (serviceType.FullName == $"{concreteClass.Namespace}.I{concreteClass.Name}")
                                webApplicationBuilder.Services.AddTransient(serviceType, concreteClass);
                        }
                        else if (interfaces.Length == 0)
                        {
                            if (concreteClass.Name.EndsWith("Command")
                              || concreteClass.Name.EndsWith("Query")
                              || concreteClass.Name.EndsWith("Service"))
                            {
                                webApplicationBuilder.Services.AddTransient(concreteClass);
                            }
                        }
                    }
                }
            }
        }

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        webApplicationBuilder.Services.AddEndpointsApiExplorer();
        webApplicationBuilder.Services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = false;
        });

        webApplicationBuilder.Services.Configure<SwaggerSettings>(webApplicationBuilder.Configuration.GetSection("SwaggerSettings"));
        var issuer = webApplicationBuilder.Configuration.GetValue<string>("Authentication:IssuerUrl");
        webApplicationBuilder.Services.AddSwaggerGen(opt =>
        {
            opt.CustomSchemaIds(x => x.FullName?.Replace("+", "."));
            opt.OperationFilter<TokenEndpointBodyDescriptionFilter>();
            opt.OperationFilter<TagByResourceUrlFilter>();
            opt.AddSecurityDefinition(
                "oauth",
                new OpenApiSecurityScheme
                {
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{issuer}/{SecurityConstants.TokenEndpoint}"),
                            Scopes = new Dictionary<string, string>
                            {
                                { SecurityConstants.Scopes.AdminApiFullAccess, "Unrestricted access to all Admin API endpoints" },
                            }
                        },
                    },
                    In = ParameterLocation.Header,
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.OAuth2
                }
            );
            opt.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                                { Type = ReferenceType.SecurityScheme, Id = "oauth" },
                        },
                        new[] { "api" }
                    }
                }
            );

            foreach (var version in AdminApiVersions.GetAllVersionStrings())
            {
                opt.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = "Admin API Documentation", Version = version
                });
            }
            opt.DocumentFilter<ListExplicitSchemaDocumentFilter>();
            opt.SchemaFilter<SwaggerOptionalSchemaFilter>();
            opt.SchemaFilter<SwaggerExcludeSchemaFilter>();
            opt.OperationFilter<SwaggerDefaultParameterFilter>();
            opt.OperationFilter<ProfileRequestExampleFilter>();
            opt.EnableAnnotations();
            opt.OrderActionsBy(x =>
            {
                return x.HttpMethod != null && Enum.TryParse<HttpVerbOrder>(x.HttpMethod, out var verb)
                    ? ((int)verb).ToString()
                    : int.MaxValue.ToString();
            });
        });

        // Logging
        var loggingOptions = webApplicationBuilder.Configuration.GetSection("Log4NetCore").Get<Log4NetProviderOptions>();
        webApplicationBuilder.Logging.AddLog4Net(loggingOptions);

        // Fluent validation
        webApplicationBuilder.Services
            .AddValidatorsFromAssembly(executingAssembly)
            .AddFluentValidationAutoValidation();
        ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression)
                    => memberInfo?
                        .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();

        //Databases
        var databaseEngine = webApplicationBuilder.Configuration["AppSettings:DatabaseEngine"];
        webApplicationBuilder.AddDatabases(databaseEngine);

        //Health
        webApplicationBuilder.Services.AddHealthCheck(webApplicationBuilder.Configuration);

        //JSON
        webApplicationBuilder.Services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.WriteIndented = true;
        });

        webApplicationBuilder.Services.AddSecurityUsingOpenIddict(webApplicationBuilder.Configuration, webApplicationBuilder.Environment);

        webApplicationBuilder.Services.AddHttpClient();
        webApplicationBuilder.Services.AddTransient<ISimpleGetRequest, SimpleGetRequest>();
        webApplicationBuilder.Services.AddTransient<IOdsApiValidator, OdsApiValidator>();
    }

    private static void EnableMultiTenancySupport(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddTransient<ITenantConfigurationProvider, TenantConfigurationProvider>();
        webApplicationBuilder.Services.AddTransient<IContextProvider<TenantConfiguration>, ContextProvider<TenantConfiguration>>();
        webApplicationBuilder.Services.AddSingleton<IContextStorage, HashtableContextStorage>();
        webApplicationBuilder.Services.AddScoped<TenantResolverMiddleware>();
        webApplicationBuilder.Services.Configure<TenantsSection>(webApplicationBuilder.Configuration);
    }

    private static void AddDatabases(this WebApplicationBuilder webApplicationBuilder, string databaseEngine)
    {
        var multiTenancyEnabled = webApplicationBuilder.Configuration.GetValue<bool>("AppSettings:MultiTenancy");

        if (DatabaseEngineEnum.Parse(databaseEngine).Equals(DatabaseEngineEnum.PostgreSql))
        {
            DbConfiguration.SetConfiguration(new DatabaseEngineDbConfiguration(Common.Configuration.DatabaseEngine.Postgres));

            webApplicationBuilder.Services.AddDbContext<AdminApiDbContext>(
            (sp, options) =>
            {
                options.UseNpgsql(AdminConnectionString(sp));
                options.UseOpenIddict<ApiApplication, ApiAuthorization, ApiScope, ApiToken, int>();
            });

            webApplicationBuilder.Services.AddScoped<ISecurityContext>(
                sp => new PostgresSecurityContext(SecurityConnectionString(sp)));

            webApplicationBuilder.Services.AddScoped<IUsersContext>(
                sp => new PostgresUsersContext(AdminConnectionString(sp)));            
        }
        else if (DatabaseEngineEnum.Parse(databaseEngine).Equals(DatabaseEngineEnum.SqlServer))
        {
            DbConfiguration.SetConfiguration(new DatabaseEngineDbConfiguration(Common.Configuration.DatabaseEngine.SqlServer));

            webApplicationBuilder.Services.AddDbContext<AdminApiDbContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(AdminConnectionString(sp));
                    options.UseOpenIddict<ApiApplication, ApiAuthorization, ApiScope, ApiToken, int>();
                });

            webApplicationBuilder.Services.AddScoped<ISecurityContext>(
                (sp) => new SqlServerSecurityContext(SecurityConnectionString(sp)));

            webApplicationBuilder.Services.AddScoped<IUsersContext>(
                (sp) => new SqlServerUsersContext(AdminConnectionString(sp)));          
        }
        else
        {
            throw new Exception($"Unexpected DB setup error. Engine '{databaseEngine}' was parsed as valid but is not configured for startup.");
        }

        string AdminConnectionString(IServiceProvider serviceProvider)
        {
            var adminConnectionString = string.Empty;

            if (multiTenancyEnabled)
            {
                var tenant = serviceProvider.GetRequiredService<IContextProvider<TenantConfiguration>>().Get();
                if (tenant != null && !string.IsNullOrEmpty(tenant.AdminConnectionString))
                {
                    adminConnectionString = tenant.AdminConnectionString;
                }
                else
                {
                    throw new Exception($"Admin database connection setup error. Tenant not configured correctly.");
                }
            }
            else
            {
                adminConnectionString = webApplicationBuilder.Configuration.GetConnectionString("EdFi_Admin");
            }

            return adminConnectionString;
        }

        string SecurityConnectionString(IServiceProvider serviceProvider)
        {
            var securityConnectionString = string.Empty;

            if (multiTenancyEnabled)
            {
                var tenant = serviceProvider.GetRequiredService<IContextProvider<TenantConfiguration>>().Get();
                if (tenant != null && !string.IsNullOrEmpty(tenant.SecurityConnectionString))
                {
                    securityConnectionString = tenant.SecurityConnectionString;
                }
                else
                {
                    throw new Exception($"Security database connection setup error. Tenant not configured correctly.");
                }
            }
            else
            {
                securityConnectionString = webApplicationBuilder.Configuration.GetConnectionString("EdFi_Security");
            }

            return securityConnectionString;
        }
    }

    private enum HttpVerbOrder
    {
        GET = 1,
        POST = 2,
        PUT = 3,
        DELETE = 4,
    }
}
