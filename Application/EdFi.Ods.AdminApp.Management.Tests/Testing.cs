using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Web;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    public static class Testing
    {
        private static readonly IServiceScopeFactory ScopeFactory;

        static Testing()
        {
            var serviceProvider = Program.CreateHostBuilder(new string[] { })
                .ConfigureServices((context, services) =>
                {
                    // Test-specific IoC modifications here.
                    services.AddTransient<ITelemetry, StubTelemetry>();
                })
                .Build()
                .Services;

            ScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public static void EnsureInitialized()
        {
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
