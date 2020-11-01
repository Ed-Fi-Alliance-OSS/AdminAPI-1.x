using System;
using EdFi.Ods.AdminApp.Management.Database;

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
    }
}
