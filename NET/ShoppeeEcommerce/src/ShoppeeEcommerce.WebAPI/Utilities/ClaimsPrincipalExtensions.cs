using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppeeEcommerce.WebAPI.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (value is null)
                throw new UnauthorizedAccessException("User id claim not found.");

            return Guid.Parse(value);
        }
    }
}
