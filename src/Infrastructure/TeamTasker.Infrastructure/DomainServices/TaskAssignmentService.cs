using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Exceptions;
using TeamTasker.Domain.Services;
using TeamTasker.Infrastructure.Data;

namespace TeamTasker.Infrastructure.DomainServices
{
    /// <summary>
    /// Implementation of the task assignment domain service
    /// </summary>
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly ApplicationDbContext _dbContext;
        private const int MaxTasksPerTeamMember = 10; // This could be configurable

        public TaskAssignmentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Checks if a team member can be assigned to a task
        /// </summary>
        public async Task<bool> CanAssignTaskToTeamMemberAsync(Entities.Task task, TeamMember teamMember)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (teamMember == null)
                throw new ArgumentNullException(nameof(teamMember));

            // Check if task is in a state that can be assigned
            if (task.Status == TaskStatus.Cancelled || task.Status == TaskStatus.Done)
                return false;

            // Check if team member is active
            if (!teamMember.IsActive)
                return false;

            // Check if team member is part of the project's team
            var project = await _dbContext.Projects.FindAsync(task.ProjectId);
            if (project != null && project.TeamId.HasValue)
            {
                bool isTeamMemberInProjectTeam = await _dbContext.TeamMembers
                    .AnyAsync(tm => tm.Id == teamMember.Id && tm.TeamId == project.TeamId);

                if (!isTeamMemberInProjectTeam)
                    return false;
            }

            // Check if team member has too many active tasks
            int activeTaskCount = await _dbContext.Tasks
                .CountAsync(t => t.AssignedToTeamMemberId == teamMember.Id &&
                                t.Status != TaskStatus.Done &&
                                t.Status != TaskStatus.Cancelled);

            return activeTaskCount < MaxTasksPerTeamMember;
        }

        /// <summary>
        /// Assigns a task to a team member with validation
        /// </summary>
        public async Task AssignTaskToTeamMemberAsync(Entities.Task task, TeamMember teamMember)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (teamMember == null)
                throw new ArgumentNullException(nameof(teamMember));

            bool canAssign = await CanAssignTaskToTeamMemberAsync(task, teamMember);
            if (!canAssign)
                throw new DomainException($"Cannot assign task to team member {teamMember.Name}");

            task.AssignToTeamMember(teamMember.Id);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the workload percentage for a team member
        /// </summary>
        public async Task<int> GetTeamMemberWorkloadPercentageAsync(int teamMemberId)
        {
            int activeTaskCount = await _dbContext.Tasks
                .CountAsync(t => t.AssignedToTeamMemberId == teamMemberId &&
                                t.Status != TaskStatus.Done &&
                                t.Status != TaskStatus.Cancelled);

            return (int)Math.Round((double)activeTaskCount / MaxTasksPerTeamMember * 100);
        }
    }
}
