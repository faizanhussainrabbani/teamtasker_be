using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTasker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreatorIdFromModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This is a no-op migration to update the model snapshot
            // The CreatorId column has already been removed from the database
            migrationBuilder.Sql(@"
                -- No-op migration to update the model snapshot
                -- The CreatorId column has already been removed from the database
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op for down migration
            migrationBuilder.Sql(@"
                -- No-op for down migration
            ");
        }
    }
}
