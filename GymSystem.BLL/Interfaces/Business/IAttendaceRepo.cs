using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;


namespace GymSystem.BLL.Interfaces
{
    public interface IAttendaceRepo
    {
        Task<ApiResponse> AddAttendanceAsync(AttendanceDto attendance);
        Task<IReadOnlyList<AttendanceDto>> GetAttendancesForUserAsync(string userCode);
        Task<ApiResponse> DeleteAttendanceAsync(int id);
    }
}