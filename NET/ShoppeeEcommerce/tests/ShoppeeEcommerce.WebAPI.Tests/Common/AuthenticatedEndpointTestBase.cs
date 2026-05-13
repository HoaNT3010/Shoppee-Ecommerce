using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppeeEcommerce.WebAPI.Tests.Common
{
    public class AuthenticatedEndpointTestBase
        : EndpointTestBase
    {
        protected ClaimsPrincipal _claimsPrincipal;
        protected AuthenticatedEndpointTestBase()
        {
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _claimsPrincipal = new ClaimsPrincipal(identity);
        }
    }
}
