using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.Login
{
    public record LoginCommand(
        string Email,
        string Password)
        : IRequest<ErrorOr<LoginResponse>>;
}
