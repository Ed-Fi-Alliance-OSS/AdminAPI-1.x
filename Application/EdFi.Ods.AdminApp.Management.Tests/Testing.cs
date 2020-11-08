using System;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    public static class Testing
    {
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
            else if (typeof(TService) == typeof(IUsersContext))
            {
                using (var service = new SqlServerUsersContext())
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
