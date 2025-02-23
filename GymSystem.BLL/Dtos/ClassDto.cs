﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos
{
    public class ClassDto
    {
        public int? ClassId { get; set; }
        //public string? ImageUrl { get; set; }
        //[NotMapped]
        //public IFormFile? Image { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? TrainerId { get; set; }
    }
}
