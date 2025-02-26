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
   
    public class MembershipController : BaseApiController
    {
        private readonly IMembershipRepo _membershipRepo;
        private readonly ILogger<MembershipController> _logger;

        public MembershipController(IMembershipRepo membershipRepo, ILogger<MembershipController> logger)
        {
            _membershipRepo = membershipRepo ?? throw new ArgumentNullException(nameof(membershipRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("GetAllMemberships")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllMemberships()
        {
            try
            {
                _logger.LogInformation("Fetching all active memberships by user with role: {Roles}", User.FindFirst(ClaimTypes.Role)?.Value);
                var memberships = await _membershipRepo.GetAllMemberships();

                _logger.LogInformation("Successfully retrieved {Count} active memberships.", memberships.Count());
                return Ok(new ApiResponse(200, "Memberships retrieved successfully", memberships));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all memberships.");
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving memberships", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Member")]
        [HttpGet("GetMembershipById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMembershipById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid membership ID provided for GetMembershipById: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Membership ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Fetching membership with ID: {Id} by user with role: {Roles}", id, User.FindFirst(ClaimTypes.Role)?.Value);
                var membership = await _membershipRepo.GetMembershipById(id);

                if (membership == null)
                {
                    _logger.LogWarning("Membership with ID {Id} not found.", id);
                    return NotFound(new ApiResponse(404, $"Membership with ID {id} not found"));
                }

                if (User.IsInRole("Member"))
                {
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim == null || membership.UserId != userIdClaim.Value)
                    {
                        _logger.LogWarning("Member attempted to access unauthorized membership with ID: {Id}", id);
                        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, "You can only view your own membership."));
                    }
                }

                _logger.LogInformation("Membership with ID {Id} retrieved successfully.", id);
                return Ok(new ApiResponse(200, "Membership retrieved successfully", membership));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving membership with ID: {Id}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while retrieving the membership", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateMembership([FromBody] MembershipDto membership)
        {
            if (!ModelState.IsValid || membership == null)
            {
                _logger.LogWarning("Invalid model state or null data for CreateMembership.");
                return BadRequest(CreateValidationErrorResponse("Invalid membership data"));
            }

            try
            {
                _logger.LogInformation("Attempting to create membership for UserId: {UserId} by user with role: {Roles}", membership.UserId, User.FindFirst(ClaimTypes.Role)?.Value);
                var response = await _membershipRepo.CreateMembership(membership);

                if (response.StatusCode == 201)
                {
                    _logger.LogInformation("Membership for UserId {UserId} created successfully.", membership.UserId);
                    return StatusCode(StatusCodes.Status201Created, response);
                }

                _logger.LogWarning("Failed to create membership for UserId {UserId}: {Message}", membership.UserId, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating membership for UserId: {UserId}", membership?.UserId);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while creating the membership", ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMembership(int id, [FromBody] MembershipDto membership)
        {
            if (!ModelState.IsValid || membership == null)
            {
                _logger.LogWarning("Invalid model state or null data for UpdateMembership with ID: {Id}", id);
                return BadRequest(CreateValidationErrorResponse("Invalid membership data"));
            }

            if (id <= 0)
            {
                _logger.LogWarning("Invalid membership ID provided for UpdateMembership: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Membership ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Attempting to update membership with ID: {Id} by user with role: {Roles}", id, User.FindFirst(ClaimTypes.Role)?.Value);
                var response = await _membershipRepo.UpdateMembership(id, membership);

                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Membership with ID {Id} updated successfully.", id);
                    return Ok(response);
                }

                if (response.StatusCode == 404)
                {
                    _logger.LogWarning("Membership with ID {Id} not found.", id);
                    return NotFound(response);
                }

                _logger.LogWarning("Failed to update membership with ID {Id}: {Message}", id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating membership with ID: {Id}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while updating the membership", ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMembership(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid membership ID provided for DeleteMembership: {Id}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Membership ID must be a positive integer." },
                    StatusCode = 400,
                    Message = "Invalid request data"
                });
            }

            try
            {
                _logger.LogInformation("Attempting to delete membership with ID: {Id} by Admin.", id);
                var response = await _membershipRepo.DeleteMembership(id);

                if (response.StatusCode == 200)
                {
                    _logger.LogInformation("Membership with ID {Id} deleted successfully.", id);
                    return Ok(response);
                }

                if (response.StatusCode == 404)
                {
                    _logger.LogWarning("Membership with ID {Id} not found.", id);
                    return NotFound(response);
                }

                _logger.LogWarning("Failed to delete membership with ID {Id}: {Message}", id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting membership with ID: {Id}", id);
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while deleting the membership", ex.Message));
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