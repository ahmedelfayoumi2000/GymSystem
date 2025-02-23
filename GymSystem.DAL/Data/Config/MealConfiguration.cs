using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Config
{
    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.NutritionPlan)
                   .WithMany(np => np.Meals)
                   .HasForeignKey(m => m.NutritionPlanId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.MealsCategory)
                   .WithMany(mc => mc.Meals)
                   .HasForeignKey(m => m.MealsCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}