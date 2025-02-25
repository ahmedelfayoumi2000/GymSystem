using AutoMapper;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.BLL.Interfaces;
using GymSystem.DAL.Entities;
using Microsoft.Extensions.Logging;
using GymSystem.BLL.Specifications.AttendanceByUserCode;

namespace GymSystem.BLL.Repositories.Business
{
    public class AttendaceRepo : IAttendaceRepo
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AttendaceRepo> _logger;

        public AttendaceRepo(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<AttendaceRepo> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse> AddAttendanceAsync(AttendanceDto attendanceDto)
        {
            try
            {
                _logger.LogInformation("Adding attendance for UserCode: {UserCode}", attendanceDto.UserCode);

                var user = await _userRepository.GetUserByCodeAsync(attendanceDto.UserCode);
                if (user == null)
                {
                    _logger.LogWarning("User with UserCode {UserCode} not found.", attendanceDto.UserCode);
                    return new ApiResponse(404, "User not found");
                }
				attendanceDto.UserId = user.Id;
				attendanceDto.IsAttended = true;
                var attendance = _mapper.Map<Attendance>(attendanceDto);

                var attendanceRepo = _unitOfWork.Repository<Attendance>();
                await attendanceRepo.Add(attendance);

                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    _logger.LogError("Failed to save attendance for UserCode {UserCode}", attendanceDto.UserCode);
                    return new ApiResponse(500, "Failed to save attendance");
                }

                _logger.LogInformation("Attendance added successfully for UserCode: {UserCode}", attendanceDto.UserCode);
                return new ApiResponse(200, "Attendance added successfully", _mapper.Map<AttendanceDto>(attendance));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attendance for UserCode: {UserCode}", attendanceDto.UserCode);
                return new ApiResponse(400, $"Error adding attendance: {ex.Message}");
            }
        }

        public async Task<IReadOnlyList<AttendanceDto>> GetAttendancesForUserAsync(string userCode)
        {
            try
            {
                _logger.LogInformation("Retrieving attendances for UserCode: {UserCode}", userCode);

                var user = await _userRepository.GetUserByCodeAsync(userCode);
                if (user == null)
                {
                    _logger.LogWarning("User with UserCode {UserCode} not found.", userCode);
                    return new List<AttendanceDto>();
                }

                var attendanceRepo = _unitOfWork.Repository<Attendance>();
                var attendances = await attendanceRepo.GetAllWithSpecAsync(new AttendanceByUserCodeSpec(userCode));

                var attendanceDtos = _mapper.Map<IReadOnlyList<AttendanceDto>>(attendances);
                _logger.LogInformation("Retrieved {Count} attendances for UserCode: {UserCode}", attendanceDtos.Count, userCode);

                return attendanceDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving attendances for UserCode: {UserCode}", userCode);
                return new List<AttendanceDto>();
            }
        }

        public async Task<ApiResponse> DeleteAttendanceAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting attendance with ID: {AttendanceId}", id);

                var attendanceRepo = _unitOfWork.Repository<Attendance>();
                var attendance = await attendanceRepo.GetByIdAsync(id);
                if (attendance == null)
                {
                    _logger.LogWarning("Attendance with ID {AttendanceId} not found.", id);
                    return new ApiResponse(404, "Attendance not found");
                }

                attendanceRepo.Delete(attendance);

                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    _logger.LogError("Failed to delete attendance with ID {AttendanceId}", id);
                    return new ApiResponse(500, "Failed to delete attendance");
                }

                _logger.LogInformation("Attendance deleted successfully with ID: {AttendanceId}", id);
                return new ApiResponse(200, "Attendance deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attendance with ID: {AttendanceId}", id);
                return new ApiResponse(400, $"Error deleting attendance: {ex.Message}");
            }
        }
    }

}