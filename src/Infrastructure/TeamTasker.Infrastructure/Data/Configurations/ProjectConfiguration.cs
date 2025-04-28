using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for Project entity
    /// </summary>
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            // Configure enum properties with documentation
            // Status: 0=NotStarted, 1=Active, 2=OnHold, 3=Completed, 4=Cancelled
            builder.Property(p => p.Status)
                .IsRequired()
                .HasComment("Project status: 0=NotStarted, 1=Active, 2=OnHold, 3=Completed, 4=Cancelled");

            // Configure DateTime properties to use appropriate database type
            builder.Property(p => p.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(p => p.UpdatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(p => p.StartDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(p => p.EndDate)
                .HasColumnType("datetime");

            // Add indexes for frequently queried columns
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.StartDate);
            builder.HasIndex(p => p.EndDate);

            builder.HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Team
            builder.HasOne(p => p.Team)
                .WithMany(t => t.Projects)
                .HasForeignKey(p => p.TeamId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
