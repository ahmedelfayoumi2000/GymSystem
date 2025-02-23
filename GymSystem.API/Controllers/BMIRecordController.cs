using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using GymSystem.BLL.Interfaces.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.API.Controllers
{
  
    public class BMIRecordController : BaseApiController
    {
        private readonly IBMIRecordRepo _bMIRecordRepo;

        public BMIRecordController(IBMIRecordRepo bMIRecordRepo)
        {
            _bMIRecordRepo = bMIRecordRepo;
        }
        [Authorize]
        [HttpGet("getBMIRecordsForUser")]
        public async Task<IActionResult> GetBMIRecordsForUser()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var UserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (UserIdClaim == null)
                {
                    return BadRequest("UserIdClaim claim not found in the token");
                }
                var result = await _bMIRecordRepo.GetBMIRecordsForUser(UserIdClaim.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(404, ex.Message));
            }
        }
        [Authorize]
        [HttpPost("addBMIRecord")]
        public async Task<IActionResult> AddBMIRecord(BMIRecordDto bmiRecord)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var UserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (UserIdClaim == null)
                {
                    return BadRequest("UserIdClaim claim not found in the token");
                }
                bmiRecord.UserId = UserIdClaim.Value;

                var result = await _bMIRecordRepo.AddBMIRecord(bmiRecord);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(404, ex.Message));
            }
        }
        [Authorize]
        [HttpDelete("deleteBMIRecord")]
        public async Task<IActionResult> DeleteBMIRecord(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _bMIRecordRepo.DeleteBMIRecord(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(404, ex.Message));
            }
        }
    }
}
