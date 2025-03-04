using GymSystem.BLL.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
		#region Private Helper Methods

		private bool IsValidId(int id) => id > 0;

		private bool IsValidModel(object model) => ModelState.IsValid && model != null;

		private ApiValidationErrorResponse CreateValidationError(string message)
		{
			return new ApiValidationErrorResponse
			{
				Errors = new List<string> { message },
				StatusCode = 400,
				Message = "Invalid request data"
			};
		}

		private IActionResult HandleApiResponse(ApiResponse response, int successStatusCode = StatusCodes.Status200OK)
		{
			return response.StatusCode switch
			{
				200 => Ok(response),
				201 => StatusCode(StatusCodes.Status201Created, response),
				404 => NotFound(response),
				400 => BadRequest(response),
				500 => StatusCode(StatusCodes.Status500InternalServerError, response),
				_ => StatusCode(response.StatusCode ?? 500, response)
			};
		}

		#endregion
	}
}
