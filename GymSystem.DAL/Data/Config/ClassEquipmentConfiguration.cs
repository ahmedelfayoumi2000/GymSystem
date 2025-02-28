using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Configurations
{
    public class ClassEquipmentConfiguration : IEntityTypeConfiguration<ClassEquipment>
    {
        public void Configure(EntityTypeBuilder<ClassEquipment> builder)
        {
            builder.HasKey(ce => new { ce.ClassId, ce.EquipmentId });

            builder.HasOne(ce => ce.Class)
                   .WithMany(c => c.ClassEquipments)
                   .HasForeignKey(ce => ce.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ce => ce.Equipment)
                   .WithMany(e => e.UsedInClasses)
                   .HasForeignKey(ce => ce.EquipmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}