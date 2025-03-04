using GymSystem.BLL.Dtos.MonthlyMembership;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface IMonthlyMembershipRepo
	{
		Task<IReadOnlyList<MonthlyMembershipViewDto>> GetAllAsync(SpecPrams specParams = null);
		Task<MonthlyMembershipViewDto> GetByEmailAsync(string email);
		Task<ApiResponse> CreateAsync(MonthlyMembershipCreateDto membershipDto);
		Task<ApiResponse> UpdateAsync(string email, MonthlyMembershipCreateDto membershipDto);
		Task<ApiResponse> DeleteAsync(string email);
	}
}