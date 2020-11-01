using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
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
    }
}
