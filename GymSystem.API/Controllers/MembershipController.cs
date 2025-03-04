using GymSystem.BLL.Dtos.MonthlyMembership;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.BLL.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymSystem.API.Controllers
{

    //إضافة مشترك
    public class MembershipController : BaseApiController
	{
		private readonly IMonthlyMembershipRepo _membershipRepo;

		public MembershipController(IMonthlyMembershipRepo membershipRepo)
		{
			_membershipRepo = membershipRepo ?? throw new ArgumentNullException(nameof(membershipRepo));
		}

		[Authorize(Roles = "Admin,Receptionist")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllMemberships([FromQuery] SpecPrams specParams)
		{
			try
			{
				var memberships = await _membershipRepo.GetAllAsync(specParams);
				return Ok(new ApiResponse(200, "Memberships retrieved successfully", memberships));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new ApiExceptionResponse(500, "An error occurred while retrieving memberships", ex.Message));
			}
		}

		[Authorize(Roles = "Admin,Receptionist")]
		[HttpGet("{email}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetMembershipByEmail(string email)
		{
			if (email == null)
			{
				return BadRequest(new ApiValidationErrorResponse
				{
					Errors = new List<string> { "Membership email must Have a Value" },
					StatusCode = 400,
					Message = "Invalid request data"
				});
			}

			try
			{
				var membership = await _membershipRepo.GetByEmailAsync(email);
				if (membership == null)
				{
					return NotFound(new ApiResponse(404, $"Membership with email {email} not found"));
				}

				return Ok(new ApiResponse(200, "Membership retrieved successfully", membership));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new ApiExceptionResponse(500, $"An error occurred while retrieving membership with ID {email}", ex.Message));
			}
		}

		[Authorize(Roles = "Admin,Receptionist")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateMembership([FromBody] MonthlyMembershipCreateDto membershipDto)
		{
			if (!ModelState.IsValid || membershipDto == null)
			{
				return BadRequest(new ApiValidationErrorResponse
				{
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
					StatusCode = 400,
					Message = "Invalid membership data"
				});
			}

			try
			{
				var response = await _membershipRepo.CreateAsync(membershipDto);
				return response.StatusCode switch
				{
					201 => StatusCode(StatusCodes.Status201Created, response),
					400 => BadRequest(response),
					500 => StatusCode(StatusCodes.Status500InternalServerError, response),
					_ => StatusCode(response.StatusCode ?? 500, response)
				};
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new ApiExceptionResponse(500, "An error occurred while creating the membership", ex.Message));
			}
		}

		[Authorize(Roles = "Admin,Receptionist")]
		[HttpPut("{email}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateMembership(string email, [FromBody] MonthlyMembershipCreateDto membershipDto)
		{
			if (email == null)
			{
				return BadRequest(new ApiValidationErrorResponse
				{
					Errors = new List<string> { "Membership email must Have a Value" },
					StatusCode = 400,
					Message = "Invalid request data"
				});
			}

			if (!ModelState.IsValid || membershipDto == null)
			{
				return BadRequest(new ApiValidationErrorResponse
				{
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
					StatusCode = 400,
					Message = "Invalid membership data"
				});
			}

			try
			{
				var response = await _membershipRepo.UpdateAsync(email, membershipDto);
				return response.StatusCode switch
				{
					200 => Ok(response),
					404 => NotFound(response),
					400 => BadRequest(response),
					500 => StatusCode(StatusCodes.Status500InternalServerError, response),
					_ => StatusCode(response.StatusCode ?? 500, response)
				};
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new ApiExceptionResponse(500, $"An error occurred while updating membership with ID {email}", ex.Message));
			}
		}

		[Authorize(Roles = "Admin,Receptionist")]
		[HttpDelete("{email}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteMembership(string email)
		{
			if (email == null)
			{
				return BadRequest(new ApiValidationErrorResponse
				{
					Errors = new List<string> { "Membership email must Have a Value" },
					StatusCode = 400,
					Message = "Invalid request data"
				});
			}

			try
			{
				var response = await _membershipRepo.DeleteAsync(email);
				return response.StatusCode switch
				{
					200 => Ok(response),
					404 => NotFound(response),
					400 => BadRequest(response),
					500 => StatusCode(StatusCodes.Status500InternalServerError, response),
					_ => StatusCode(response.StatusCode ?? 500, response)
				};
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new ApiExceptionResponse(500, $"An error occurred while deleting membership with ID {email}", ex.Message));
			}
		}
	}
}