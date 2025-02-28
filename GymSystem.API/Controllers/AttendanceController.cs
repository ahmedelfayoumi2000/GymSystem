using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : BaseApiController
    {
        private readonly IAttendaceRepo _attendaceRepo;

        public AttendanceController(IAttendaceRepo attendaceRepo)
        {
            _attendaceRepo = attendaceRepo ?? throw new ArgumentNullException(nameof(attendaceRepo));
        }

        /// <summary>
        /// Retrieves all attendance records for a specific user by UserCode.
        /// </summary>
        [Authorize(Roles = "Admin, Receptionist")]
        [HttpGet("getattendances")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendances([FromQuery] string userCode)
        {
            if (string.IsNullOrWhiteSpace(userCode) || !ModelState.IsValid)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                var attendances = await _attendaceRepo.GetAttendancesForUserAsync(userCode);
                if (attendances == null || !attendances.Any())
                {
                    return NotFound(new ApiResponse(404, "No attendances found for the specified user."));
                }

                return Ok(new ApiResponse(200, "Attendances retrieved successfully", attendances));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving attendances", ex.Message));
            }
        }

        /// <summary>
        /// Adds a new attendance record for a user.
        /// </summary>
        [Authorize(Roles = "Admin, Receptionist")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAttendance([FromBody] AttendanceDto attendance)
        {
            if (!ModelState.IsValid || attendance == null)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    StatusCode = 400,
                    Message = "Invalid attendance data"
                });
            }

            try
            {
                var response = await _attendaceRepo.AddAttendanceAsync(attendance);
                return response.StatusCode == 200
                    ? Ok(response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while adding attendance", ex.Message));
            }
        }

        /// <summary>
        /// Deletes an attendance record by its ID.
        /// </summary>
        [Authorize(Roles = "Admin, Receptionist")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            if (id <= 0 || !ModelState.IsValid)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).Concat(new[] { "Attendance ID must be a positive integer." }).ToList(),
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                var response = await _attendaceRepo.DeleteAttendanceAsync(id);
                return response.StatusCode switch
                {
                    200 => Ok(response),
                    404 => NotFound(response),
                    _ => BadRequest(response)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while deleting attendance", ex.Message));
            }
        }
    }
}