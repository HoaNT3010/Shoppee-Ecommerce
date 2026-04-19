using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ShoppeeEcommerce.Application.Abstractions.Authentication;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;

namespace ShoppeeEcommerce.Application.UseCases.Authentication.Login
{
    internal class LoginCommandHandler(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IJwtTokenProvider jwtProvider)
        : IRequestHandler<LoginCommand, ErrorOr<LoginResponse>>
    {

        public async Task<ErrorOr<LoginResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) return Errors.Authentication.InvalidCredentials();

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
            if (result.IsLockedOut) return Errors.User.LockedOut();
            if (!result.Succeeded) return Errors.Authentication.InvalidCredentials();

            var roles = await userManager.GetRolesAsync(user);
            var refreshToken = jwtProvider.GenerateRefreshToken();
            await userManager.SetAuthenticationTokenAsync(user, "ShoppeeEcommerce", "refreshToken", refreshToken);

            return new LoginResponse(
                jwtProvider.GenerateAccessToken(user, roles),
                refreshToken);
        }
    }
}
