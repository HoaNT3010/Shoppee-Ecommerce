using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Refresh;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.Refresh
{
    public record RefreshCommand(
        string RefreshToken)
        : IRequest<ErrorOr<RefreshResponse>>;
}
