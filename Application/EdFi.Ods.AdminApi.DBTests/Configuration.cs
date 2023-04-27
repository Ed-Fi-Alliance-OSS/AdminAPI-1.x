using Microsoft.Extensions.Configuration;


namespace EdFi.Ods.Admin.Api.Tests;

public static class Config
{
    private static IConfigurationRoot _config;

    public static IConfiguration Configuration()
    {
        if (_config == null)
        {
            _config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

        }
        return _config;
    }

    public static string AdminConnectionString {  get { return Configuration().GetConnectionString("Admin");  } }

    public static string SecurityConnectionString { get { return Configuration().GetConnectionString("Security"); } }

    public static string SecurityV53ConnectionString { get { return Configuration().GetConnectionString("SecurityV53"); } }
}
