using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class user_id_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "users",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "user_id");
        }
    }
}
