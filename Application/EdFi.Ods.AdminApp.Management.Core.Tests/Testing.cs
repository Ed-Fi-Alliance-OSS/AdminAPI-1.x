using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Web;
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
                })
                .Build()
                .Services;

            ScopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
        }

        public static void EnsureInitialized()
        {
            ScopeFactory.ShouldNotBeNull();
        }

        public static void Scoped<TService>(Action<TService> action)
        {
            using (var scope = ScopeFactory.CreateScope())
                action(scope.ServiceProvider.GetService<TService>());
        }

        public static async Task ScopedAsync<TService>(Func<TService, Task> actionAsync)
        {
            using (var scope = ScopeFactory.CreateScope())
                await actionAsync(scope.ServiceProvider.GetService<TService>());
        }
    }
}
