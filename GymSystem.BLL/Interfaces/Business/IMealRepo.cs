using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface IMealRepo
    {
        Task<IEnumerable<MealDto>> GetAllMeals();
        Task<MealDto> GetMealById(int id);
        Task<ApiResponse> CreateMeal(MealDto meal);
        Task<ApiResponse> UpdateMeal(int id, MealDto meal);
        Task<ApiResponse> DeleteMeal(int id);
    }
}
