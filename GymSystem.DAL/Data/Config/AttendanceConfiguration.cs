using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Config
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasOne(a => a.Class)
                   .WithMany(c => c.Attendances)
                   .HasForeignKey(a => a.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}