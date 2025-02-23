using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymSystem.API.Controllers
{
  
    public class AttendanceController : BaseApiController
    {
        private readonly IAttendaceRepo _attendaceRepo;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            IAttendaceRepo attendaceRepo,
            ILogger<AttendanceController> logger)
        {
            _attendaceRepo = attendaceRepo ?? throw new ArgumentNullException(nameof(attendaceRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize(Roles = "Admin, Receptionist")]
        [HttpGet("getattendances")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttendances([FromQuery] string userCode)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for GetAttendances with UserCode: {UserCode}", userCode);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                _logger.LogInformation("Fetching attendances for UserCode: {UserCode}", userCode);
                var attendances = await _attendaceRepo.GetAttendancesForUserAsync(userCode);

                if (attendances == null || !attendances.Any())
                {
                    _logger.LogWarning("No attendances found for UserCode: {UserCode}", userCode);
                    return NotFound(new ApiResponse(404, "No attendances found for the specified user."));
                }

                _logger.LogInformation("Successfully retrieved {Count} attendances for UserCode: {UserCode}", attendances.Count, userCode);
                return Ok(new ApiResponse(200, "Attendances retrieved successfully", attendances));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving attendances for UserCode: {UserCode}", userCode);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving attendances", ex.Message));
            }
        }

        [Authorize(Roles = "Admin, Receptionist")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAttendance([FromBody] AttendanceDto attendance)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddAttendance with UserCode: {UserCode}", attendance?.UserCode);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                _logger.LogInformation("Adding attendance for UserCode: {UserCode}", attendance.UserCode);
                var response = await _attendaceRepo.AddAttendanceAsync(attendance);

                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Attendance added successfully for UserCode: {UserCode}", attendance.UserCode);
                    return Ok(response);
                }

                _logger.LogWarning("Failed to add attendance for UserCode: {UserCode}. Response: {Message}", attendance.UserCode, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attendance for UserCode: {UserCode}", attendance.UserCode);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while adding attendance", ex.Message));
            }
        }

        [Authorize(Roles = "Admin, Receptionist")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for DeleteAttendance with ID: {AttendanceId}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                _logger.LogInformation("Deleting attendance with ID: {AttendanceId}", id);
                var response = await _attendaceRepo.DeleteAttendanceAsync(id);

                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Attendance deleted successfully with ID: {AttendanceId}", id);
                    return Ok(response);
                }

                if (response.StatusCode == 404)
                {
                    _logger.LogWarning("Attendance with ID {AttendanceId} not found.", id);
                    return NotFound(response);
                }

                _logger.LogWarning("Failed to delete attendance with ID: {AttendanceId}. Response: {Message}", id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attendance with ID: {AttendanceId}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while deleting attendance", ex.Message));
            }
        }
    }
}
