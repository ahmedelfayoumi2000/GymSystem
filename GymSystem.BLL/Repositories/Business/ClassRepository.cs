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
using GymSystem.BLL.Specifications;

namespace GymSystem.BLL.Repositories.Business
{

    public class ClassRepository : IClassRepo
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ClassRepository> _logger;

       
        public ClassRepository(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ClassRepository> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

       
        public async Task<ApiResponse> AddClass(ClassDto classDto)
        {
            if (classDto == null)
            {
                _logger.LogWarning("Attempted to add a null ClassDto.");
                return new ApiResponse(400, "Class data cannot be null.");
            }

            try
            {
                _logger.LogInformation("Attempting to add class with name: {ClassName}", classDto.ClassName);

                var classSpec = new BaseSpecification<Class>(c => c.ClassName == classDto.ClassName && !c.IsDeleted);
                var existingClass = await _unitOfWork.Repository<Class>().GetByIdWithSpecAsync(classSpec);
                if (existingClass != null)
                {
                    _logger.LogWarning("Class with name {ClassName} already exists.", classDto.ClassName);
                    return new ApiResponse(409, $"Class '{classDto.ClassName}' already exists.");
                }

                var classEntity = _mapper.Map<Class>(classDto);
                await _unitOfWork.Repository<Class>().Add(classEntity);
                await _unitOfWork.Complete();

                _logger.LogInformation("Class {ClassName} added successfully with ID: {Id}", classDto.ClassName, classEntity.Id);
                return new ApiResponse(201, "Class added successfully", _mapper.Map<ClassDto>(classEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding class with name: {ClassName}", classDto.ClassName);
                return new ApiResponse(500, $"Failed to add class: {ex.Message}");
            }
        }

       
        public async Task<ApiResponse> DeleteClass(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete class with ID: {Id}", id);

                var classEntity = await _unitOfWork.Repository<Class>().GetByIdAsync(id);
                if (classEntity == null || classEntity.IsDeleted)
                {
                    _logger.LogWarning("Class with ID {Id} not found or already deleted.", id);
                    return new ApiResponse(404, $"Class with ID {id} not found.");
                }

                classEntity.IsDeleted = true;
                _unitOfWork.Repository<Class>().Update(classEntity);
                await _unitOfWork.Complete();

                _logger.LogInformation("Class with ID {Id} deleted successfully.", id);
                return new ApiResponse(200, "Class deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class with ID: {Id}", id);
                return new ApiResponse(500, $"Failed to delete class: {ex.Message}");
            }
        }

       
        public async Task<ClassDto> GetClass(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving class with ID: {Id}", id);

                var spec = new BaseSpecification<Class>(c => c.Id == id && !c.IsDeleted);
                var classEntity = await _unitOfWork.Repository<Class>().GetByIdWithSpecAsync(spec);
                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {Id} not found.", id);
                    return null;
                }

                var classDto = _mapper.Map<ClassDto>(classEntity);
                _logger.LogInformation("Class with ID {Id} retrieved successfully.", id);
                return classDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class with ID: {Id}", id);
                throw new ApplicationException($"Failed to retrieve class: {ex.Message}", ex);
            }
        }

      
        public async Task<IEnumerable<ClassDto>> GetClasses()
        {
            try
            {
                _logger.LogInformation("Retrieving all active classes.");

                var spec = new BaseSpecification<Class>(c => !c.IsDeleted);
                var classes = await _unitOfWork.Repository<Class>().GetAllWithSpecAsync(spec);
                var classDtos = _mapper.Map<IEnumerable<ClassDto>>(classes);

                _logger.LogInformation("Retrieved {Count} active classes.", classDtos.Count());
                return classDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all classes.");
                throw new ApplicationException($"Failed to retrieve classes: {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse> UpdateClass(int id, ClassDto classDto)
        {
            if (classDto == null)
            {
                _logger.LogWarning("Attempted to update class with ID {Id} using null ClassDto.", id);
                return new ApiResponse(400, "Class data cannot be null.");
            }

            try
            {
                _logger.LogInformation("Attempting to update class with ID: {Id}", id);

                var existingClass = await _unitOfWork.Repository<Class>().GetByIdAsync(id);
                if (existingClass == null || existingClass.IsDeleted)
                {
                    _logger.LogWarning("Class with ID {Id} not found or already deleted.", id);
                    return new ApiResponse(404, $"Class with ID {id} not found.");
                }

                _mapper.Map(classDto, existingClass); 
                _unitOfWork.Repository<Class>().Update(existingClass);
                await _unitOfWork.Complete();

                _logger.LogInformation("Class with ID {Id} updated successfully.", id);
                return new ApiResponse(200, "Class updated successfully", _mapper.Map<ClassDto>(existingClass));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class with ID: {Id}", id);
                return new ApiResponse(500, $"Failed to update class: {ex.Message}");
            }
        }
    }

}
