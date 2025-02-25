using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public bool IsAttended { get; set; }
        public string UserCode { get; set; }
        public int ClassId { get; set; }
		public string UserId { get; set; } // إضافة UserId
	}
}
