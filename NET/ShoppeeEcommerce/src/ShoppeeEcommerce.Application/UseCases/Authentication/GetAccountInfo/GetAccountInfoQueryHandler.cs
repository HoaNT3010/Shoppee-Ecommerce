using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.GetAccountInfo;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.GetAccountInfo
{
    internal class GetAccountInfoQueryHandler(
        UserManager<User> userManager,
        ILogger<GetAccountInfoQueryHandler> logger)
        : IRequestHandler<GetAccountInfoQuery, ErrorOr<GetAccountInfoResponse>>
    {
        public async Task<ErrorOr<GetAccountInfoResponse>> Handle(
            GetAccountInfoQuery request,
            CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            try
            {
                if (user is null) return Errors.User.NotFoundWithId(request.UserId.ToString());
                var roles = await userManager.GetRolesAsync(user);
                return new GetAccountInfoResponse(user.Id,
                    user.UserName!,
                    user.Email!,
                    user.FirstName,
                    user.LastName,
                    roles.ToArray());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred when trying to get info of user with ID '{userId}'.", request.UserId.ToString());
                return Errors.User.GetUserInfoFailed(request.UserId.ToString());
            }
        }
    }
}
