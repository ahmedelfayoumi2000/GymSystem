using GymSystem.DAL.Entities.Identity;
using System.Collections.Generic;

namespace GymSystem.DAL.Entities
{
    public class DailyPlan : SubscriptionPlan
    {
        // العلاقة مع AppUser (علاقة واحد إلى كثير)
        public ICollection<AppUser> UsersWithDailyPlan { get; set; } = new List<AppUser>();
    }
}