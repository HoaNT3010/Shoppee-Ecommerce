using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.CustomerRegister
{
    public record CustomerRegisterCommand(
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? FirstName,
        string? LastName)
        : IRequest<ErrorOr<Created>>;
}
