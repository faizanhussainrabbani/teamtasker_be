using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity configuration for Activity
    /// </summary>
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId)
                .IsRequired();

            builder.Property(a => a.Action)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Target)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.TargetType)
                .HasMaxLength(100);

            builder.Property(a => a.TargetId);

            builder.Property(a => a.AssigneeId);

            builder.Property(a => a.Comment)
                .HasMaxLength(1000);

            builder.Property(a => a.CreatedDate)
                .IsRequired();

            // Configure relationships
            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Assignee)
                .WithMany()
                .HasForeignKey(a => a.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes for better query performance
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.CreatedDate);
            builder.HasIndex(a => new { a.TargetType, a.TargetId });
            builder.HasIndex(a => a.Action);
        }
    }
}
