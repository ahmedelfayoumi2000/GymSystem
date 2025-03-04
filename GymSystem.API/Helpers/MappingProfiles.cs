using AutoMapper;
using GymSystem.API.DTOs.Trainer;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Dtos.MonthlyMembership;
using GymSystem.BLL.Dtos.NutritionPlan;
using GymSystem.BLL.Dtos.Product;
using GymSystem.BLL.Dtos.Trainer;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Entities.Identity;

namespace GymSystem.API.Helpers
{
    public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			// Map from AppUser to TrainerDto
			CreateMap<AppUser, TrainerDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
				.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
				.ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
				.ForMember(dest => dest.IsStopped, opt => opt.MapFrom(src => src.IsStopped))
				.ForMember(dest => dest.HaveDays, opt => opt.MapFrom(src => src.HaveDays))
				.ForMember(dest => dest.AddBy, opt => opt.MapFrom(src => src.AddBy))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
		   	    .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary));


			// Map from CreateTrainerDto to AppUser
			CreateMap<CreateTrainerDto, AppUser>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
				.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
				 .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
				.ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => 2)) // Trainer Role
			    .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary));

			// Map from UpdateTrainerDto to AppUser
			CreateMap<UpdateTrainerDto, AppUser>()
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
				.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
				 .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
			   .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
			   .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary));
			//====================================================================================


			CreateMap<Attendance, DailyAttendanceDto>().ReverseMap();
			CreateMap<Class, ClassDto>().ReverseMap();
			CreateMap<ExerciseCategory, ExerciseCategoryDto>().ReverseMap();
			CreateMap<WorkoutPlan, WorkoutPlanDto>().ReverseMap();
			CreateMap<Exercise, ExerciseDto>().ReverseMap();
			CreateMap<Feedback, FeedbackDto>().ReverseMap();
			CreateMap<BMIRecord, BMIRecordDto>().ReverseMap();
			CreateMap<MealsCategory, MealsCategoryDto>().ReverseMap();
			CreateMap<Meal, MealDto>().ReverseMap();
			CreateMap<NutritionPlan, NutritionPlanDto>().ReverseMap();
			CreateMap<Membership, MembershipDto>().ReverseMap();
			CreateMap<Notification, NotificationDto>().ReverseMap();
			CreateMap<AppUser, UserDto>().ReverseMap();
			CreateMap<EquipmentCreateDto, Equipment>();
			CreateMap<Equipment, EquipmentViewDto>();
			CreateMap<Repair, RepairDto>().ReverseMap();

			// ✅ عند عرض الاشتراك الشهري، تضمين جميع بيانات الـ Plan
			CreateMap<MonthlyMembershipp, MonthlyMembershipViewDto>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
			.ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.UserEmail))
				   .ForMember(dest => dest.phoneNumber, opt => opt.MapFrom(src => src.phoneNumber))
			.ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.Plan)); // جلب جميع بيانات الخطة
     

			// ✅ عند إدخال اشتراك جديد، البحث عن الـ Plan باستخدام PlanId
			CreateMap<MonthlyMembershipCreateDto, MonthlyMembershipp>()
		    .ForMember(dest => dest.Plan, opt => opt.Ignore()); // سيتم جلب الـ Plan من قاعدة البيانات

			CreateMap<PlanDto, Plan>().ReverseMap();

			// من `ProductCreateDto` إلى `Product`
			CreateMap<ProductCreateDto, Product>()
				.ForMember(dest => dest.IsDeleted, opt => opt.Ignore()); // لا نريد تغيير `IsDeleted` عند الإنشاء

			// من `Product` إلى `ProductViewDto`
			CreateMap<Product, ProductViewDto>()
				.ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsActive)); // تعديل اسم الحقل
		}
	}
}
