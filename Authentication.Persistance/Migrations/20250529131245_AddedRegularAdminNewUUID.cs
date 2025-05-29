using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedRegularAdminNewUUID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("c0cc1b7f-2705-49b9-b746-75a8dba9761d"));

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "CreatedAt", "name", "UpdatedAt" },
                values: new object[] { new Guid("33ee7453-b06c-4959-9377-badf265ee52d"), null, "Admin", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("33ee7453-b06c-4959-9377-badf265ee52d"));

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "CreatedAt", "name", "UpdatedAt" },
                values: new object[] { new Guid("c0cc1b7f-2705-49b9-b746-75a8dba9761d"), null, "Admin", null });
        }
    }
}
