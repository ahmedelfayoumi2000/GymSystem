using AutoMapper;
using GymSystem.BLL.Dtos;
using GymSystem.BLL.Interfaces;
using GymSystem.BLL.Specifications.EquipmentSpec;
using GymSystem.BLL.Specifications;
using GymSystem.DAL.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;

namespace GymSystem.BLL.Repositories.Business
{
	public class RepairEquipmentRepo : IRepairEquipmentRepo
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ILogger<EquipmentRepo> _logger;

		public RepairEquipmentRepo(
			IUnitOfWork unitOfWork,
			IMapper mapper,
			ILogger<EquipmentRepo> logger)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_logger = logger;
		}


		public async Task<IReadOnlyList<RepairDto>> GetAllAsync()
		{
			try
			{
				_logger.LogInformation("Retrieving all Repairs ");


				var Repairs = await _unitOfWork.Repository<Repair>().GetAllAsync();

				var repairDto = _mapper.Map<IReadOnlyList<RepairDto>>(Repairs);
				_logger.LogInformation("Successfully retrieved {Count} Repair for all equipment in the Gym", repairDto.Count);

				return repairDto;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving equipment list");
				throw new Exception("Failed to retrieve equipment from the database.", ex);
			}
		}


		public async Task<List<RepairDto>> GetRepairsByEquipmentIdAsync(int EquipmentId)
		{
			try
			{
				_logger.LogInformation("Retrieving Repairs for Equipment with ID: {EquipmentId}", EquipmentId);

				var repairs = await _unitOfWork.Repository<Repair>()
					.GetRepairByEquipmentIdAsync(EquipmentId); // استرجاع جميع الإصلاحات لنفس الـ EquipmentId

				if (repairs == null || !repairs.Any())
				{
					_logger.LogWarning("No repairs found for equipment with ID {EquipmentId}", EquipmentId);
					return new List<RepairDto>(); // إرجاع قائمة فارغة بدلاً من `null`
				}

				var repairDtos = _mapper.Map<List<RepairDto>>(repairs);
				_logger.LogInformation("Successfully retrieved {Count} repairs for Equipment with ID: {EquipmentId}", repairDtos.Count, EquipmentId);

				return repairDtos;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving repairs for Equipment with ID: {EquipmentId}", EquipmentId);
				throw new Exception($"Failed to retrieve repairs for Equipment with ID {EquipmentId} from the database.", ex);
			}
		}


		public async Task<ApiResponse> CreateAsync(RepairDto RepairDto)
		{
			if (RepairDto == null)
			{
				_logger.LogWarning("Received null Repair data for Adding.");
				return new ApiResponse(400, "Repair data cannot be null.");
			}

			try
			{
				_logger.LogInformation("Adding new equipmentRepair with Description: {RepairDescription}", RepairDto.Description);

				var repair = _mapper.Map<Repair>(RepairDto);
				await _unitOfWork.Repository<Repair>().Add(repair);

				var result = await _unitOfWork.Complete();
				if (result <= 0)
				{
					_logger.LogError("Failed to save Repair Data for this Equipment Id: {EquipmentId}", RepairDto.EquipmentId);
					return new ApiResponse(500, "Failed to save the equipmentRepair to the database.");
				}

				var createdDto = _mapper.Map<RepairDto>(repair);
				_logger.LogInformation("Adding Repair details  successfully for EquipmentId: {EquipmentId}", createdDto.EquipmentId);

				return new ApiResponse(201, "Equipment created successfully", createdDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error Dding repairDetails for EquipmentId: {Equipment}", RepairDto.EquipmentId);
				return new ApiExceptionResponse(500, "An error occurred while Adding Repair Details", ex.Message);
			}
		}




	}
}
