using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Config
{
    public class NutritionPlanConfiguration : IEntityTypeConfiguration<NutritionPlan>
    {
        public void Configure(EntityTypeBuilder<NutritionPlan> builder)
        {
            builder.HasKey(np => np.Id);

            builder.HasMany(np => np.Users)
                   .WithOne(u => u.nutritionPlan)
                   .HasForeignKey(u => u.NutritionPlanId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(np => np.Meals)
                   .WithOne(m => m.NutritionPlan)
                   .HasForeignKey(m => m.NutritionPlanId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(np => !np.IsDeleted);
        }
    }
}