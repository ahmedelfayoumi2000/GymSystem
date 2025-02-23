using AutoMapper;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.BLL.Interfaces;
using GymSystem.BLL.Specifications;
using GymSystem.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("getClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClass(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for GetClass with ID: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Fetching class with ID: {Id}", id);
                var classDto = await _classRepo.GetClass(id);

                if (classDto == null)
                {
                    _logger.LogWarning("Class with ID {Id} not found.", id);
                    return NotFound(new ApiResponse(404, $"Class with ID {id} not found."));
                }

                _logger.LogInformation("Class with ID {Id} retrieved successfully.", id);
                return Ok(classDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class with ID: {Id}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving the class", ex.Message));
            }
        }


        [HttpGet("getAllClasses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                _logger.LogInformation("Fetching all active classes.");
                var classes = await _classRepo.GetClasses();

                _logger.LogInformation("Retrieved {Count} active classes.", classes.Count());
                return Ok(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all classes.");
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving classes", ex.Message));
            }
        }

      
        [Authorize(Roles = "Admin,Receptionist,Trainer")] 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddClass([FromBody] ClassDto classDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddClass with ClassName: {ClassName}", classDto?.ClassName);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Attempting to add class with name: {ClassName}", classDto.ClassName);

                var trainerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TrainerId");
                if (trainerIdClaim == null)
                {
                    _logger.LogWarning("TrainerId claim not found in token for AddClass request.");
                    return BadRequest(new ApiResponse(400, "TrainerId claim not found in the token."));
                }

                classDto.TrainerId = trainerIdClaim.Value;
                var response = await _classRepo.AddClass(classDto);

                if (response.StatusCode == 201)
                {
                    _logger.LogInformation("Class {ClassName} added successfully.", classDto.ClassName);
                    return Ok(response);
                }

                _logger.LogWarning("Failed to add class {ClassName}: {Message}", classDto.ClassName, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding class with name: {ClassName}", classDto.ClassName);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while adding the class", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Trainer")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for DeleteClass with ID: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Attempting to delete class with ID: {Id}", id);
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

        [Authorize(Roles = "Admin,Receptionist,Trainer")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] ClassDto classDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateClass with ID: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Attempting to update class with ID: {Id}", id);
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
    }
}
