using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for Task entity
    /// </summary>
    public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            // Configure enum properties with documentation
            // Status: 0=ToDo, 1=InProgress, 2=Done, 3=Blocked, 4=OnHold, 5=Cancelled
            builder.Property(t => t.Status)
                .IsRequired()
                .HasComment("Task status: 0=ToDo, 1=InProgress, 2=Done, 3=Blocked, 4=OnHold, 5=Cancelled");

            // Priority: 0=Low, 1=Medium, 2=High, 3=Critical
            builder.Property(t => t.Priority)
                .IsRequired()
                .HasComment("Task priority: 0=Low, 1=Medium, 2=High, 3=Critical");

            // Configure DateTime properties to use appropriate database type
            builder.Property(t => t.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(t => t.UpdatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(t => t.DueDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(t => t.CompletedDate)
                .HasColumnType("datetime");

            builder.Property(t => t.Progress)
                .IsRequired();

            // Add indexes for frequently queried columns
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.DueDate);
            builder.HasIndex(t => t.Priority);

            // Configure relationship with TeamMember (Creator)
            builder.HasOne(t => t.CreatorTeamMember)
                .WithMany()
                .HasForeignKey(t => t.CreatorTeamMemberId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationship with TeamMember (Assignee)
            builder.HasOne(t => t.AssignedToTeamMember)
                .WithMany()
                .HasForeignKey(t => t.AssignedToTeamMemberId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
