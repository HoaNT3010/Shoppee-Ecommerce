using ShoppeeEcommerce.Application.Abstractions.Services;
using System.Security.Claims;

namespace ShoppeeEcommerce.WebAPI.Utilities
{
    public class CartOwnerProvider(
        IHttpContextAccessor contextAccessor) : ICartOwnerProvider
    {
        const string CookieName = "cart_session";

        public Task<(Guid? userId, string? sessionId)> GetOwnerAsync(CancellationToken cancellationToken = default)
        {
            var httpContext = contextAccessor.HttpContext!;
            var user = httpContext.User;

            // If authenticated -> use UserId
            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var userId = Guid.Parse(userIdString!);

                return Task.FromResult<(Guid?, string?)>((userId, null));
            }

            // Guest -> use/create session cookie
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (!request.Cookies.TryGetValue(CookieName, out var sessionId))
            {
                sessionId = Guid.NewGuid().ToString();

                response.Cookies.Append(CookieName, sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/api/cart",
                    Expires = DateTime.UtcNow.AddYears(1)
                });
            }

            return Task.FromResult<(Guid?, string?)>((null, sessionId));
        }
    }
}
