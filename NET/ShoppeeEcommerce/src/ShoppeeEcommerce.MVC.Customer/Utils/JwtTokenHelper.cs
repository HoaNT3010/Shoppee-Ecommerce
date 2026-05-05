using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppeeEcommerce.MVC.Customer.Utils
{
    public static class JwtTokenHelper
    {
        public static DateTime GetExpiryUtc(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo;
        }

        public static IEnumerable<Claim> GetUserClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims;
        }
    }
}
