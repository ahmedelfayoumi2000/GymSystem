using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Config
{
    //public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    //{
    //    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    //    {
    //        // 1. تكوين المفتاح الأساسي
    //        builder.HasKey(p => p.Id);

    //        // 2. تكوين العلاقة مع Memberships (واحد إلى كثير)
    //        builder.HasMany(p => p.Memberships)
    //               .WithOne(m => m.SubscriptionPlan)
    //               .HasForeignKey(m => m.)
    //               .OnDelete(DeleteBehavior.Cascade); // Cascade مسموح هنا لأنه المسار الوحيد
    //    }
    //}
}