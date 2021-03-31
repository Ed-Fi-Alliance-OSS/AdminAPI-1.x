using System.Reflection;

namespace EdFi.Ods.AdminApp.Management
{
    public static class Version
    {
        public static string InternalVersion =>
            Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
}
