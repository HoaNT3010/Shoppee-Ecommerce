using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Refresh;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Refresh
{
    public class RefreshRequestValidator
        : AbstractValidator<RefreshRequest>
    {
        public RefreshRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(100)
            .Must(t => t.Count(c => c == '.') == 2)
            .WithMessage("Invalid refresh token format.");
        }
    }
}
