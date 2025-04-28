using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Infrastructure.Data.Configurations
{
    public class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
    {
        public void Configure(EntityTypeBuilder<UserSkill> builder)
        {
            builder.HasKey(us => us.Id);

            // Configure Level property with documentation
            builder.Property(us => us.Level)
                .IsRequired()
                .HasComment("Proficiency level: 1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert, 5=Master");

            // Configure DateTime properties to use appropriate database type
            builder.Property(us => us.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(us => us.UpdatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            // Configure relationship with User
            builder.HasOne(us => us.User)
                .WithMany(u => u.Skills)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Skill
            builder.HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
