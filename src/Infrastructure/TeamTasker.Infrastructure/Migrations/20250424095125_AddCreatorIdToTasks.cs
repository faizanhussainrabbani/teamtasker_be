using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTasker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorIdToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatorId",
                table: "Tasks",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatorId",
                table: "Tasks",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatorId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Tasks");
        }
    }
}
