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
    public class BMIRecordController : BaseApiController
    {
        private readonly IBMIRecordRepo _bmiRecordRepo;

        public BMIRecordController(IBMIRecordRepo bmiRecordRepo)
        {
            _bmiRecordRepo = bmiRecordRepo ?? throw new ArgumentNullException(nameof(bmiRecordRepo));
        }

        /// <summary>
        /// Retrieves all BMI records for the authenticated user.
        /// </summary>
        /// <returns>A list of BMI records if successful, or an error response.</returns>
        [Authorize(Roles = "Admin,Trainer,Member")]
        [HttpGet("getBMIRecordsForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBMIRecordsForUser()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
                {
                    return BadRequest(new ApiResponse(400, "UserId claim is missing or invalid in the token."));
                }

                var result = await _bmiRecordRepo.GetBMIRecordsForUser(userIdClaim.Value);
                return Ok(new ApiResponse(200, "BMI records retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving BMI records", ex.Message));
            }
        }

        /// <summary>
        /// Adds a new BMI record for a specified user.
        /// </summary>
        /// <param name="bmiRecord">The BMI record data to add.</param>
        /// <returns>The result of the operation, including the added record if successful.</returns>
        [Authorize(Roles = "Admin,Trainer")]
        [HttpPost("addBMIRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBMIRecord([FromBody] BMIRecordDto bmiRecord)
        {
            if (!ModelState.IsValid || bmiRecord == null)
            {
                return BadRequest(CreateValidationErrorResponse("Invalid BMI record data"));
            }

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
                {
                    return BadRequest(new ApiResponse(400, "UserId claim is missing or invalid in the token."));
                }

                var result = await _bmiRecordRepo.AddBMIRecord(bmiRecord);
                return result.StatusCode == 200
                    ? Ok(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while adding BMI record", ex.Message));
            }
        }

        /// <summary>
        /// Deletes a BMI record by its ID.
        /// </summary>
        /// <param name="id">The ID of the BMI record to delete.</param>
        /// <returns>The result of the operation, including confirmation if successful.</returns>
        [Authorize(Roles = "Admin,Trainer")]
        [HttpDelete("deleteBMIRecord/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBMIRecord(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "BMI record ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                var result = await _bmiRecordRepo.DeleteBMIRecord(id);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while deleting BMI record", ex.Message));
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