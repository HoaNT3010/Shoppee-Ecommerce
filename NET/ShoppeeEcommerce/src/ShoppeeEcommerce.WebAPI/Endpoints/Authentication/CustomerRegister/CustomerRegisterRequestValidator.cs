using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.CustomerRegister;
using ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Login;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.CustomerRegister
{
    public class CustomerRegisterRequestValidator
        : AbstractValidator<CustomerRegisterRequest>
    {
        public CustomerRegisterRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("User's username is required.")
                .MaximumLength(256)
                .WithMessage("User's username cannot exceed 256 characters.");
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .WithMessage("User's email is required and must be a valid email address.")
                .MaximumLength(256)
                .WithMessage("User's email cannot exceed 256 characters.");
            RuleFor(x => x.Password)
                .StrongPassword();
            RuleFor(x => x.ConfirmPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Confirm password must match password.");
            RuleFor(x => x.FirstName)
                .MaximumLength(100)
                .WithMessage("User's first name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));
            RuleFor(x => x.LastName)
                .MaximumLength(100)
                .WithMessage("User's last name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));
        }
    }
}
