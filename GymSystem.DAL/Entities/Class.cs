﻿using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymSystem.DAL.Entities
{
    public class Class : BaseEntity
    {
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }

        public string ClassName { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDeleted { get; set; }



        public string? TrainerId { get; set; }
        public AppUser Trainer { get; set; }

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        public ICollection<ClassEquipment> ClassEquipments { get; set; } = new List<ClassEquipment>();
    }
}