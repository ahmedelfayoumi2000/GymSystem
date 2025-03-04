using GymSystem.DAL.Entities.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace GymSystem.DAL.Entities
{
	public class MonthlyMembershipp : BaseEntity
	{

		public string UserName { get; set; }
		public string UserEmail { get; set; }
		public string phoneNumber { get; set; }

		//public int? ClassId { get; set; }
		//public Class? Class { get; set; }
		public int? PlanId { get; set; }
		public Plan? Plan { get; set; }
		public DateTime StartDate { get; set; }=DateTime.Now;

		private DateTime? _endDate; // تخزين القيمة إذا تم تعيينها يدويًا
		public DateTime EndDate
		{
			get => _endDate ?? (Plan != null ? StartDate.AddDays(Plan.DurationDays) : StartDate);
			set => _endDate = value; // السماح بتعيينها يدويًا إذا لزم الأمر
		}
		public bool IsActive { get; set; }
	}
}