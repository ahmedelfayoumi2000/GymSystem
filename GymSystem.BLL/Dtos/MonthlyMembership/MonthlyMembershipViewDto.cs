using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos.MonthlyMembership
{
	public class MonthlyMembershipViewDto
	{

		public string UserName { get; set; }

		public string UserEmail { get; set; }

		public string phoneNumber { get; set; }

		public Plan Plan { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; } 


		public bool IsActive { get; set; }
	}
}
