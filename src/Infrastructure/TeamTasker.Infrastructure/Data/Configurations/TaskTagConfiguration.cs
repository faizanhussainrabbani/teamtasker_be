using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for TaskTag entity
    /// </summary>
    public class TaskTagConfiguration : IEntityTypeConfiguration<TaskTag>
    {
        public void Configure(EntityTypeBuilder<TaskTag> builder)
        {
            builder.HasKey(tt => tt.Id);

            // Configure DateTime properties to use appropriate database type
            builder.Property(tt => tt.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            // Configure relationship with Task
            builder.HasOne(tt => tt.Task)
                .WithMany(t => t.Tags)
                .HasForeignKey(tt => tt.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Tag
            builder.HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create a unique index on TaskId and TagId to prevent duplicate tags on a task
            builder.HasIndex(tt => new { tt.TaskId, tt.TagId })
                .IsUnique();
        }
    }
}
