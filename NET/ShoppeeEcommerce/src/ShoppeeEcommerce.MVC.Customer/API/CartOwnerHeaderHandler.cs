using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public class CartOwnerHeaderHandler(
        IHttpContextAccessor contextAccessor)
        : DelegatingHandler
    {
        const string CookieName = "cart_session";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var context = contextAccessor.HttpContext!;
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
                request.Headers.Add("X-Cart-Customer", userId);
            }
            if (context.Request.Cookies.TryGetValue(CookieName, out var sessionId))
            {
                request.Headers.Add("X-Cart-Session", sessionId);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
