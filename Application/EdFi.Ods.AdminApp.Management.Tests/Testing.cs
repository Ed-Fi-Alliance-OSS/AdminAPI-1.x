using System;
using System.Threading.Tasks;
using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using EdFi.Security.DataAccess.Contexts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    public static class Testing
    {
        private static IMapper _mapper = AutoMapperBootstrapper.CreateMapper();

        public static TResult Scoped<TService, TResult>(Func<TService, TResult> func)
        {
            var result = default(TResult);

            Scoped<TService>(service =>
            {
                result = func(service);
            });

            return result;
        }

        public static void Scoped<TService>(Action<TService> action)
        {
            if (typeof(TService) == typeof(AdminAppIdentityDbContext))
            {
                using (var service = AdminAppIdentityDbContext.Create())
                    action((TService)(object)service);
            }
            else if (typeof(TService) == typeof(AdminAppDbContext))
            {
                using (var service = new AdminAppDbContext())
                    action((TService)(object)service);
            }
            else if (typeof(TService) == typeof(GetOdsInstanceRegistrationsByUserIdQuery))
            {
                using (var identity = AdminAppIdentityDbContext.Create())
                using (var database = new AdminAppDbContext())
                {
                    var service = new GetOdsInstanceRegistrationsByUserIdQuery(database, identity);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(EditOdsInstanceRegistrationForUserModelValidator))
            {
                using (var identity = AdminAppIdentityDbContext.Create())
                using (var database = new AdminAppDbContext())
                {
                    var service = new EditOdsInstanceRegistrationForUserModelValidator(database, identity);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(DeregisterOdsInstanceCommand))
            {
                using (var sqlServerUsersContext = new SqlServerUsersContext())
                using (var identity = AdminAppIdentityDbContext.Create())
                using (var database = new AdminAppDbContext())
                {
                    var service = new DeregisterOdsInstanceCommand(database, sqlServerUsersContext, identity);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(IGetClaimSetsByApplicationNameQuery))
            {
                using (var securityContext = new SqlServerSecurityContext())
                using (var usersContext = new SqlServerUsersContext())
                {
                    var service = new GetClaimSetsByApplicationNameQuery(securityContext, usersContext);
                    action((TService) (object) service);
                }
            }
            else if (typeof(TService) == typeof(IGetApplicationsByClaimSetIdQuery))
            {
                using (var securityContext = new SqlServerSecurityContext())
                using (var usersContext = new SqlServerUsersContext())
                {
                    var service = new GetApplicationsByClaimSetIdQuery(securityContext, usersContext);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(IGetClaimSetByIdQuery))
            {
                using (var securityContext = new SqlServerSecurityContext())
                {
                    var service = new GetClaimSetByIdQuery(securityContext);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(IGetResourcesByClaimSetIdQuery))
            {
                using (var securityContext = new SqlServerSecurityContext())
                {
                    var service = new GetResourcesByClaimSetIdQuery(securityContext, _mapper);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(ClaimSetFileImportCommand))
            {
                using (var securityContext = new SqlServerSecurityContext())
                {
                    var addClaimSetCommand = new AddClaimSetCommand(securityContext);
                    var getResourceClaimsQuery = new GetResourceClaimsQuery(securityContext);
                    var editResourceOnClaimSetCommand = new EditResourceOnClaimSetCommand(securityContext);
                    var service = new ClaimSetFileImportCommand(addClaimSetCommand, editResourceOnClaimSetCommand, getResourceClaimsQuery);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(ClaimSetFileExportCommand))
            {
                using (var securityContext = new SqlServerSecurityContext())
                {
                    var getResourceByClaimSetIdQuery = new GetResourcesByClaimSetIdQuery(securityContext, _mapper);
                    var service = new ClaimSetFileExportCommand(securityContext, getResourceByClaimSetIdQuery);
                    action((TService)(object)service);
                }
            }
            else if (typeof(TService) == typeof(IUsersContext))
            {
                using (var service = new SqlServerUsersContext())
                    action((TService)(object)service);
            }
            else if (typeof(TService) == typeof(ISecurityContext))
            {
                using (var service = new SqlServerSecurityContext())
                    action((TService)(object)service);
            }
            else
            {
                throw new NotSupportedException($"In NET48 test runs Scoped<{typeof(TService).Name}>(...) is not supported.");
            }
        }

        public static async Task ScopedAsync<TService>(Func<TService, Task> actionAsync)
        {
            if (typeof(TService) == typeof(AdminAppIdentityDbContext))
            {
                using (var service = AdminAppIdentityDbContext.Create())
                    await actionAsync((TService)(object)service);
            }
            else if (typeof(TService) == typeof(AdminAppDbContext))
            {
                using (var service = new AdminAppDbContext())
                    await actionAsync((TService)(object)service);
            }
            else if (typeof(TService) == typeof(UserManager<AdminAppUser>))
            {
                using (var identity = AdminAppIdentityDbContext.Create())
                using (var userStore = new UserStore<AdminAppUser>(identity))
                using (var service = new UserManager<AdminAppUser>(userStore))
                    await actionAsync((TService)(object)service);
            }
            else if (typeof(TService) == typeof(IUsersContext))
            {
                using (var service = new SqlServerUsersContext())
                    await actionAsync((TService)(object)service);
            }
            else
            {
                throw new NotSupportedException($"In NET48 test runs ScopedAsync<{typeof(TService).Name}>(...) is not supported.");
            }
        }

        public static async Task<TResult> ScopedAsync<TService, TResult>(Func<TService, Task<TResult>> actionAsync)
        {
            if (typeof(TService) == typeof(AdminAppIdentityDbContext))
            {
                using (var service = AdminAppIdentityDbContext.Create())
                    return await actionAsync((TService)(object)service);
            }
            else if (typeof(TService) == typeof(AdminAppDbContext))
            {
                using (var service = new AdminAppDbContext())
                    return await actionAsync((TService)(object)service);
            }
            else
            {
                throw new NotSupportedException($"In NET48 test runs ScopedAsync<{typeof(TService).Name}, {typeof(TResult).Name}>(...) is not supported.");
            }
        }
    }
}
