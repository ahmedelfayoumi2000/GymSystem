using AutoMapper;
using GymMangamentSystem.Apis.Controllers;
using GymSystem.API.DTOs.Trainer;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Specifications;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace GymSystem.API.Controllers
{

	/// <summary>
	/// إضافة موظف وعرض الموظفين
	/// </summary>
	public class EmployeeController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<UserController> _logger;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;

		public EmployeeController(UserManager<AppUser> userManager,
			                      ILogger<UserController> logger,
			                      RoleManager<IdentityRole> roleManager,
			                      IMapper mapper)
		{
			_userManager = userManager;
			_logger = logger;
			_roleManager = roleManager;
			_mapper = mapper;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllEmployees([FromQuery] SpecPrams specParams)
		{
			try
			{
				var employees = _userManager.Users
					.Where(u => u.UserRole == 1 || u.UserRole == 2 || u.UserRole == 3) // Admin, Trainer, Receptionist
					.Where(u => !u.IsDeleted);

				if (!string.IsNullOrEmpty(specParams.Search))
				{
					employees = employees.Where(u => u.DisplayName.ToLower().Contains(specParams.Search.ToLower()));
				}

				if (!string.IsNullOrEmpty(specParams.Sort))
				{
					employees = specParams.Sort.ToLower() switch
					{
						"name" => employees.OrderBy(u => u.DisplayName),
						"namedesc" => employees.OrderByDescending(u => u.DisplayName),
						_ => employees.OrderBy(u => u.Id)
					};
				}

				var totalItems = employees.Count();
				if (specParams.PageSize > 0)
				{
					employees = employees.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize);
				}

				var employeeList = employees.Select(u => new EmployeeDto
				{
					Id = u.Id,
					DisplayName = u.DisplayName,
					Email = u.Email,
					UserRole = u.UserRole,
					Gender = u.Gender,
					Salary=u.Salary
				}).ToList();

				return Ok(new ApiResponse(200, "Employees retrieved successfully", new { Items = employeeList, TotalItems = totalItems }));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new ApiExceptionResponse(500, "An error occurred while retrieving employees", ex.Message));
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto employeeDto)
		{
			if (!ModelState.IsValid || employeeDto == null)
			{
				return BadRequest(new ApiValidationErrorResponse
				{
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
					StatusCode = 400,
					Message = "Invalid employee data"
				});
			}

			try
			{
				var user = new AppUser
				{
					UserName = employeeDto.DisplayName,
					DisplayName = employeeDto.DisplayName,
					Email = employeeDto.Email,
					UserRole = employeeDto.UserRole,
					Gender = employeeDto.Gender,
					Salary=employeeDto.Salary
				};

				var result = await _userManager.CreateAsync(user, employeeDto.PassWord); 
				if (!result.Succeeded)
				{
					return BadRequest(new ApiResponse(400, "Failed to create employee.", result.Errors));
				}

				var role = employeeDto.UserRole switch
				{
					1 => "Admin",
					2 => "Trainer",
					3 => "Receptionist",
					_ => "Member"
				};
				await _userManager.AddToRoleAsync(user, role);

				return StatusCode(StatusCodes.Status201Created, new ApiResponse(201, "Employee created successfully", new { Id = user.Id , user.UserRole ,Salary=user.Salary}));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new ApiExceptionResponse(500, "An error occurred while creating the employee", ex.Message));
			}
		}


//		[Authorize(Roles = "Admin")]
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateEmployee(string id, [FromBody] EmployeeDto employeeDto)
		{
			if (!ModelState.IsValid || employeeDto == null)
			{
				return BadRequest(new ApiValidationErrorResponse
				{
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
					StatusCode = 400,
					Message = "Invalid employee update data"
				});
			}

			if (id != employeeDto.Id)
			{
				_logger.LogWarning("Mismatch between route ID {RouteId} and model ID {ModelId}", id, employeeDto.Id);
				return BadRequest(new ApiResponse(400, "Employee ID in route and model must match"));
			}

			try
			{
				_logger.LogInformation("Searching for Employee with ID: {EmployeeId}", id);

				var employee = await _userManager.FindByIdAsync(id);
				if (employee == null)
				{
					_logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
					return NotFound(new ApiResponse(404, $"Employee with ID {id} not found"));
				}

				// تحديث البيانات
				employee.DisplayName = employeeDto.DisplayName;
				employee.Gender = employeeDto.Gender;
				employee.Salary = employeeDto.Salary;

				// التحقق من تغيير البريد الإلكتروني
				if (!string.Equals(employee.Email, employeeDto.Email, StringComparison.OrdinalIgnoreCase))
				{
					var existingUser = await _userManager.FindByEmailAsync(employeeDto.Email);
					if (existingUser != null && existingUser.Id != employee.Id)
					{
						return BadRequest(new ApiResponse(400, "Email is already in use by another user."));
					}
					employee.Email = employeeDto.Email;
					employee.NormalizedEmail = employeeDto.Email.ToUpper();
				}

				// إزالة جميع الأدوار القديمة وإضافة الدور الجديد
				var currentRoles = await _userManager.GetRolesAsync(employee);
				await _userManager.RemoveFromRolesAsync(employee, currentRoles);

				var newRole = employeeDto.UserRole switch
				{
					1 => "Admin",
					2 => "Trainer",
					3 => "Receptionist",
					_ => "Member"
				};

				var roleExists = await _roleManager.RoleExistsAsync(newRole);
				if (!roleExists)
				{
					return BadRequest(new ApiResponse(400, $"Role '{newRole}' does not exist."));
				}

				await _userManager.AddToRoleAsync(employee, newRole);

				// حفظ التعديلات
				var updateResult = await _userManager.UpdateAsync(employee);
				if (!updateResult.Succeeded)
				{
					var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
					_logger.LogError("Failed to update Employee details for ID {EmployeeId}: {Errors}", id, errors);
					return BadRequest(new ApiResponse(400, $"Failed to update Employee: {errors}"));
				}

				_logger.LogInformation("Employee details updated successfully for ID: {EmployeeId}", id);
				return Ok(new ApiResponse(200, "User updated successfully"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating employee details for ID: {EmployeeId}", id);
				return StatusCode(500, new ApiResponse(500, "An unexpected error occurred."));
			}
		}



	}

}