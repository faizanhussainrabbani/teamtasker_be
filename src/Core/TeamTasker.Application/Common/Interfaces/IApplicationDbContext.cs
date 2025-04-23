using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for the application database context
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<Project> Projects { get; }
        DbSet<Domain.Entities.Task> Tasks { get; }
        DbSet<User> Users { get; }
        DbSet<Team> Teams { get; }
        DbSet<TeamMember> TeamMembers { get; }
        DbSet<Skill> Skills { get; }
        DbSet<UserSkill> UserSkills { get; }
        DbSet<TaskTag> TaskTags { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
