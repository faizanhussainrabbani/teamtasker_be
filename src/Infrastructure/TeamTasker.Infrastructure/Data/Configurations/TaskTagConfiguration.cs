using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    public class TaskTagConfiguration : IEntityTypeConfiguration<TaskTag>
    {
        public void Configure(EntityTypeBuilder<TaskTag> builder)
        {
            builder.HasKey(tt => tt.Id);

            builder.Property(tt => tt.Tag)
                .IsRequired()
                .HasMaxLength(50);

            // Configure relationship with Task
            builder.HasOne(tt => tt.Task)
                .WithMany(t => t.Tags)
                .HasForeignKey(tt => tt.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
