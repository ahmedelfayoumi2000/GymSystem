using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos
{
    public class FeedbackDto
    {
        public int? FeedbackId { get; set; }
        public string? Comments { get; set; }
        public int Rating { get; set; }
        public string? UserId { get; set; }
        public string? TrainerId { get; set; }
    }
}
