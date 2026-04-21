using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ShoppeeEcommerce.Application.Abstractions.Authentication;
using ShoppeeEcommerce.Domain.Constants;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Refresh;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.Refresh
{
    internal class RefreshCommandHandler(
        UserManager<User> userManager,
        IJwtTokenProvider tokenProvider)
        : IRequestHandler<RefreshCommand, ErrorOr<RefreshResponse>>
    {
        public async Task<ErrorOr<RefreshResponse>> Handle(
            RefreshCommand request,
            CancellationToken cancellationToken)
        {
            // Has token validation but skip token lifetime validation
            var userId = tokenProvider.GetUserIdFromRefreshToken(request.RefreshToken);
            if (userId.IsError) return userId.FirstError;

            var user = await userManager.FindByIdAsync(userId.Value);
            if (user is null) return Errors.User.NotFoundWithId(userId.Value);

            var storedToken = await userManager.GetAuthenticationTokenAsync(user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName);
            if (storedToken is null) return Errors.Authentication.Unauthorized();

            if (storedToken != request.RefreshToken) return Errors.Authentication.InvalidRefreshToken();
            // Need to revalidate token to confirm token's lifetime
            var isValid = tokenProvider.IsTokenValid(request.RefreshToken);
            if (!isValid) return Errors.Authentication.InvalidRefreshToken();

            var roles = await userManager.GetRolesAsync(user);
            var refreshToken = tokenProvider.GenerateRefreshToken(user);
            var result = await userManager.SetAuthenticationTokenAsync(
                user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName,
                refreshToken);
            if (!result.Succeeded) return Errors.Authentication.GenerateRefreshTokenFailed();

            return new RefreshResponse(
                tokenProvider.GenerateAccessToken(user, roles),
                refreshToken);
        }
    }
}
