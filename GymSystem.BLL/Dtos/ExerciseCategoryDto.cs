using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos
{
    public class ExerciseCategoryDto
    {
        public int? ID { get; set; }
        //public string? ImageUrl { get; set; }
        //[NotMapped]
        //public IFormFile Image { get; set; }
        public string CategoryName { get; set; }
    }
}
