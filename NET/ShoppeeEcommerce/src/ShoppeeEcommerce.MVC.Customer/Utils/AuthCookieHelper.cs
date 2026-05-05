using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ShoppeeEcommerce.MVC.Customer.Utils
{
    public static class AuthCookieHelper
    {
        public const string AccessTokenClaimName = "access_token";
        public const string RefreshTokenClaimName = "refresh_token";
        public const string ExpiresAtClaimName = "expires_at";

        public static async Task SetAuthCookie(
            HttpContext context,
            string accessToken,
            string refreshToken,
            bool rememberMe)
        {
            var claims = JwtTokenHelper.GetUserClaims(accessToken).ToList();
            var expiresAt = JwtTokenHelper.GetExpiryUtc(accessToken);

            claims.Add(new Claim(AccessTokenClaimName, accessToken));
            claims.Add(new Claim(RefreshTokenClaimName, refreshToken));
            claims.Add(new Claim(ExpiresAtClaimName, expiresAt.ToString("O")));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                });
        }
    }
}
