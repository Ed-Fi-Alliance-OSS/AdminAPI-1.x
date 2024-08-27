namespace EdFi.Ods.AdminApi.Infrastructure;

public static class DatabaseEngineEnum
{
    public const string SqlServer = "SqlServer";
    public const string PostgreSql = "PostgreSql";
    public const string SqlServerCollation = "Latin1_General_100_BIN2_UTF8";

    public static string Parse(string value)
    {
        if (value.Equals(SqlServer, StringComparison.InvariantCultureIgnoreCase))
        {
            return SqlServer;
        }

        if (value.Equals(PostgreSql, StringComparison.InvariantCultureIgnoreCase))
        {
            return PostgreSql;
        }

        throw new NotSupportedException("Not supported DatabaseEngine \"" + value + "\". Supported engines: SqlServer, and PostgreSql.");
    }
}
