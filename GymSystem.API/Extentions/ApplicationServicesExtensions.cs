using GymSystem.BLL.Errors;
using GymSystem.API.Helpers;
using GymSystem.BLL.Interfaces;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.BLL.Services;
using GymSystem.DAL.Data;
using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GymSystem.BLL.Interfaces.Auth;
using GymSystem.BLL.Services.Auth;
using GymSystem.BLL.Repositories.Business;
using GymSystem.BLL.Repositories;
using MailKit;

namespace GymSystem.API.Extentions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
           
           
            services.AddDataProtection();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(p => p.Value.Errors.Count > 0)
                        .SelectMany(p => p.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });


            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IResponceCacheService, ResponceCacheService>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<IAttendaceRepo, AttendaceRepo>();
            services.AddScoped<IFeedbackRepo, FeedbackRepo>();
            services.AddScoped<IBMIRecordRepo, BMIRecordRepo>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INutritionPlanRepo, NutritionPlanRepo>();
            //services.AddScoped<IExerciseCategoryRepo, ExerciseCategoryRepo>();
            //services.AddScoped<IClassRepo, ClassRepo>();
            //services.AddScoped<IWorkoutPlanRepo, WorkoutPlanRepo>();
            //services.AddScoped<IExerciseRepo, ExerciseRepo>();
            //services.AddScoped<IMealsCategoryRepo, MealsCategoryRepo>();
            //services.AddScoped<IMealRepo, MealRepo>();
            //services.AddScoped<IMembershipRepo, MembershipRepo>();
            //services.AddScoped<INotificationRepo, NotificationRepo>();




            return services;
        }

    }
}
