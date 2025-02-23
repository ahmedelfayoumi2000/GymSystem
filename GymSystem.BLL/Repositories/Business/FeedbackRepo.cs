using AutoMapper;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.BLL.Interfaces;
using GymSystem.DAL.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Repositories.Business
{
    public class FeedbackRepo : IFeedbackRepo
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FeedbackRepo> _logger;

        public FeedbackRepo(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FeedbackRepo> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse> CreateFeedbackAsync(FeedbackDto feedbackDto)
        {
            try
            {
                _logger.LogInformation("Creating new feedback with data: {@FeedbackDto}", feedbackDto);

                var feedback = _mapper.Map<Feedback>(feedbackDto);

                var feedbackRepo = _unitOfWork.Repository<Feedback>();
                await feedbackRepo.Add(feedback);

                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    _logger.LogError("Failed to save feedback for data: {@FeedbackDto}", feedbackDto);
                    return new ApiResponse(500, "Failed to save feedback");
                }

                _logger.LogInformation("Feedback created successfully with ID: {FeedbackId}", feedback.Id);
                return new ApiResponse(200, "Feedback added successfully", _mapper.Map<FeedbackDto>(feedback));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback for data: {@FeedbackDto}", feedbackDto);
                return new ApiResponse(400, $"Error adding feedback: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteFeedbackAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting feedback with ID: {FeedbackId}", id);

                var feedbackRepo = _unitOfWork.Repository<Feedback>();
                var feedback = await feedbackRepo.GetByIdAsync(id);

                if (feedback == null)
                {
                    _logger.LogWarning("Feedback with ID {FeedbackId} not found.", id);
                    return new ApiResponse(404, "Feedback not found");
                }

                feedbackRepo.Delete(feedback);

                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    _logger.LogError("Failed to delete feedback with ID {FeedbackId}", id);
                    return new ApiResponse(500, "Failed to delete feedback");
                }

                _logger.LogInformation("Feedback deleted successfully with ID: {FeedbackId}", id);
                return new ApiResponse(200, "Feedback deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback with ID: {FeedbackId}", id);
                return new ApiResponse(400, $"Error deleting feedback: {ex.Message}");
            }
        }

        public async Task<IReadOnlyList<FeedbackDto>> GetAllFeedbacksAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all feedbacks");

                var feedbackRepo = _unitOfWork.Repository<Feedback>();
                var feedbacks = await feedbackRepo.GetAllAsync();

                var feedbackDtos = _mapper.Map<IReadOnlyList<FeedbackDto>>(feedbacks);
                _logger.LogInformation("Retrieved {Count} feedbacks successfully", feedbackDtos.Count);

                return feedbackDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all feedbacks");
                throw new Exception($"Error retrieving feedbacks: {ex.Message}");
            }
        }

        public async Task<FeedbackDto> GetFeedbackByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving feedback with ID: {FeedbackId}", id);

                var feedbackRepo = _unitOfWork.Repository<Feedback>();
                var feedback = await feedbackRepo.GetByIdAsync(id);

                if (feedback == null)
                {
                    _logger.LogWarning("Feedback with ID {FeedbackId} not found.", id);
                    return null;
                }

                var feedbackDto = _mapper.Map<FeedbackDto>(feedback);
                _logger.LogInformation("Feedback retrieved successfully with ID: {FeedbackId}", id);

                return feedbackDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback with ID: {FeedbackId}", id);
                throw new Exception($"Error retrieving feedback: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateFeedbackAsync(int id, FeedbackDto feedbackDto)
        {
            try
            {
                _logger.LogInformation("Updating feedback with ID: {FeedbackId}", id);

                var feedbackRepo = _unitOfWork.Repository<Feedback>();
                var existingFeedback = await feedbackRepo.GetByIdAsync(id);

                if (existingFeedback == null)
                {
                    _logger.LogWarning("Feedback with ID {FeedbackId} not found.", id);
                    return new ApiResponse(404, "Feedback not found");
                }

                existingFeedback.Comments = feedbackDto.Comments;
                existingFeedback.Rating = feedbackDto.Rating;

                feedbackRepo.Update(existingFeedback);

                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    _logger.LogError("Failed to update feedback with ID {FeedbackId}", id);
                    return new ApiResponse(500, "Failed to update feedback");
                }

                _logger.LogInformation("Feedback updated successfully with ID: {FeedbackId}", id);
                return new ApiResponse(200, "Feedback updated successfully", _mapper.Map<FeedbackDto>(existingFeedback));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback with ID: {FeedbackId}", id);
                return new ApiResponse(400, $"Error updating feedback: {ex.Message}");
            }
        }
    }
}
