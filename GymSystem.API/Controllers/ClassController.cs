using AutoMapper;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GymSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : BaseApiController
    {
        private readonly IClassRepo _classRepo;

        public ClassController(IClassRepo classRepo)
        {
            _classRepo = classRepo ?? throw new ArgumentNullException(nameof(classRepo));
        }

        /// <summary>
        /// Retrieves a specific class by its ID.
        /// </summary>
        /// <param name="id">The ID of the class to retrieve.</param>
        /// <returns>The class details if found, or an error response.</returns>
        [Authorize(Roles = "Admin,Receptionist,Trainer,Member")]
        [HttpGet("getClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClass(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Class ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                var classDto = await _classRepo.GetClass(id);
                if (classDto == null)
                {
                    return NotFound(new ApiResponse(404, $"Class with ID {id} not found"));
                }

                return Ok(new ApiResponse(200, "Class retrieved successfully", classDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving the class", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all active classes in the system.
        /// </summary>
        /// <returns>A list of all active classes if successful, or an error response.</returns>
        [Authorize(Roles = "Admin,Receptionist,Trainer,Member")]
        [HttpGet("getAllClasses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                var classes = await _classRepo.GetClasses();
                return Ok(new ApiResponse(200, "Classes retrieved successfully", classes));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving classes", ex.Message));
            }
        }

        /// <summary>
        /// Adds a new class to the system.
        /// </summary>
        /// <param name="classDto">The class data to add.</param>
        /// <returns>The result of the operation, including the added class if successful.</returns>
        [Authorize(Roles = "Admin,Trainer")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddClass([FromBody] ClassDto classDto)
        {
            if (!ModelState.IsValid || classDto == null)
            {
                return BadRequest(CreateValidationErrorResponse("Invalid class data"));
            }

            try
            {
                if (User.IsInRole("Trainer"))
                {
                    var trainerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TrainerId");
                    if (trainerIdClaim == null || string.IsNullOrWhiteSpace(trainerIdClaim.Value))
                    {
                        return BadRequest(new ApiResponse(400, "TrainerId claim is missing or invalid in the token."));
                    }
                    if (classDto.TrainerId != trainerIdClaim.Value)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, "Trainers can only create classes for themselves."));
                    }
                }

                var response = await _classRepo.AddClass(classDto);
                return response.StatusCode == 201
                    ? StatusCode(StatusCodes.Status201Created, response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while adding the class", ex.Message));
            }
        }

        /// <summary>
        /// Deletes a specific class by its ID.
        /// </summary>
        /// <param name="id">The ID of the class to delete.</param>
        /// <returns>The result of the operation, including confirmation if successful.</returns>
        [Authorize(Roles = "Admin,Trainer")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Class ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                var classDto = await _classRepo.GetClass(id);
                if (classDto == null)
                {
                    return NotFound(new ApiResponse(404, $"Class with ID {id} not found"));
                }

                if (User.IsInRole("Trainer"))
                {
                    var trainerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TrainerId");
                    if (trainerIdClaim == null || string.IsNullOrWhiteSpace(trainerIdClaim.Value))
                    {
                        return BadRequest(new ApiResponse(400, "TrainerId claim is missing or invalid in the token."));
                    }
                    if (classDto.TrainerId != trainerIdClaim.Value)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, "Trainers can only delete their own classes."));
                    }
                }

                var response = await _classRepo.DeleteClass(id);
                return response.StatusCode == 200
                    ? Ok(response)
                    : response.StatusCode == 404
                        ? NotFound(response)
                        : BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while deleting the class", ex.Message));
            }
        }

        /// <summary>
        /// Updates an existing class by its ID.
        /// </summary>
        /// <param name="id">The ID of the class to update.</param>
        /// <param name="classDto">The updated class data.</param>
        /// <returns>The result of the operation, including the updated class if successful.</returns>
        [Authorize(Roles = "Admin,Trainer")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] ClassDto classDto)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Class ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            if (!ModelState.IsValid || classDto == null)
            {
                return BadRequest(CreateValidationErrorResponse("Invalid class data"));
            }

            try
            {
                var existingClass = await _classRepo.GetClass(id);
                if (existingClass == null)
                {
                    return NotFound(new ApiResponse(404, $"Class with ID {id} not found"));
                }

                if (User.IsInRole("Trainer"))
                {
                    var trainerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TrainerId");
                    if (trainerIdClaim == null || string.IsNullOrWhiteSpace(trainerIdClaim.Value))
                    {
                        return BadRequest(new ApiResponse(400, "TrainerId claim is missing or invalid in the token."));
                    }
                    if (existingClass.TrainerId != trainerIdClaim.Value || classDto.TrainerId != trainerIdClaim.Value)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, "Trainers can only update their own classes."));
                    }
                }

                var response = await _classRepo.UpdateClass(id, classDto);
                return response.StatusCode == 200
                    ? Ok(response)
                    : response.StatusCode == 404
                        ? NotFound(response)
                        : BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while updating the class", ex.Message));
            }
        }

        #region Helper Methods
        private ApiValidationErrorResponse CreateValidationErrorResponse(string message)
        {
            return new ApiValidationErrorResponse
            {
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                StatusCode = 400,
                Message = message
            };
        }
        #endregion
    }
}