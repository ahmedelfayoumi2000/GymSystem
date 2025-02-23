﻿using AutoMapper;
using GymSystem.API.DTOs.Trainer;
using GymSystem.BLL.Dtos.Trainer;
using GymSystem.BLL.Interfaces.Business;
using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public TrainerService(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<TrainerDto> GetTrainerByIdAsync(string id)
        {
            var trainer = await _userManager.FindByIdAsync(id);
            if (trainer == null || trainer.UserRole != 2) // UserRole 2 for Trainer
                return null;

            return _mapper.Map<TrainerDto>(trainer);
        }

        public async Task<IEnumerable<TrainerDto>> GetAllTrainersAsync()
        {
            var trainers = _userManager.Users.Where(u => u.UserRole == 2).ToList();
            return _mapper.Map<IEnumerable<TrainerDto>>(trainers);
        }

        public async Task<TrainerDto> CreateTrainerAsync(CreateTrainerDto trainerDto)
        {
            var user = _mapper.Map<AppUser>(trainerDto);

            var result = await _userManager.CreateAsync(user, trainerDto.Password);

            if (!result.Succeeded)
                throw new Exception("Failed to create trainer.");

            await _userManager.AddToRoleAsync(user, "Trainer");

            return _mapper.Map<TrainerDto>(user);
        }

        public async Task<bool> UpdateTrainerAsync(string id, UpdateTrainerDto trainerDto)
        {
            var trainer = await _userManager.FindByIdAsync(id);
            if (trainer == null || trainer.UserRole != 2)
                return false;

            _mapper.Map(trainerDto, trainer); // Update properties using AutoMapper

            var result = await _userManager.UpdateAsync(trainer);
            return result.Succeeded;
        }

        public async Task<bool> DeleteTrainerAsync(string id)
        {
            var trainer = await _userManager.FindByIdAsync(id);
            if (trainer == null || trainer.UserRole != 2)
                return false;

            var result = await _userManager.DeleteAsync(trainer);
            return result.Succeeded;
        }

        public async Task<bool> SuspendTrainerAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new System.Exception("Trainer not found.");
            }

            user.IsStopped = true;
            user.StopDate = System.DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}