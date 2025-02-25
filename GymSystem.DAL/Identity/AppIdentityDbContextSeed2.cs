using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Identity
{
	public class AppIdentityDbContextSeed2
	{
		public async static Task SeedAsync(AppIdentityDbContext _context)
		{
			await SeedDailyPlansAsync(_context);

			await SeedMonthlyPlansAsync(_context);


		}

		public async static Task SeedDailyPlansAsync(AppIdentityDbContext _context)
		{
			if (_context.DailyPlans.Count() == 0)
			{
				var dailyplans = new List<SubscriptionPlan>
				{
					new DailyPlan {   Name="Cardio", Price=20  },
					new DailyPlan {   Name="Box", Price=30  }
				};

				if (dailyplans is not null && dailyplans.Count() > 0)
				{
					await _context.SubscriptionPlan.AddRangeAsync(dailyplans);
					await _context.SaveChangesAsync();
				}

			}


		}
		public async static Task SeedMonthlyPlansAsync(AppIdentityDbContext _context)
		{

			if (_context.MonthlyPlans.Count() == 0)
			{

				var monthlyPlan = new List<SubscriptionPlan>()

				{

					new MonthlyPlan {  Name="Half Month", Price=150,DurationInDays=15  },
					new MonthlyPlan {  Name="Month", Price=250,DurationInDays=30  }

				};

				if (monthlyPlan is not null && monthlyPlan.Count() > 0)
				{
					await _context.SubscriptionPlan.AddRangeAsync(monthlyPlan);
					await _context.SaveChangesAsync();
				}

			}

		}
	}
}
