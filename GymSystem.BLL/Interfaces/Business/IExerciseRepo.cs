﻿using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface IExerciseRepo
    {
        Task<ApiResponse> AddExercise(ExerciseDto exercise);
        Task<ApiResponse> UpdateExercise(int id, ExerciseDto exercise);
        Task<ApiResponse> DeleteExercise(int exerciseId);
        Task<IEnumerable<ExerciseDto>> GetExerciseList();
        Task<ExerciseDto> GetExerciseById(int exerciseId);
    }
}
