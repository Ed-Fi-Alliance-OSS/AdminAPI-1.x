namespace EdFi.Ods.AdminApi.Helpers;
public class AppSettings
{
    private static AppSettings? _appSettings;
    public int DefaultPageSizeOffset { get; set; }
    public int DefaultPageSizeLimit { get; set; }

    public AppSettings(IConfiguration config)
    {
        this.DefaultPageSizeOffset = config.GetValue<int>("DefaultPageSizeOffset");
        this.DefaultPageSizeLimit = config.GetValue<int>("DefaultPageSizeLimit");
        _appSettings = this;
    }

    public static AppSettings Current
    {
        get
        {
            if (_appSettings == null) { _appSettings = GetCurrentSettings(); }
            return _appSettings;
        }
    }

    public static AppSettings GetCurrentSettings()
    {
        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

        IConfigurationRoot configuration = builder.Build();

        var settings = new AppSettings(configuration.GetSection("AppSettings"));

        return settings;
    }
}
