using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addedExtraSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "policies",
                columns: new[] { "id", "CreatedAt", "name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("067fa298-9ec1-4f73-f906-73fb05fc0777"), null, "roles:policies:remove", null },
                    { new Guid("178fb399-afd2-5074-0a07-82fc16fd1888"), null, "roles:inheritance:create", null },
                    { new Guid("289fc49a-c0e3-6185-1b18-91fd27fe2999"), null, "users:roles:read", null },
                    { new Guid("39afd59b-d1f4-7296-2c29-a20e380f3aaa"), null, "users:roles:assign", null },
                    { new Guid("4ab0e69c-e205-83a7-3d3a-b31f491045bb"), null, "users:policies:read", null },
                    { new Guid("5bc1f79d-f316-94b8-4e4b-c42f5a2156cc"), null, "users:policies:assign", null },
                    { new Guid("6cd2089e-0427-a5c9-5f5c-d53f6b3267dd"), null, "users:policies:remove", null },
                    { new Guid("7de3199f-1538-b6da-607d-e64f7c4378ee"), null, "policies:read", null },
                    { new Guid("8ef42aa0-2649-c7eb-718e-f75f8d5489ff"), null, "policies:create", null },
                    { new Guid("9ff53bb1-375a-d8fc-829f-08609e659a00"), null, "policies:update", null },
                    { new Guid("a1064cc2-486b-e90d-93a0-1971af76ab11"), null, "policies:delete", null },
                    { new Guid("a10b5c92-3e9b-4d8a-bf01-19d3f9b6f111"), null, "roles:read", null },
                    { new Guid("b21e5d93-4f7c-4d2e-a401-28a5f0c7e222"), null, "roles:create", null },
                    { new Guid("c32f6e94-5a8d-4d3f-b502-37c7f1d8f333"), null, "roles:update", null },
                    { new Guid("d43f7f95-6b9e-4e40-c603-46e8f2e9f444"), null, "roles:delete", null },
                    { new Guid("e54f8096-7caf-4f51-d704-55f9f3faf555"), null, "roles:policies:read", null },
                    { new Guid("f65f9197-8db0-4f62-e805-64faf4fbf666"), null, "roles:policies:assign", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("067fa298-9ec1-4f73-f906-73fb05fc0777"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("178fb399-afd2-5074-0a07-82fc16fd1888"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("289fc49a-c0e3-6185-1b18-91fd27fe2999"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("39afd59b-d1f4-7296-2c29-a20e380f3aaa"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("4ab0e69c-e205-83a7-3d3a-b31f491045bb"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("5bc1f79d-f316-94b8-4e4b-c42f5a2156cc"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("6cd2089e-0427-a5c9-5f5c-d53f6b3267dd"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("7de3199f-1538-b6da-607d-e64f7c4378ee"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("8ef42aa0-2649-c7eb-718e-f75f8d5489ff"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("9ff53bb1-375a-d8fc-829f-08609e659a00"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("a1064cc2-486b-e90d-93a0-1971af76ab11"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("a10b5c92-3e9b-4d8a-bf01-19d3f9b6f111"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("b21e5d93-4f7c-4d2e-a401-28a5f0c7e222"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("c32f6e94-5a8d-4d3f-b502-37c7f1d8f333"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("d43f7f95-6b9e-4e40-c603-46e8f2e9f444"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("e54f8096-7caf-4f51-d704-55f9f3faf555"));

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: new Guid("f65f9197-8db0-4f62-e805-64faf4fbf666"));
        }
    }
}
