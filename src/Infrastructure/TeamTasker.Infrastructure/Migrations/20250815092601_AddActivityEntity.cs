using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTasker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Target = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    TargetType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TargetId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssigneeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Action",
                table: "Activities",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_AssigneeId",
                table: "Activities",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CreatedDate",
                table: "Activities",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TargetType_TargetId",
                table: "Activities",
                columns: new[] { "TargetType", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_UserId",
                table: "Activities",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
