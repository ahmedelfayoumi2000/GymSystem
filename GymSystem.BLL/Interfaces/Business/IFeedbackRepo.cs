using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface IFeedbackRepo
    {
        Task<ApiResponse> CreateFeedbackAsync(FeedbackDto feedbackDto);
        Task<ApiResponse> DeleteFeedbackAsync(int id);
        Task<IReadOnlyList<FeedbackDto>> GetAllFeedbacksAsync();
        Task<FeedbackDto> GetFeedbackByIdAsync(int id);
        Task<ApiResponse> UpdateFeedbackAsync(int id, FeedbackDto feedbackDto);
    }
}
