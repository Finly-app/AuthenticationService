using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedRegularAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "CreatedAt", "name", "UpdatedAt" },
                values: new object[] { new Guid("c0cc1b7f-2705-49b9-b746-75a8dba9761d"), null, "Admin", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("c0cc1b7f-2705-49b9-b746-75a8dba9761d"));
        }
    }
}
