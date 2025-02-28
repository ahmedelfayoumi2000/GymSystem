using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos
{
    public class EquipmentViewDto
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
    }
}
