using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for Tag entity
    /// </summary>
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Color)
                .IsRequired()
                .HasMaxLength(20);

            // Configure DateTime properties to use appropriate database type
            builder.Property(t => t.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(t => t.UpdatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            // Create a unique index on the Name property
            builder.HasIndex(t => t.Name)
                .IsUnique();
        }
    }
}
