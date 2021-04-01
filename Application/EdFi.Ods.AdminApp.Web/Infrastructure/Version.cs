using System.Reflection;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public static class Version
    {
        public static string InformationalVersion =>
            Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
}
