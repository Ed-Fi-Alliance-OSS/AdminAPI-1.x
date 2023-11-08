namespace EdFi.Ods.AdminApi.Helpers;
public class AppSettings
{
    public int DefaultPageSizeOffset { get; set; }
    public int DefaultPageSizeLimit { get; set; }
    public string? DatabaseEngine { get; set; }
    public bool MultiTenancy { get; set; }
}
