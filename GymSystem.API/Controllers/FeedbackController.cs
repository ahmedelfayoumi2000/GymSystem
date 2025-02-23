using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymSystem.API.Controllers
{
    public class FeedbackController : BaseApiController
    {
        private readonly IFeedbackRepo _feedbackRepo;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            IFeedbackRepo feedbackRepo,
            ILogger<FeedbackController> logger)
        {
            _feedbackRepo = feedbackRepo ?? throw new ArgumentNullException(nameof(feedbackRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("getFeedback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFeedback([FromQuery] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for GetFeedback with ID: {FeedbackId}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                _logger.LogInformation("Fetching feedback with ID: {FeedbackId}", id);
                var feedback = await _feedbackRepo.GetFeedbackByIdAsync(id);

                if (feedback == null)
                {
                    _logger.LogWarning("Feedback with ID {FeedbackId} not found.", id);
                    return NotFound(new ApiResponse(404, $"Feedback with ID {id} not found."));
                }

                _logger.LogInformation("Feedback retrieved successfully with ID: {FeedbackId}", id);
                return Ok(new ApiResponse(200, "Feedback retrieved successfully", feedback));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback with ID: {FeedbackId}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving feedback", ex.Message));
            }
        }

        [HttpGet("getAllFeedbacks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                _logger.LogInformation("Fetching all feedbacks");
                var feedbacks = await _feedbackRepo.GetAllFeedbacksAsync();

                _logger.LogInformation("Retrieved {Count} feedbacks successfully", feedbacks.Count);
                return Ok(new ApiResponse(200, "Feedbacks retrieved successfully", feedbacks));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all feedbacks");
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving feedbacks", ex.Message));
            }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateFeedback with data: {@FeedbackDto}", feedbackDto);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                _logger.LogInformation("Creating feedback with data: {@FeedbackDto}", feedbackDto);

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim == null)
                {
                    _logger.LogWarning("UserId claim not found in the token for CreateFeedback request.");
                    return BadRequest(new ApiResponse(400, "UserId claim not found in the token"));
                }

                feedbackDto.UserId = userIdClaim.Value;
                var response = await _feedbackRepo.CreateFeedbackAsync(feedbackDto);

                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Feedback created successfully for UserId: {UserId}", feedbackDto.UserId);
                    return Ok(response);
                }

                _logger.LogWarning("Failed to create feedback for UserId: {UserId}. Response: {Message}", feedbackDto.UserId, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback for UserId: {UserId}", feedbackDto?.UserId);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while creating feedback", ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for DeleteFeedback with ID: {FeedbackId}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                _logger.LogInformation("Deleting feedback with ID: {FeedbackId}", id);
                var response = await _feedbackRepo.DeleteFeedbackAsync(id);

                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Feedback deleted successfully with ID: {FeedbackId}", id);
                    return Ok(response);
                }

                if (response.StatusCode == 404)
                {
                    _logger.LogWarning("Feedback with ID {FeedbackId} not found.", id);
                    return NotFound(response);
                }

                _logger.LogWarning("Failed to delete feedback with ID: {FeedbackId}. Response: {Message}", id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback with ID: {FeedbackId}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while deleting feedback", ex.Message));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateFeedback with ID: {FeedbackId}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                _logger.LogInformation("Updating feedback with ID: {FeedbackId}", id);

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim == null)
                {
                    _logger.LogWarning("UserId claim not found in the token for UpdateFeedback request with ID: {FeedbackId}", id);
                    return BadRequest(new ApiResponse(400, "UserId claim not found in the token"));
                }

                var response = await _feedbackRepo.UpdateFeedbackAsync(id, feedbackDto);

                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Feedback updated successfully with ID: {FeedbackId}", id);
                    return Ok(response);
                }

                if (response.StatusCode == 404)
                {
                    _logger.LogWarning("Feedback with ID {FeedbackId} not found.", id);
                    return NotFound(response);
                }

                _logger.LogWarning("Failed to update feedback with ID: {FeedbackId}. Response: {Message}", id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback with ID: {FeedbackId}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while updating feedback", ex.Message));
            }
        }

    }
}
