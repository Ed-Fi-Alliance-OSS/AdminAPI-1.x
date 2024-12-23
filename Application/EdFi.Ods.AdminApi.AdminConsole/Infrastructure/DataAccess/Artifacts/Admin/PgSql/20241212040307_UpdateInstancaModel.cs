using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.PgSql
{
    /// <inheritdoc />
    public partial class UpdateInstancaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstanceId",
                schema: "adminconsole",
                table: "Instances",
                newName: "OdsInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Instances_InstanceId",
                schema: "adminconsole",
                table: "Instances",
                newName: "IX_Instances_OdsInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OdsInstanceId",
                schema: "adminconsole",
                table: "Instances",
                newName: "InstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Instances_OdsInstanceId",
                schema: "adminconsole",
                table: "Instances",
                newName: "IX_Instances_InstanceId");
        }
    }
}
