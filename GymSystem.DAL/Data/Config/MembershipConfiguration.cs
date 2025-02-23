using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Config
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(m => m.Id);
            builder.HasOne(m => m.User)
                   .WithOne(u => u.membership)
                   .HasForeignKey<Membership>(m => m.UserId);



            builder.HasOne(m => m.Class)
                   .WithMany(c => c.Memberships)
                   .HasForeignKey(m => m.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(m => m.Price)
                   .HasColumnType("decimal(18,2)");

            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}