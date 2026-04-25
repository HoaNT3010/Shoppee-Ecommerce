using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.GetAccountInfo;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.GetAccountInfo
{
    public record GetAccountInfoQuery(
        Guid UserId)
        : IRequest<ErrorOr<GetAccountInfoResponse>>;
}
