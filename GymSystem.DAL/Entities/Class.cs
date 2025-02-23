using GymSystem.DAL.Entities.Identity;
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

        // العلاقة مع Trainer (AppUser)
        public string? TrainerId { get; set; } // مفتاح أجنبي
        public AppUser Trainer { get; set; }

        // العلاقة مع Memberships
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

        // العلاقة مع Attendances
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}