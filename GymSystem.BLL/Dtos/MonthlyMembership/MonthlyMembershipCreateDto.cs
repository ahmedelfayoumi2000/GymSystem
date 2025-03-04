﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GymSystem.BLL.Dtos.MonthlyMembership
{
    public class MonthlyMembershipCreateDto
    {   

        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "User name must be between 2 and 100 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "phoneNumber is required.")]
        public string phoneNumber { get; set; }


		[Required(ErrorMessage = "PlanId is required.")]
		public int PlanId { get; set; }


        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }



		public bool IsActive { get; set; }
    }
}