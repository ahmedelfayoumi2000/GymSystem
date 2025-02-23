
using GymSystem.BLL.Dtos.Role;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GymSystem.DAL.Entities.Enums.Auth;

namespace GymMangamentSystem.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            ILogger<UserController> logger,
            IMapper mapper)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper;
        }

     
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users");

                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = roles.ToList(),
                        UserCode = user.UserCode
                    });
                }

                _logger.LogInformation("Successfully retrieved {UserCount} users", userDtos.Count);
                return Ok(new ApiResponse(200, "Users retrieved successfully", userDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return HandleException(ex);
            }
        }

        [HttpGet("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("GetUser called with null or empty ID");
                return BadRequest(new ApiResponse(400, "User ID is required"));
            }

            try
            {
                _logger.LogInformation("Fetching user with ID: {UserId}", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(new ApiResponse(404, $"User with ID {id} not found"));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList(),
                    UserCode = user.UserCode
                };

                _logger.LogInformation("User retrieved successfully with ID: {UserId}", id);
                return Ok(new ApiResponse(200, "User retrieved successfully", userDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
                return HandleException(ex);
            }
        }

      
        [HttpPut("users/roles/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateUserRoles(string id, [FromBody] UserRoleDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateUserRoles with ID: {UserId}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });
            }

            if (id != model.UserId)
            {
                _logger.LogWarning("Mismatch between route ID {RouteId} and model UserId {ModelUserId}", id, model.UserId);
                return BadRequest(new ApiResponse(400, "User ID in route and model must match"));
            }

            try
            {
                _logger.LogInformation("Updating roles for user with ID: {UserId}", id);

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(new ApiResponse(404, $"User with ID {id} not found"));
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var updatedRoles = model.Roles
                    .Where(r => r.IsSelected)
                    .Select(r => r.Name)
                    .ToList();

                var rolesToAdd = updatedRoles.Except(currentRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to add roles to user ID {UserId}: {Errors}", id, errors);
                        return BadRequest(new ApiResponse(400, $"Failed to add roles: {errors}"));
                    }
                }

                var rolesToRemove = currentRoles.Except(updatedRoles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to remove roles from user ID {UserId}: {Errors}", id, errors);
                        return BadRequest(new ApiResponse(400, $"Failed to remove roles: {errors}"));
                    }
                }

                _logger.LogInformation("User roles updated successfully for ID: {UserId}", id);
                return Ok(new ApiResponse(200, "User roles updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating roles for user with ID: {UserId}", id);
                return HandleException(ex);
            }
        }
        [HttpPost("users")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> AddUser([FromBody] Register model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddUser with Email: {Email}", model?.Email);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            try
            {
                _logger.LogInformation("Adding new user with Email: {Email}", model.Email);

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User with Email {Email} already exists", model.Email);
                    return Conflict(new ApiResponse(409, $"User with email '{model.Email}' already exists"));
                }

                var user = new AppUser
                {
                    DisplayName = model.DisplayName,
                    Email = model.Email,
                    UserName = model.Email.Split('@')[0],
                    UserRole = (int)model.UserRole,
                    EmailConfirmed = true // Admin عندو صلاحية إنشاء مستخدمين مؤكدين مباشرة
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create user with Email {Email}: {Errors}", model.Email, errors);
                    return BadRequest(new ApiResponse(400, $"Failed to create user: {errors}"));
                }

                var roleName = GetRoleNameFromEnum(model.UserRole);
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user); // Rollback
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to assign role {RoleName} to user {Email}: {Errors}", roleName, model.Email, errors);
                    return BadRequest(new ApiResponse(400, $"Failed to assign role: {errors}"));
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = new List<string> { roleName }
                };

                _logger.LogInformation("User added successfully with ID: {UserId}", user.Id);
                return StatusCode(StatusCodes.Status201Created, new ApiResponse(201, "User created successfully", userDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user with Email: {Email}", model.Email);
                return HandleException(ex);
            }
        }

        [HttpPut("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateUser(string id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateUser with ID: {UserId}", id);
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });
            }

            if (id != userDto.Id)
            {
                _logger.LogWarning("Mismatch between route ID {RouteId} and model ID {ModelId}", id, userDto.Id);
                return BadRequest(new ApiResponse(400, "User ID in route and model must match"));
            }

            try
            {
                _logger.LogInformation("Updating user details for ID: {UserId}", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(new ApiResponse(404, $"User with ID {id} not found"));
                }

                user.DisplayName = userDto.DisplayName;
                user.Email = userDto.Email;
                user.UserName = userDto.UserName;
                user.PhoneNumber = userDto.PhoneNumber;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to update user details for ID {UserId}: {Errors}", id, errors);
                    return BadRequest(new ApiResponse(400, $"Failed to update user: {errors}"));
                }

                _logger.LogInformation("User details updated successfully for ID: {UserId}", id);
                return Ok(new ApiResponse(200, "User updated successfully", _mapper.Map<UserDto>(user)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user details for ID: {UserId}", id);
                return HandleException(ex);
            }
        }

        [HttpDelete("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("DeleteUser called with null or empty ID");
                return BadRequest(new ApiResponse(400, "User ID is required"));
            }

            try
            {
                _logger.LogInformation("Deleting user with ID: {UserId}", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(new ApiResponse(404, $"User with ID {id} not found"));
                }

                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to delete user with ID {UserId}: {Errors}", id, errors);
                    return BadRequest(new ApiResponse(400, $"Failed to delete user: {errors}"));
                }

                _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
                return Ok(new ApiResponse(200, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                return HandleException(ex);
            }
        }


        private string GetRoleNameFromEnum(UserRoleEnum role)
        {
            return role switch
            {
                UserRoleEnum.Admin => "Admin",
                UserRoleEnum.Trainer => "Trainer",
                UserRoleEnum.Member => "Member",
                UserRoleEnum.Receptionist => "Receptionist",
                _ => throw new ArgumentException("Invalid user role", nameof(role))
            };
        }
        private ActionResult<ApiResponse> HandleException(Exception ex)
        {
            return StatusCode(500, new ApiExceptionResponse(500, "An unexpected error occurred", ex.Message));
        }
    }
}