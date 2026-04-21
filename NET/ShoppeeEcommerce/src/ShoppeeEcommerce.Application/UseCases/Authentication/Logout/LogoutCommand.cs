using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.Logout
{
    public record LogoutCommand(
        string RefreshToken)
        : IRequest<ErrorOr<Deleted>>;
}
