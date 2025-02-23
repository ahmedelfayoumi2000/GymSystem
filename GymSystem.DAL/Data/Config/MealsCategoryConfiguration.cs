using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Config
{
    public class MealsCategoryConfiguration : IEntityTypeConfiguration<MealsCategory>
    {
        public void Configure(EntityTypeBuilder<MealsCategory> builder)
        {
            builder.HasKey(mc => mc.Id);

            builder.HasMany(mc => mc.Meals)
                   .WithOne(m => m.MealsCategory)
                   .HasForeignKey(m => m.MealsCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(mc => !mc.IsDeleted);
        }
    }
}