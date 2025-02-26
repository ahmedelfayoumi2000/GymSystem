using AutoMapper;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GymSystem.API.Controllers
{
  
    public class ClassController : BaseApiController
    {
        private readonly IClassRepo _classRepo;
        private readonly ILogger<ClassController> _logger;

        public ClassController(
            IClassRepo classRepo,
            ILogger<ClassController> logger)
        {
            _classRepo = classRepo ?? throw new ArgumentNullException(nameof(classRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize(Roles = "Admin,Receptionist,Trainer,Member")]
        [HttpGet("getClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClass(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid class ID provided for GetClass: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Class ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Fetching class with ID: {Id} by user with role: {Roles}", id, User.FindFirst(ClaimTypes.Role)?.Value);
                var classDto = await _classRepo.GetClass(id);

                if (classDto == null)
                {
                    _logger.LogWarning("Class with ID {Id} not found.", id);
                    return NotFound(new ApiResponse(404, $"Class with ID {id} not found"));
                }

                _logger.LogInformation("Class with ID {Id} retrieved successfully.", id);
                return Ok(new ApiResponse(200, "Class retrieved successfully", classDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class with ID: {Id}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving the class", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Trainer,Member")]
        [HttpGet("getAllClasses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                _logger.LogInformation("Fetching all active classes by user with role: {Roles}", User.FindFirst(ClaimTypes.Role)?.Value);
                var classes = await _classRepo.GetClasses();

                _logger.LogInformation("Retrieved {Count} active classes.", classes.Count());
                return Ok(new ApiResponse(200, "Classes retrieved successfully", classes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all classes.");
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving classes", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Trainer")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddClass([FromBody] ClassDto classDto)
        {
            if (!ModelState.IsValid || classDto == null)
            {
                _logger.LogWarning("Invalid model state or null data for AddClass with ClassName: {ClassName}", classDto?.ClassName);
                return BadRequest(CreateValidationErrorResponse("Invalid class data"));
            }

            try
            {
                var trainerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TrainerId");
                if (trainerIdClaim == null && User.IsInRole("Trainer"))
                {
                    _logger.LogWarning("TrainerId claim not found in token for AddClass request by Trainer.");
                    return BadRequest(new ApiResponse(400, "TrainerId claim not found in the token"));
                }

                if (User.IsInRole("Trainer") && classDto.TrainerId != trainerIdClaim?.Value)
                {
                    _logger.LogWarning("Trainer attempted to add class with unauthorized TrainerId: {TrainerId}", classDto.TrainerId);
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, "Trainers can only create classes for themselves."));
                }

                _logger.LogInformation("Adding class with name: {ClassName} by user with role: {Roles}", classDto.ClassName, User.FindFirst(ClaimTypes.Role)?.Value);
                var response = await _classRepo.AddClass(classDto);

                if (response.StatusCode == 201)
                {
                    _logger.LogInformation("Class {ClassName} added successfully.", classDto.ClassName);
                    return StatusCode(StatusCodes.Status201Created, response);
                }

                _logger.LogWarning("Failed to add class {ClassName}: {Message}", classDto.ClassName, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding class with name: {ClassName}", classDto?.ClassName);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while adding the class", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Trainer")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid class ID provided for DeleteClass: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Class ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Attempting to delete class with ID: {Id} by user with role: {Roles}", id, User.FindFirst(ClaimTypes.Role)?.Value);
                var classDto = await _classRepo.GetClass(id);

                if (classDto == null)
                {
                    _logger.LogWarning("Class with ID {Id} not found.", id);
                    return NotFound(new ApiResponse(404, $"Class with ID {id} not found"));
                }

                if (User.IsInRole("Trainer"))
                {
                    var trainerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TrainerId");
                    if (trainerIdClaim == null || classDto.TrainerId != trainerIdClaim.Value)
                    {
                        _logger.LogWarning("Trainer attempted to delete unauthorized class with ID: {Id}", id);
                        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, "Trainers can only delete their own classes."));
                    }
                }

                var response = await _classRepo.DeleteClass(id);
                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Class with ID {Id} deleted successfully.", id);
                    return Ok(response);
                }

                _logger.LogWarning("Failed to delete class with ID {Id}: {Message}", id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class with ID: {Id}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while deleting the class", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Trainer")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] ClassDto classDto)
        {
            if (!ModelState.IsValid || classDto == null)
            {
                _logger.LogWarning("Invalid model state or null data for UpdateClass with ID: {Id}", id);
                return BadRequest(CreateValidationErrorResponse("Invalid class data"));
            }

            if (id <= 0)
            {
                _logger.LogWarning("Invalid class ID provided for UpdateClass: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Class ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Attempting to update class with ID: {Id} by user with role: {Roles}", id, User.FindFirst(ClaimTypes.Role)?.Value);
                var existingClass = await _classRepo.GetClass(id);

                if (existingClass == null)
                {
                    _logger.LogWarning("Class with ID {Id} not found.", id);
                    return NotFound(new ApiResponse(404, $"Class with ID {id} not found"));
                }

                if (User.IsInRole("Trainer"))
                {
                    var trainerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TrainerId");
                    if (trainerIdClaim == null || (existingClass.TrainerId != trainerIdClaim.Value || classDto.TrainerId != trainerIdClaim.Value))
                    {
                        _logger.LogWarning("Trainer attempted to update unauthorized class with ID: {Id}", id);
                        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, "Trainers can only update their own classes."));
                    }
                }

                var response = await _classRepo.UpdateClass(id, classDto);
                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Class with ID {Id} updated successfully.", id);
                    return Ok(response);
                }

                _logger.LogWarning("Failed to update class with ID {Id}: {Message}", id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class with ID: {Id}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while updating the class", ex.Message));
            }
        }

        private ApiValidationErrorResponse CreateValidationErrorResponse(string message)
        {
            return new ApiValidationErrorResponse
            {
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                StatusCode = 400,
                Message = message
            };
        }

        private ActionResult<ApiResponse> HandleException(Exception ex)
        {
            return StatusCode(500, new ApiExceptionResponse(500, "An unexpected error occurred", ex.Message));
        }
    }
}