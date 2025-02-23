using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface IMembershipRepo
    {
        Task<IEnumerable<MembershipDto>> GetAllMemberships();
        Task<MembershipDto> GetMembershipById(int id);
        Task<ApiResponse> CreateMembership(MembershipDto membership);
        Task<ApiResponse> UpdateMembership(int id, MembershipDto membership);
        Task<ApiResponse> DeleteMembership(int id);
    }
}
