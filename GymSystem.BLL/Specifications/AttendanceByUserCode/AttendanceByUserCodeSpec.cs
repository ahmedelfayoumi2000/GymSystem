using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Specifications.AttendanceByUserCode
{
    internal class AttendanceByUserCodeSpec : BaseSpecification<Attendance>
    {
        public AttendanceByUserCodeSpec(string userCode) : base(a => a.User.UserCode == userCode)
        {
        }
    }
}
