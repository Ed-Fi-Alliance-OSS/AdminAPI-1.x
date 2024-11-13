using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.MsSql
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "adminconsole");

            migrationBuilder.CreateTable(
                name: "HealthChecks",
                schema: "adminconsole",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    EdOrgId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthChecks", x => x.DocId);
                });

            migrationBuilder.CreateTable(
                name: "Instances",
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
                    table.PrimaryKey("PK_Instances", x => x.DocId);
                });

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

            migrationBuilder.CreateTable(
                name: "Tenants",
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
                    table.PrimaryKey("PK_Tenants", x => x.DocId);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
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
                    table.PrimaryKey("PK_UserProfile", x => x.DocId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthChecks_EdOrgId",
                schema: "adminconsole",
                table: "HealthChecks",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthChecks_InstanceId",
                schema: "adminconsole",
                table: "HealthChecks",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthChecks_TenantId",
                schema: "adminconsole",
                table: "HealthChecks",
                column: "TenantId",
                unique: true);

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
                name: "IX_Permissions_EdOrgId",
                schema: "adminconsole",
                table: "Permissions",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_InstanceId",
                schema: "adminconsole",
                table: "Permissions",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_EdOrgId",
                schema: "adminconsole",
                table: "Tenants",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_InstanceId",
                schema: "adminconsole",
                table: "Tenants",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantId",
                schema: "adminconsole",
                table: "Tenants",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_EdOrgId",
                schema: "adminconsole",
                table: "UserProfile",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_InstanceId",
                schema: "adminconsole",
                table: "UserProfile",
                column: "InstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthChecks",
                schema: "adminconsole");

            migrationBuilder.DropTable(
                name: "Instances",
                schema: "adminconsole");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "adminconsole");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "adminconsole");

            migrationBuilder.DropTable(
                name: "UserProfile",
                schema: "adminconsole");
        }
    }
}
