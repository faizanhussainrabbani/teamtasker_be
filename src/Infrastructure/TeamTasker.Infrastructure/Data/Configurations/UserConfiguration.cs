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

            builder.Property(u => u.Status)
                .IsRequired();

            builder.Property(u => u.CreatedDate)
                .IsRequired();

            builder.OwnsOne(u => u.Address, a =>
            {
                a.Property(p => p.Street)
                    .HasMaxLength(100)
                    .HasColumnName("Street");

                a.Property(p => p.City)
                    .HasMaxLength(50)
                    .HasColumnName("City");

                a.Property(p => p.State)
                    .HasMaxLength(50)
                    .HasColumnName("State");

                a.Property(p => p.Country)
                    .HasMaxLength(50)
                    .HasColumnName("Country");

                a.Property(p => p.ZipCode)
                    .HasMaxLength(20)
                    .HasColumnName("ZipCode");
            });
        }
    }
}
