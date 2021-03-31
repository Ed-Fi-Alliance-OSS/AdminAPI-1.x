using System.Reflection;

namespace EdFi.Ods.AdminApp.Management
{
    public static class Version
    {
        public static string InformationalVersion =>
            Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
}
