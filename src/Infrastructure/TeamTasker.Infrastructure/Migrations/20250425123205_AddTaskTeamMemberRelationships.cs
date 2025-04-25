using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTasker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskTeamMemberRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedToTeamMemberId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatorTeamMemberId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToTeamMemberId",
                table: "Tasks",
                column: "AssignedToTeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatorTeamMemberId",
                table: "Tasks",
                column: "CreatorTeamMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TeamMembers_AssignedToTeamMemberId",
                table: "Tasks",
                column: "AssignedToTeamMemberId",
                principalTable: "TeamMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TeamMembers_CreatorTeamMemberId",
                table: "Tasks",
                column: "CreatorTeamMemberId",
                principalTable: "TeamMembers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TeamMembers_AssignedToTeamMemberId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TeamMembers_CreatorTeamMemberId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToTeamMemberId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatorTeamMemberId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToTeamMemberId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatorTeamMemberId",
                table: "Tasks");
        }
    }
}
