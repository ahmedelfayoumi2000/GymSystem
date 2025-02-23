using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Config
{
    public class ExerciseCategoryConfiguration : IEntityTypeConfiguration<ExerciseCategory>
    {
        public void Configure(EntityTypeBuilder<ExerciseCategory> builder)
        {
            builder.HasKey(ec => ec.Id);
            builder.HasMany(ec => ec.Exercises)
                   .WithOne(e => e.ExerciseCategory)
                   .HasForeignKey(e => e.ExerciseCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
