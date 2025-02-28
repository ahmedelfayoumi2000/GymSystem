using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Auth;
using GymSystem.DAL.Entities.Identity;
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
    public class AuthController : BaseApiController
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

      
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid || request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                                     .Append(string.IsNullOrWhiteSpace(request?.RefreshToken) ? "Refresh token is required." : null)
                                     .Where(e => e != null)
                                     .ToList(),
                    StatusCode = 400,
                    Message = "Invalid token request data"
                });
            }

            try
            {
                var (newJwtToken, newRefreshToken) = await _tokenService.RefreshTokenAsync(request.RefreshToken);

                if (string.IsNullOrWhiteSpace(newJwtToken) || newRefreshToken == null || string.IsNullOrWhiteSpace(newRefreshToken.Token))
                {
                    return StatusCode(500, new ApiExceptionResponse(500, "Token refresh failed due to internal error."));
                }

                return Ok(new TokenResponse
                {
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken.Token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while refreshing the token", ex.Message));
            }
        }

     
        [Authorize]
        [HttpPost("revoke-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            if (!ModelState.IsValid || request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                                     .Append(string.IsNullOrWhiteSpace(request?.RefreshToken) ? "Refresh token is required." : null)
                                     .Where(e => e != null)
                                     .ToList(),
                    StatusCode = 400,
                    Message = "Invalid token revocation request data"
                });
            }

            try
            {
                var success = await _tokenService.RevokeTokenAsync(request.RefreshToken);
                if (!success)
                {
                    return BadRequest(new ApiResponse(400, "Invalid or already revoked token."));
                }

                return Ok(new ApiResponse(200, "Token revoked successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiExceptionResponse(500, "An error occurred while revoking the token", ex.Message));
            }
        }
    }

   
}