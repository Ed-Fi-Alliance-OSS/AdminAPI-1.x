using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.Mssql
{
    /// <inheritdoc />
    public partial class PermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "adminconsole",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    EdOrgId = table.Column<int>(type: "int", nullable: true),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.DocId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_EdOrgId",
                schema: "adminconsole",
                table: "Permissions",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_InstanceId",
                schema: "adminconsole",
                table: "Permissions",
                column: "InstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "adminconsole");
        }
    }
}
