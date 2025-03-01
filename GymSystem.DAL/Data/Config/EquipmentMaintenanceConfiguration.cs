using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Configurations
{
	public class EquipmentMaintenanceConfiguration : IEntityTypeConfiguration<EquipmentMaintenance>
	{
		public void Configure(EntityTypeBuilder<EquipmentMaintenance> builder)
		{
			builder.HasKey(em => new { em.EquipmentId, em.UserId, em.MaintenanceDate });

			builder.Property(em => em.MaintenanceDate)
				   .IsRequired();

			builder.HasOne(em => em.Equipment)
				   .WithMany(e => e.MaintainedByUsers)
				   .HasForeignKey(em => em.EquipmentId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(em => em.User)
				   .WithMany(u => u.MaintainedEquipments)
				   .HasForeignKey(em => em.UserId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}