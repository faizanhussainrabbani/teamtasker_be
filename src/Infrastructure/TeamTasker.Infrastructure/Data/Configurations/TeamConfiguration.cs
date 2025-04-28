using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.Department)
                .HasMaxLength(100);

            // Configure DateTime properties to use appropriate database type
            builder.Property(t => t.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(t => t.UpdatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            // Add indexes for frequently queried columns
            builder.HasIndex(t => t.Department);

            // Configure relationship with Lead (User)
            builder.HasOne(t => t.Lead)
                .WithMany()
                .HasForeignKey(t => t.LeadId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
