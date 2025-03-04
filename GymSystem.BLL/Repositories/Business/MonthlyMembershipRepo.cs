using AutoMapper;
using GymSystem.BLL.Dtos.MonthlyMembership;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.BLL.Specifications;
using GymSystem.BLL.Specifications.MonthlyMembershipWithRelationsSpeci;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GymSystem.BLL.Repositories
{
    public class MonthlyMembershipRepo : IMonthlyMembershipRepo
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;

		public MonthlyMembershipRepo(IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_userRepository = userRepository;
		}

		public async Task<IReadOnlyList<MonthlyMembershipViewDto>> GetAllAsync(SpecPrams specParams = null)
		{
			try
			{
				ISpecification<MonthlyMembershipp> spec = specParams != null
					? new MonthlyMembershipWithFiltersSpecification(specParams)
					: new MonthlyMembershipWithRelationsSpecification();

				var memberships = await _unitOfWork.Repository<MonthlyMembershipp>()
					.GetAllWithSpecAsync(spec);

				var membershipDtos = memberships.Select(m => _mapper.Map<MonthlyMembershipViewDto>(m)).ToList();
				return membershipDtos.AsReadOnly();
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to retrieve monthly memberships from the database.", ex);
			}
		}

		public async Task<MonthlyMembershipViewDto> GetByEmailAsync(string email)
		{
			try
			{
				var membership = await _unitOfWork.Repository<MonthlyMembershipp>()
					.GetByEmailAsync<MonthlyMembershipp>(email);

				return membership == null ? null : _mapper.Map<MonthlyMembershipViewDto>(membership);
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to retrieve monthly membership with this email {email} from the database.", ex);
			}
		}

		public async Task<ApiResponse> CreateAsync(MonthlyMembershipCreateDto membershipDto)
		{
			if (membershipDto == null)
			{
				return new ApiResponse(400, "Monthly membership data cannot be null.");
			}

			try
			{ 
				var membership = await _unitOfWork.Repository<MonthlyMembershipp>()
				.GetByEmailAsync<MonthlyMembershipp>(membershipDto.UserEmail);
				if (membership != null)
				{
					return new ApiResponse(409, "there is a membership with this email .");
				}

				// ✅ جلب بيانات الخطة بناءً على PlanId
				var planEntity = await _unitOfWork.Repository<Plan>().GetByIdAsync(membershipDto.PlanId);
				if (planEntity == null)
				{
					return new ApiResponse(404, "Plan not found.");
				}

				 membership = _mapper.Map<MonthlyMembershipp>(membershipDto);
				 membership.Plan = planEntity;
				membership.EndDate = membership.StartDate.AddDays(planEntity.DurationDays);

				await _unitOfWork.Repository<MonthlyMembershipp>().Add(membership);
				var result = await _unitOfWork.Complete();

				if (result <= 0)
				{
					return new ApiResponse(500, "Failed to save the monthly membership to the database.");
				}

				var createdDto = _mapper.Map<MonthlyMembershipViewDto>(membership);
				return new ApiResponse(201, "Monthly membership created successfully", createdDto);
			}
			catch (Exception ex)
			{
				return new ApiExceptionResponse(500, "An unexpected error occurred while creating the monthly membership.", ex.Message);
			}
		}


		public async Task<ApiResponse> UpdateAsync(string email, MonthlyMembershipCreateDto membershipDto)
		{
			if (string.IsNullOrEmpty(email))
			{
				return new ApiResponse(400, "Monthly membership Email must be a value.");
			}

			try
			{
				var existingMembership = await _unitOfWork.Repository<MonthlyMembershipp>()
					.GetByEmailAsync<MonthlyMembershipp>(email);

				if (existingMembership == null)
				{
					return new ApiResponse(404, $"Membership with email {email} not found.");
				}

				// ✅ تحديث بيانات الخطة إذا تم تغيير الـ PlanId
				if (existingMembership.Plan == null || existingMembership.Plan.Id != membershipDto.PlanId)
				{
					var newPlan = await _unitOfWork.Repository<Plan>().GetByIdAsync(membershipDto.PlanId);
					if (newPlan == null)
					{
						return new ApiResponse(404, "New plan not found.");
					}

					existingMembership.Plan = newPlan;

					// ✅ تحديث EndDate بعد تغيير الخطة
					existingMembership.EndDate = existingMembership.StartDate.AddDays(newPlan.DurationDays);
				}

				_mapper.Map(membershipDto, existingMembership);
				_unitOfWork.Repository<MonthlyMembershipp>().Update(existingMembership);

				var result = await _unitOfWork.Complete();
				if (result <= 0)
				{
					return new ApiResponse(500, "Failed to update the monthly membership.");
				}

				var updatedDto = _mapper.Map<MonthlyMembershipViewDto>(existingMembership);
				return new ApiResponse(200, "Monthly membership updated successfully", updatedDto);
			}
			catch (Exception ex)
			{
				return new ApiExceptionResponse(500, "An error occurred while updating the membership.", ex.Message);
			}
		}



		public async Task<ApiResponse> DeleteAsync(string email)
		{
			if (email == null)
			{
				return new ApiResponse(400, "Monthly membership Email must be a VAlue.");
			}

			try
			{
				var membership = await _unitOfWork.Repository<MonthlyMembershipp>()
				.GetByEmailAsync<MonthlyMembershipp>(email);

				if (membership == null)
				{
					return new ApiResponse(404, $"Monthly membership with Email {email} not found.");
				}

				_unitOfWork.Repository<MonthlyMembershipp>().Delete(membership);

				var result = await _unitOfWork.Complete();
				if (result <= 0)
				{
					return new ApiResponse(500, "Failed to delete the monthly membership from the database.");
				}

				return new ApiResponse(200, "Monthly membership deleted successfully");
			}
			catch (Exception ex)
			{
				return new ApiExceptionResponse(500, "An error occurred while deleting the monthly membership", ex.Message);
			}
		}
	}


}