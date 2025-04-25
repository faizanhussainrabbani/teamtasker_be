using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;

namespace TeamTasker.Infrastructure.Data.Migrations
{
    /// <summary>
    /// Migration to ensure all tasks have team member references
    /// </summary>
    public partial class EnsureTasksHaveTeamMembers : Migration
    {
        private readonly ILogger<EnsureTasksHaveTeamMembers> _logger;

        public EnsureTasksHaveTeamMembers(ILogger<EnsureTasksHaveTeamMembers> logger)
        {
            _logger = logger;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This migration doesn't modify the schema, but ensures all tasks have team member references
            // We'll use raw SQL to update the data

            // 1. First, ensure all users have at least one team membership
            migrationBuilder.Sql(@"
                -- Create a default team for users without team memberships
                INSERT INTO Teams (Name, Description, Department, CreatedDate, UpdatedDate)
                SELECT 'Default Team', 'Default team for users without team memberships', 'General', datetime('now'), datetime('now')
                WHERE NOT EXISTS (SELECT 1 FROM Teams WHERE Name = 'Default Team' AND Description = 'Default team for users without team memberships');
                
                -- Get the default team ID
                DECLARE @defaultTeamId INT;
                SELECT @defaultTeamId = Id FROM Teams WHERE Name = 'Default Team' AND Description = 'Default team for users without team memberships';
                
                -- Add users without team memberships to the default team
                INSERT INTO TeamMembers (TeamId, UserId, Role, JoinedDate)
                SELECT @defaultTeamId, u.Id, 'Member', datetime('now')
                FROM Users u
                WHERE NOT EXISTS (
                    SELECT 1 FROM TeamMembers tm WHERE tm.UserId = u.Id
                );
            ");

            // 2. Update tasks to use team member references
            migrationBuilder.Sql(@"
                -- Update tasks with AssignedToUserId but no AssignedToTeamMemberId
                UPDATE Tasks
                SET AssignedToTeamMemberId = (
                    SELECT tm.Id 
                    FROM TeamMembers tm 
                    WHERE tm.UserId = Tasks.AssignedToUserId
                    LIMIT 1
                )
                WHERE AssignedToUserId IS NOT NULL 
                AND AssignedToTeamMemberId IS NULL;
                
                -- Update tasks with CreatorId but no CreatorTeamMemberId
                UPDATE Tasks
                SET CreatorTeamMemberId = (
                    SELECT tm.Id 
                    FROM TeamMembers tm 
                    WHERE tm.UserId = Tasks.CreatorId
                    LIMIT 1
                )
                WHERE CreatorId IS NOT NULL 
                AND CreatorTeamMemberId IS NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No need to revert the data changes
        }
    }
}
