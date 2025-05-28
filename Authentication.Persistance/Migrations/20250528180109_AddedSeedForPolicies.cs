using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeedForPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "policies",
                columns: new[] { "id", "CreatedAt", "name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("321aa912-e34e-4f41-9dbf-f5f61a3951f2"), null, "users:delete", null },
                    { new Guid("3fbed1fd-75e7-42a9-b017-8a2be84c81f1"), null, "users:read", null },
                    { new Guid("9a308869-5e11-4480-916b-cef3908797dc"), null, "users:create", null },
                    { new Guid("cdd60f4c-521f-4a45-8b87-1b84b69d49c1"), null, "users:update", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("321aa912-e34e-4f41-9dbf-f5f61a3951f2"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("3fbed1fd-75e7-42a9-b017-8a2be84c81f1"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("9a308869-5e11-4480-916b-cef3908797dc"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("cdd60f4c-521f-4a45-8b87-1b84b69d49c1"));
        }
    }
}
