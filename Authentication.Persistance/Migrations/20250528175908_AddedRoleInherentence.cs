using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoleInherentence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "role_inheritance",
                columns: table => new
                {
                    ParentRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildRoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_inheritance", x => new { x.ChildRoleId, x.ParentRoleId });
                    table.ForeignKey(
                        name: "FK_role_inheritance_roles_ChildRoleId",
                        column: x => x.ChildRoleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_inheritance_roles_ParentRoleId",
                        column: x => x.ParentRoleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_inheritance_ParentRoleId",
                table: "role_inheritance",
                column: "ParentRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_inheritance");
        }
    }
}
