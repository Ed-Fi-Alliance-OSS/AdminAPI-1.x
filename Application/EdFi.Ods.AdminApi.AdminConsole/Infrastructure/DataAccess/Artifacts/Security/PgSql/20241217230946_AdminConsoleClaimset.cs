using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.Security.PgSql
{
    /// <inheritdoc />
    public partial class AdminConsoleClaimset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure/DataAccess/Artifacts/Security/PgSql/SQL/AdminConsoleClaimsetUp.sql");
            var sqlScript = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(sqlScript);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
