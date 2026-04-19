using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShoppeeEcommerce.WebAPI.Utilities
{
    public class GlobalValidationFilter(
        IServiceProvider serviceProvider)
        : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Get the request object (usually the first argument in ApiEndpoints)
            var request = context.ActionArguments.Values.FirstOrDefault();

            if (request != null)
            {
                // Try to find a validator for this specific type
                var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());
                var validator = serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    // 3. Validate
                    var validationContext = new ValidationContext<object>(request);
                    var result = await validator.ValidateAsync(validationContext);

                    if (!result.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(result.ToDictionary());
                        return;
                    }
                }
            }

            await next();
        }
    }
}
