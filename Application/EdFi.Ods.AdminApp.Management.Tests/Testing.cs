using System;
using System.Threading.Tasks;
using EdFi.Ods.Admin.Api;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    public static class Testing
    {
        public static readonly IServiceScopeFactory ScopeFactory;

        static Testing()
        {
            var serviceProvider = Program.StartApplication(Array.Empty<string>());

            ScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public static void EnsureInitialized()
        {
            var dbSetup = new SecurityTestDatabaseSetup();
            dbSetup.EnsureSecurityDatabase(@"C:\\temp");
            ScopeFactory.ShouldNotBeNull();
        }

        public static void Scoped<TService>(Action<TService> action)
        {
            using (var scope = ScopeFactory.CreateScope())
                action(scope.ServiceProvider.GetRequiredService<TService>());
        }

        public static TResult Scoped<TService, TResult>(Func<TService, TResult> func)
        {
            var result = default(TResult);

            Scoped<TService>(service =>
            {
                result = func(service);
            });

            return result;
        }

        public static async Task ScopedAsync<TService>(Func<TService, Task> actionAsync)
        {
            using (var scope = ScopeFactory.CreateScope())
                await actionAsync(scope.ServiceProvider.GetRequiredService<TService>());
        }

        public static async Task<TResult> ScopedAsync<TService, TResult>(Func<TService, Task<TResult>> actionAsync)
        {
            using (var scope = ScopeFactory.CreateScope())
                return await actionAsync(scope.ServiceProvider.GetRequiredService<TService>());
        }
    }
}
