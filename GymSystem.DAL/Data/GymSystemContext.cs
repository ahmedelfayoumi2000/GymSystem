using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymSystem.DAL.Data
{
    //public class GymSystemContext : DbContext
    //{
    //    public GymSystemContext(DbContextOptions<GymSystemContext> options) : base(options) { }

    //    public DbSet<DailyPlan> DailyPlans { get; set; }
    //    public DbSet<MonthlyPlan> MonthlyPlans { get; set; }
    //    public DbSet<NutritionPlan> NutritionPlans { get; set; }
    //    public DbSet<Membership> Memberships { get; set; }
    //    public DbSet<Class> Classes { get; set; }
    //    public DbSet<Attendance> Attendances { get; set; }
    //    public DbSet<Meal> Meals { get; set; }
    //    public DbSet<MealsCategory> MealsCategories { get; set; }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);
    //        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    //    }
    //}
}