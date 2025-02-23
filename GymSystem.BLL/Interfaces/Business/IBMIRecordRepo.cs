using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface IBMIRecordRepo
    {
        Task<IEnumerable<object>> GetBMIRecordsForUser(string userId);
        Task<ApiResponse> AddBMIRecord(BMIRecordDto bmiRecord);
        Task<ApiResponse> DeleteBMIRecord(int id);
    }
}
