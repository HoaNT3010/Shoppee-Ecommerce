using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Login
{
    public class LoginRequestValidator
        : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("User's email must be a valid email address.");
            RuleFor(x => x.Password)
                .StrongPassword();
        }
    }

    public static class PasswordRules
    {
        public static IRuleBuilderOptions<T, string> StrongPassword<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .MinimumLength(6)

                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")

                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")

                .Matches(@"\d")
                .WithMessage("Password must contain at least one digit.")

                .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one non-alphanumeric character.")

                .Must(HaveUniqueChars)
                .WithMessage("Password must contain at least one unique character.");
        }

        private static bool HaveUniqueChars(string password)
            => password.Distinct().Count() >= 1;
    }
}
