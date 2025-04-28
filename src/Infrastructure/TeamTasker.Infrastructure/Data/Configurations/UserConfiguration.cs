using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.ValueObjects;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for User entity
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(u => u.Username)
                .IsUnique();

            // Configure enum properties with documentation
            // Status: 0=Active, 1=Inactive, 2=Pending, 3=Locked
            builder.Property(u => u.Status)
                .IsRequired()
                .HasComment("User status: 0=Active, 1=Inactive, 2=Pending, 3=Locked");

            // Configure DateTime properties to use appropriate database type
            builder.Property(u => u.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(u => u.UpdatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(255);

            builder.Property(u => u.Role)
                .HasMaxLength(50);

            builder.Property(u => u.Avatar)
                .HasMaxLength(255);

            builder.Property(u => u.Initials)
                .HasMaxLength(10);

            builder.Property(u => u.Department)
                .HasMaxLength(100);

            builder.Property(u => u.Bio)
                .HasMaxLength(500);

            builder.Property(u => u.Location)
                .HasMaxLength(100);

            builder.Property(u => u.Phone)
                .HasMaxLength(20);

            builder.OwnsOne(u => u.Address, a =>
            {
                a.Property(p => p.Street)
                    .HasMaxLength(100)
                    .HasColumnName("Street")
                    .IsRequired(false);

                a.Property(p => p.City)
                    .HasMaxLength(50)
                    .HasColumnName("City")
                    .IsRequired(false);

                a.Property(p => p.State)
                    .HasMaxLength(50)
                    .HasColumnName("State")
                    .IsRequired(false);

                a.Property(p => p.Country)
                    .HasMaxLength(50)
                    .HasColumnName("Country")
                    .IsRequired(false);

                a.Property(p => p.ZipCode)
                    .HasMaxLength(20)
                    .HasColumnName("ZipCode")
                    .IsRequired(false);
            });
        }
    }
}
