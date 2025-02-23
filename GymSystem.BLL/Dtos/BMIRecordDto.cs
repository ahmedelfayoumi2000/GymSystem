using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos
{
    public class BMIRecordDto
    {
        public int? BMIRecordId { get; set; }
        public int? Category { get; set; }
        public string? UserId { get; set; }
        public decimal WeightInKg { get; set; }
        public decimal HeightInMeters { get; set; }
    }
}
