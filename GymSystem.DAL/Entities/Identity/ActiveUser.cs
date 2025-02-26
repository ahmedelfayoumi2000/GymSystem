using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Entities.Identity
{
	public class ActiveUser
	{
		public string UserId { get; set; } // المفتاح الأساسي
		public DateTime LoginTime { get; set; } // وقت تسجيل الدخول
	}
}
