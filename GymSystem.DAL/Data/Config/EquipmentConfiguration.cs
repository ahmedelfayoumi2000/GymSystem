using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Configurations
{
	public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
	{
		public void Configure(EntityTypeBuilder<Equipment> builder)
		{
			builder.HasKey(e => e.Id);

			builder.Property(e => e.EquipmentName)
				   .IsRequired()
				   .HasMaxLength(100);

			builder.Property(e => e.Description)
				   .HasMaxLength(500);

			builder.Property(e => e.IsAvailable)
				   .IsRequired()
				   .HasDefaultValue(true);

			builder.Property(e => e.LastMaintenanceDate)
				   .IsRequired();

			// العلاقة مع AppUser عبر EquipmentMaintenance
			builder.HasMany(e => e.MaintainedByUsers)
				   .WithOne(em => em.Equipment)
				   .HasForeignKey(em => em.EquipmentId)
				   .OnDelete(DeleteBehavior.Cascade);

			// العلاقة مع Class عبر ClassEquipment
			builder.HasMany(e => e.UsedInClasses)
				   .WithOne(ce => ce.Equipment)
				   .HasForeignKey(ce => ce.EquipmentId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}