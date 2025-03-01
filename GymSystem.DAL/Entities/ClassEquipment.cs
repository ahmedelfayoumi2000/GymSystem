using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Entities
{
	public class ClassEquipment
	{
		public int ClassId { get; set; }
		public Class Class { get; set; }
		public int EquipmentId { get; set; }
		public Equipment Equipment { get; set; }
	}
}