// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.MsSql
{
    /// <inheritdoc />
    public partial class InstanceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "adminconsole");

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
