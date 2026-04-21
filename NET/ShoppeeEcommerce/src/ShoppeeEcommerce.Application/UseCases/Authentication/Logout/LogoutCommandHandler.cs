using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ShoppeeEcommerce.Application.Abstractions.Authentication;
using ShoppeeEcommerce.Domain.Constants;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.Logout
{
    internal class LogoutCommandHandler(
        UserManager<User> userManager,
        IJwtTokenProvider tokenProvider)
        : IRequestHandler<LogoutCommand, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            var userId = tokenProvider.GetUserIdFromRefreshToken(request.RefreshToken);
            if (userId.IsError) return userId.FirstError;

            var user = await userManager.FindByIdAsync(userId.Value);
            if (user is null) return Errors.User.NotFoundWithId(userId.Value);

            var storedToken = await userManager.GetAuthenticationTokenAsync(user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName);
            // If no token found, return immediately
            if (storedToken is null) return Result.Deleted;
            // If token does not match the stored token -> Invalid
            if (storedToken != request.RefreshToken) return Errors.Authentication.InvalidRefreshToken();

            var result = await userManager.RemoveAuthenticationTokenAsync(
                user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName);
            if (!result.Succeeded) return Errors.Authentication.RevokeRefreshTokenFailed();
            return Result.Deleted;
        }
    }
}
