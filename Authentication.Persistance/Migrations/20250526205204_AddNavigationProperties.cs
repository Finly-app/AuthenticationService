using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_has_roles_role_id",
                table: "users_has_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_has_policies_policy_id",
                table: "users_has_policies",
                column: "policy_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_has_policies_policy_id",
                table: "roles_has_policies",
                column: "policy_id");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_has_policies_policies_policy_id",
                table: "roles_has_policies",
                column: "policy_id",
                principalTable: "policies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_has_policies_roles_role_id",
                table: "roles_has_policies",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_has_policies_policies_policy_id",
                table: "users_has_policies",
                column: "policy_id",
                principalTable: "policies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_has_roles_roles_role_id",
                table: "users_has_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_has_policies_policies_policy_id",
                table: "roles_has_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_has_policies_roles_role_id",
                table: "roles_has_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_users_has_policies_policies_policy_id",
                table: "users_has_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_users_has_roles_roles_role_id",
                table: "users_has_roles");

            migrationBuilder.DropIndex(
                name: "IX_users_has_roles_role_id",
                table: "users_has_roles");

            migrationBuilder.DropIndex(
                name: "IX_users_has_policies_policy_id",
                table: "users_has_policies");

            migrationBuilder.DropIndex(
                name: "IX_roles_has_policies_policy_id",
                table: "roles_has_policies");
        }
    }
}
