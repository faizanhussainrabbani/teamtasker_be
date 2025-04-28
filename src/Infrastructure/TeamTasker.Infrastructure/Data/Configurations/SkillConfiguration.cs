using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            // Configure DateTime properties to use appropriate database type
            builder.Property(s => s.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(s => s.UpdatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            // Add indexes for frequently queried columns
            builder.HasIndex(s => s.Category);
        }
    }
}
