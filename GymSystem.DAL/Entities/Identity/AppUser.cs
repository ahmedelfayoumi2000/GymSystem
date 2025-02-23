using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace GymSystem.DAL.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } 
        public Address? Address { get; set; }
        public int UserRole { get; set; } 
        public bool IsDeleted { get; set; } 
        public DateTime? DeletedAt { get; set; }
        public string? ProfileImageName { get; set; } 
        public string? UserCode { get; set; }
        public string? Gender { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public string? City { get; set; } 
        public bool IsStopped { get; set; } = false; 
        public DateTime? StopDate { get; set; }
        public int? HaveDays { get; set; }
        public string? AddBy { get; set; } 
        public int? RemainingDays { get; set; }

        // العلاقات مع الخطط
        public int? DailyPlanId { get; set; } 
        public DailyPlan DailyPlan { get; set; } 

        public int? MonthlyPlanId { get; set; } 
        public MonthlyPlan MonthlyPlan { get; set; } 


        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        public int? NutritionPlanId { get; set; }
        public NutritionPlan nutritionPlan { get; set; }
        public Membership membership { get; set; }
        public ICollection<WorkoutPlan> WorkoutPlans { get; set; }
        public ICollection<BMIRecord> BMIRecords { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}