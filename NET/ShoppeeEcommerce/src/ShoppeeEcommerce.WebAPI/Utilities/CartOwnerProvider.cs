using ShoppeeEcommerce.Application.Abstractions.Services;

namespace ShoppeeEcommerce.WebAPI.Utilities
{
    public class CartOwnerProvider(
        IHttpContextAccessor contextAccessor) : ICartOwnerProvider
    {
        const string SessionCartHeader = "X-Cart-Session";
        const string CustomerCartHeader = "X-Cart-Customer";

        public Task<(Guid? userId, string? sessionId)> GetOwnerAsync(CancellationToken cancellationToken = default)
        {
            var sessionHeader = contextAccessor.HttpContext!.Request.Headers[SessionCartHeader].ToString();
            var customerHeader = contextAccessor.HttpContext!.Request.Headers[CustomerCartHeader].ToString();
            Guid? userId = !string.IsNullOrWhiteSpace(customerHeader) ? Guid.Parse(customerHeader) : null;
            string? sessionId = !string.IsNullOrWhiteSpace(sessionHeader) ? sessionHeader : null;
            return Task.FromResult<(Guid?, string?)>((userId, sessionId));
        }
    }
}
