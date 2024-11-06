using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.PgSql
{
    /// <inheritdoc />
    public partial class InstanceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instances",
                schema: "adminconsole",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstanceId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    EdOrgId = table.Column<int>(type: "integer", nullable: true),
                    Document = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instances", x => x.DocId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instances_EdOrgId",
                schema: "adminconsole",
                table: "Instances",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Instances_InstanceId",
                schema: "adminconsole",
                table: "Instances",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Instances_TenantId",
                schema: "adminconsole",
                table: "Instances",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instances",
                schema: "adminconsole");
        }
    }
}
