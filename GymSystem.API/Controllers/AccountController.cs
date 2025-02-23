using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Auth;
using GymSystem.DAL.Entities.Enums.Auth;
using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
namespace GymSystem.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        public AccountController(IAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.LoginAsync(dto);

            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.RegisterAsync(model, GenerateCallBackUrl);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.ForgetPassword(email);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp(VerifyOtp dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _accountService.VerfiyOtp(dto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.ResetPasswordAsync(dto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }


            var result = await _accountService.ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.ResendConfirmationEmailAsync(email, GenerateCallBackUrl);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail([FromQuery] string userId, [FromQuery] string confirmationToken)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(confirmationToken))
            {
                return BadRequest(new ApiResponse(400, "User ID and confirmation token are required."));
            }

            var (succeeded, errorMessage) = await _accountService.ConfirmUserEmailAsync(userId, confirmationToken);

            if (succeeded)
            {
                return RedirectPermanent("https://www.google.com/webhp?authuser=0");
            }
            else
            {
                return BadRequest(new ApiResponse(400, errorMessage));
            }
        }

        // Helper Method
        private string GenerateCallBackUrl(string token, string userId)
        {
            var baseUrl = _configuration["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var encodedToken = Uri.EscapeDataString(token);
            var encodedUserId = Uri.EscapeDataString(userId);
            var callBackUrl = $"{baseUrl}/api/Account/confirm-email?userId={encodedUserId}&confirmationToken={encodedToken}";
            return callBackUrl;
        }
    }
}