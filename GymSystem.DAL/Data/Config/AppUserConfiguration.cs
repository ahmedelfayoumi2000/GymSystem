using GymSystem.DAL.Entities;
using GymSystem.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Config
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasIndex(u => u.DisplayName).IsUnique();

            builder.HasMany(u => u.WorkoutPlans)
                .WithOne(wp => wp.Trainer)
                .HasForeignKey(wp => wp.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.BMIRecords)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Feedbacks)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.nutritionPlan)
                .WithMany(np => np.Users)
                .HasForeignKey(u => u.NutritionPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(u => !u.IsDeleted);

            builder.HasIndex(u => u.DisplayName).IsUnique(false);
        }
        //public void Configure(EntityTypeBuilder<AppUser> builder)
        //{
        //    // 1. تكوين الفهرس الفريد لـ DisplayName
        //    builder.HasIndex(u => u.DisplayName).IsUnique();

        //    // 2. تكوين الكيان المملوك Address
        //    builder.OwnsOne(u => u.Address, a =>
        //    {
        //        a.Property(a => a.FristName).HasMaxLength(100);
        //        a.Property(a => a.LastName).HasMaxLength(100);
        //        a.Property(a => a.Country).HasMaxLength(100);
        //        a.Property(a => a.City).HasMaxLength(100);
        //        a.Property(a => a.Street).HasMaxLength(255);
        //    });

        //    // 3. تكوين العلاقة مع DailyPlan (واحد إلى واحد اختياري)
        //    builder.HasOne(u => u.DailyPlan)
        //           .WithMany()
        //           .HasForeignKey(u => u.DailyPlanId)
        //           .OnDelete(DeleteBehavior.NoAction); // تغيير السلوك لتجنب المسارات المتعددة

        //    // 4. تكوين العلاقة مع MonthlyPlan (واحد إلى واحد اختياري)
        //    builder.HasOne(u => u.MonthlyPlan)
        //           .WithMany()
        //           .HasForeignKey(u => u.MonthlyPlanId)
        //           .OnDelete(DeleteBehavior.NoAction); // تغيير السلوك لتجنب المسارات المتعددة

        //    // 5. تكوين العلاقة مع NutritionPlan (واحد إلى واحد اختياري)
        //    builder.HasOne(u => u.NutritionPlan)
        //           .WithMany(np => np.Users)
        //           .HasForeignKey(u => u.NutritionPlanId)
        //           .OnDelete(DeleteBehavior.SetNull);

        //    // 6. تكوين العلاقة مع Membership (واحد إلى واحد اختياري)
        //    //builder.HasOne(u => u.Membership)
        //    //       .WithOne(m => m.User) // Membership له مستخدم واحد فقط
        //    //       .HasForeignKey<Membership>(m => m.UserId)
        //    //       .OnDelete(DeleteBehavior.SetNull);

        //    // 7. تكوين العلاقة مع Attendances (واحد إلى كثير)
        //    builder.HasMany(u => u.Attendances)
        //           .WithOne(a => a.User)
        //           .HasForeignKey(a => a.UserId)
        //           .OnDelete(DeleteBehavior.Cascade);

        //    // 8. تكوين الكيان المملوك RefreshToken
        //    builder.OwnsMany(u => u.RefreshTokens, rt =>
        //    {
        //        rt.Property(rt => rt.Token).HasMaxLength(500);
        //        rt.Property(rt => rt.Expires).IsRequired();
        //        rt.Property(rt => rt.Created).IsRequired();
        //        rt.Property(rt => rt.Revoked).IsRequired(false);
        //    });

        //    // 9. تكوين فلتر لاستبعاد المستخدمين المحذوفين
        //    builder.HasQueryFilter(u => !u.IsDeleted);

        //    // 10. تكوين الخصائص الإضافية
        //    builder.Property(u => u.UserCode).HasMaxLength(50).IsRequired(); // تحديد الطول الأقصى لـ UserCode
        //    builder.Property(u => u.Gender).HasMaxLength(10); // تحديد الطول الأقصى لـ Gender
        //    builder.Property(u => u.City).HasMaxLength(100); // تحديد الطول الأقصى لـ City
        //    builder.Property(u => u.ProfileImageName).HasMaxLength(255); // تحديد الطول الأقصى لـ ProfileImageName
        //}
    }
}