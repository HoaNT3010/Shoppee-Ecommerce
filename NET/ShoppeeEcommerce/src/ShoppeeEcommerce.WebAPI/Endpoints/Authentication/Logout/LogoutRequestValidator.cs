using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Logout;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Logout
{
    public class LogoutRequestValidator
        : AbstractValidator<LogoutRequest>
    {
        public LogoutRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(100)
            .Must(t => t.Count(c => c == '.') == 2)
            .WithMessage("Invalid refresh token format.");
        }
    }
}
