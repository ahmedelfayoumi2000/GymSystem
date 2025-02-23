using GymSystem.DAL.Entities.Identity;
using System.Collections.Generic;

namespace GymSystem.DAL.Entities
{
    public class MonthlyPlan : SubscriptionPlan
    {
        public int DurationInDays { get; set; }

        // العلاقة مع AppUser (علاقة واحد إلى كثير)
        public ICollection<AppUser> UsersWithMonthlyPlan { get; set; } = new List<AppUser>();
    }
}