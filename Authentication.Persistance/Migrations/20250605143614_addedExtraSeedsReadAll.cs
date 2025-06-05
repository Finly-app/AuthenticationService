using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addedExtraSeedsReadAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "policies",
                columns: new[] { "id", "CreatedAt", "name", "UpdatedAt" },
                values: new object[] { new Guid("2666478b-d202-412e-b166-392499b06c95"), null, "users:read:all", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("2666478b-d202-412e-b166-392499b06c95"));
        }
    }
}
