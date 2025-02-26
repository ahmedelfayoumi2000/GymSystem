using AutoMapper;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.BLL.Interfaces;
using GymSystem.BLL.Specifications;
using GymSystem.DAL.Entities;
using Microsoft.Extensions.Logging;

namespace GymSystem.BLL.Repositories.Business
{
    public class MembershipRepository : IMembershipRepo
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MembershipRepository> _logger;

        public MembershipRepository(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MembershipRepository> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse> CreateMembership(MembershipDto membership)
        {
            if (membership == null)
            {
                _logger.LogWarning("Attempted to create a null MembershipDto.");
                return new ApiResponse(400, "Membership data cannot be null.");
            }

            try
            {
                _logger.LogInformation("Attempting to create membership for UserId: {UserId}", membership.UserId);

                var spec = new BaseSpecification<Membership>(m => m.UserId == membership.UserId && !m.IsDeleted);
                var existingMembership = await _unitOfWork.Repository<Membership>().GetByIdWithSpecAsync(spec);
                if (existingMembership != null)
                {
                    _logger.LogWarning("Membership for UserId {UserId} already exists.", membership.UserId);
                    return new ApiResponse(409, "User already has an active membership. Please update the existing membership.");
                }

                var membershipEntity = _mapper.Map<Membership>(membership);
                await _unitOfWork.Repository<Membership>().Add(membershipEntity);
                await _unitOfWork.Complete();

                _logger.LogInformation("Membership for UserId {UserId} created successfully with ID: {Id}", membership.UserId, membershipEntity.Id);
                return new ApiResponse(201, "Membership added successfully", _mapper.Map<MembershipDto>(membershipEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating membership for UserId: {UserId}", membership.UserId);
                return new ApiResponse(500, $"Failed to add membership: {ex.Message}");
            }
        }

      
        public async Task<ApiResponse> DeleteMembership(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete membership with ID: {Id}", id);

                var spec = new BaseSpecification<Membership>(m => m.Id == id && !m.IsDeleted);
                var membership = await _unitOfWork.Repository<Membership>().GetByIdWithSpecAsync(spec);
                if (membership == null)
                {
                    _logger.LogWarning("Membership with ID {Id} not found or already deleted.", id);
                    return new ApiResponse(404, $"Membership with ID {id} not found.");
                }

                membership.IsDeleted = true;
                _unitOfWork.Repository<Membership>().Update(membership);
                await _unitOfWork.Complete();

                _logger.LogInformation("Membership with ID {Id} deleted successfully.", id);
                return new ApiResponse(200, "Membership deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting membership with ID: {Id}", id);
                return new ApiResponse(500, $"Failed to delete membership: {ex.Message}");
            }
        }

       
        public async Task<IEnumerable<MembershipDto>> GetAllMemberships()
        {
            try
            {
                _logger.LogInformation("Retrieving all active memberships.");

                var spec = new BaseSpecification<Membership>(m => !m.IsDeleted);
                var memberships = await _unitOfWork.Repository<Membership>().GetAllWithSpecAsync(spec);
                var membershipDtos = _mapper.Map<IEnumerable<MembershipDto>>(memberships);

                _logger.LogInformation("Retrieved {Count} active memberships.", membershipDtos.Count());
                return membershipDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all memberships.");
                throw new ApplicationException($"Failed to retrieve memberships: {ex.Message}", ex);
            }
        }

       
        public async Task<MembershipDto> GetMembershipById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving membership with ID: {Id}", id);

                var spec = new BaseSpecification<Membership>(m => m.Id == id && !m.IsDeleted);
                var membership = await _unitOfWork.Repository<Membership>().GetByIdWithSpecAsync(spec);
                if (membership == null)
                {
                    _logger.LogWarning("Membership with ID {Id} not found.", id);
                    return null;
                }

                var membershipDto = _mapper.Map<MembershipDto>(membership);
                _logger.LogInformation("Membership with ID {Id} retrieved successfully.", id);
                return membershipDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving membership with ID: {Id}", id);
                throw new ApplicationException($"Failed to retrieve membership: {ex.Message}", ex);
            }
        }

       
        public async Task<ApiResponse> UpdateMembership(int id, MembershipDto membership)
        {
            if (membership == null)
            {
                _logger.LogWarning("Attempted to update membership with ID {Id} using null MembershipDto.", id);
                return new ApiResponse(400, "Membership data cannot be null.");
            }

            try
            {
                _logger.LogInformation("Attempting to update membership with ID: {Id}", id);

                var spec = new BaseSpecification<Membership>(m => m.Id == id && !m.IsDeleted);
                var existingMembership = await _unitOfWork.Repository<Membership>().GetByIdWithSpecAsync(spec);
                if (existingMembership == null)
                {
                    _logger.LogWarning("Membership with ID {Id} not found or already deleted.", id);
                    return new ApiResponse(404, $"Membership with ID {id} not found.");
                }

                _mapper.Map(membership, existingMembership); 
                _unitOfWork.Repository<Membership>().Update(existingMembership);
                await _unitOfWork.Complete();

                _logger.LogInformation("Membership with ID {Id} updated successfully.", id);
                return new ApiResponse(200, "Membership updated successfully", _mapper.Map<MembershipDto>(existingMembership));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating membership with ID: {Id}", id);
                return new ApiResponse(500, $"Failed to update membership: {ex.Message}");
            }
        }
    }
}
