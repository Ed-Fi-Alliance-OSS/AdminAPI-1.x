using System.Reflection;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.Admin.Api.Features.Connect;
using EdFi.Ods.Admin.Api.Infrastructure.Security;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EdFi.Ods.Admin.Api.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
    {
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        webApplicationBuilder.Services.AddEndpointsApiExplorer();
        webApplicationBuilder.Services.AddSwaggerGen(opt => opt.CustomSchemaIds(x => x.FullName));

        // Logging
        var loggingOptions = webApplicationBuilder.Configuration.GetSection("Log4NetCore").Get<Log4NetProviderOptions>();
        webApplicationBuilder.Logging.AddLog4Net(loggingOptions);

        // Fluent validation
        var executingAssembly = Assembly.GetExecutingAssembly();
        webApplicationBuilder.Services.AddFluentValidation(
            opt =>
            {
                opt.RegisterValidatorsFromAssembly(executingAssembly);

                opt.ValidatorOptions.DisplayNameResolver = (type, memberInfo, expression)
                    => memberInfo?
                        .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();
            });

        //Databases
        var databaseEngine = webApplicationBuilder.Configuration["AppSettings:DatabaseEngine"];
        webApplicationBuilder.AddDatabases(databaseEngine);

        //OpenIddict Auth
        webApplicationBuilder.Services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore().UseDbContext<AdminApiDbContext>()
                    .ReplaceDefaultEntities<ApiApplication, ApiAuthorization, ApiScope, ApiToken, int>();
            })
            .AddServer(opt =>
            {
                opt.AllowClientCredentialsFlow();

                opt.SetTokenEndpointUris("/connect/token");

                opt.AddEphemeralEncryptionKey();
                opt.AddEphemeralSigningKey();

                opt.RegisterScopes("edfi_admin_api/full_access");
                opt.UseAspNetCore().EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        var issuer = webApplicationBuilder.Configuration.GetValue<string>("Authentication:IssuerUrl");
        webApplicationBuilder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.Authority = issuer;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                };
            });
        webApplicationBuilder.Services.AddAuthorization(opt =>
        {
            opt.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim("scope", "edfi_adminapi/full_access")
                .Build();
        });
		
		webApplicationBuilder.Services.AddTransient<ITokenService, TokenService>();
        webApplicationBuilder.Services.AddControllers();
    }

    private static void AddDatabases(this WebApplicationBuilder webApplicationBuilder, string databaseEngine)
    {
        var adminConnectionString = webApplicationBuilder.Configuration.GetConnectionString("Admin");
        var securityConnectionString = webApplicationBuilder.Configuration.GetConnectionString("Security");

        if (DatabaseEngineEnum.Parse(databaseEngine).Equals(DatabaseEngineEnum.PostgreSql))
        {
            webApplicationBuilder.Services.AddDbContext<AdminAppDbContext>(
                options => options.UseNpgsql(adminConnectionString));

            webApplicationBuilder.Services.AddScoped<ISecurityContext>(
                sp => new PostgresSecurityContext(securityConnectionString));

            webApplicationBuilder.Services.AddScoped<IUsersContext>(
                sp => new PostgresUsersContext(adminConnectionString));
        }
        else if (DatabaseEngineEnum.Parse(databaseEngine).Equals(DatabaseEngineEnum.SqlServer))
        {
            webApplicationBuilder.Services.AddDbContext<AdminAppDbContext>(
                options => options.UseSqlServer(adminConnectionString));

            webApplicationBuilder.Services.AddDbContext<AdminApiDbContext>(
                options =>
                {
                    options.UseSqlServer(adminConnectionString);
                    options.UseOpenIddict<ApiApplication, ApiAuthorization, ApiScope, ApiToken, int>();
                });

            webApplicationBuilder.Services.AddScoped<ISecurityContext>(
                sp => new SqlServerSecurityContext(securityConnectionString));

            webApplicationBuilder.Services.AddScoped<IUsersContext>(
                sp => new SqlServerUsersContext(adminConnectionString));
        }
    }
}
