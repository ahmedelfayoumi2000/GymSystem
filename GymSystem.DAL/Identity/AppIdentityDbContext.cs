﻿using GymSystem.DAL.Entities;
using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymSystem.DAL.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedRoles(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData
            (
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Trainer", NormalizedName = "TRAINER" },
                new IdentityRole { Id = "3", Name = "Member", NormalizedName = "MEMBER" },
                new IdentityRole { Id = "5", Name = "Receptionist", NormalizedName = "RECEPTIONIST" }
            );
        }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseCategory> ExerciseCategories { get; set; }
        public DbSet<MealsCategory> MealsCategories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<BMIRecord> bMIRecords { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NutritionPlan> NutritionPlans { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<DailyPlan> DailyPlans { get; set; }
        public DbSet<MonthlyPlan> MonthlyPlans { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }

        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<EquipmentMaintenance> EquipmentMaintenances { get; set; }
        public DbSet<ClassEquipment> ClassEquipments { get; set; }
    }
}