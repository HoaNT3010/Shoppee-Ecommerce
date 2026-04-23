using FluentValidation;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.GetById
{
    public class GetCategoryByIdRequestValidator
        : AbstractValidator<GetCategoryByIdRequest>
    {
        public GetCategoryByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeGuid();
        }
    }

    public static class FluentValidationGuidExtensions
    {
        public static IRuleBuilderOptions<T, string?> MustBeGuid<T>(
            this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .Must(value => value is not null && Guid.TryParse(value, out _))
                .WithMessage("{PropertyName} must be a valid Guid/uuid.");
        }
    }
}
