﻿using GymSystem.DAL.Entities.Identity;
using System;

namespace GymSystem.DAL.Entities
{
	public class EquipmentMaintenance
	{
		public int EquipmentId { get; set; }
		public Equipment Equipment { get; set; }
		public string UserId { get; set; } // AppUser (Trainer)
		public AppUser User { get; set; }
		public DateTime MaintenanceDate { get; set; }
	}
}