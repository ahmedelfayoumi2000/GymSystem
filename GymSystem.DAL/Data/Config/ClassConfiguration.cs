using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Configurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClassName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Description)
                   .HasMaxLength(500);

            builder.Property(c => c.ImageUrl)
                   .HasMaxLength(255);

            builder.Property(c => c.StartTime)
                   .IsRequired();

            builder.Property(c => c.EndTime)
                   .IsRequired();

            builder.Property(c => c.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.HasOne(c => c.Trainer)
                   .WithMany(t => t.Classes) 
                   .HasForeignKey(c => c.TrainerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Memberships)
                   .WithOne(m => m.Class)
                   .HasForeignKey(m => m.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Attendances)
                   .WithOne(a => a.Class)
                   .HasForeignKey(a => a.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.ClassEquipments)
                   .WithOne(ce => ce.Class)
                   .HasForeignKey(ce => ce.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query Filter لاستبعاد السجلات المحذوفة
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}