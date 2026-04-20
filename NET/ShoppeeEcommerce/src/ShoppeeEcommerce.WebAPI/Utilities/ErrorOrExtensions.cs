using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.WebAPI.Utilities
{
    public static class ErrorOrExtensions
    {
        public static ActionResult<T> ToActionResult<T>(
            this ErrorOr<T> result)
        {
            if (result.IsError)
            {
                return result.FirstError.Type switch
                {
                    ErrorType.Conflict => new ConflictObjectResult(result.Errors),
                    ErrorType.Validation => new BadRequestObjectResult(result.Errors),
                    ErrorType.NotFound => new NotFoundObjectResult(result.Errors),
                    ErrorType.Unauthorized => new UnauthorizedObjectResult(result.Errors),
                    _ => new ObjectResult(result.Errors)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    }
                };
            }

            return result.Value switch
            {
                Created => new StatusCodeResult(StatusCodes.Status201Created),
                Deleted => new NoContentResult(),
                Updated => new NoContentResult(),
                Success => new OkResult(),
                _ => new OkObjectResult(result.Value)
            };
        }
    }
}
