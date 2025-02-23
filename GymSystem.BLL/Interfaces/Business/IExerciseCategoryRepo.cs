using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface IExerciseCategoryRepo
    {
        public Task<ApiResponse> AddExerciseCategory(ExerciseCategoryDto exerciseCategoryDto);
        public Task<ApiResponse> DeleteExerciseCategory(int id);
        public Task<ExerciseCategoryDto> GetExerciseCategory(int id);
        public Task<IEnumerable<ExerciseCategoryDto>> GetExerciseCategories();
        public Task<ApiResponse> UpdateExerciseCategory(int id, ExerciseCategoryDto exerciseCategoryDto);

    }
}
