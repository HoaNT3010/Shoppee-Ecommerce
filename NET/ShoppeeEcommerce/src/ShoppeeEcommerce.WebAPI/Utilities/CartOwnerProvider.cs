using ShoppeeEcommerce.Application.Abstractions.Services;

namespace ShoppeeEcommerce.WebAPI.Utilities
{
    public class CartOwnerProvider(
        IHttpContextAccessor contextAccessor) : ICartOwnerProvider
    {
        const string CartOwnerHeader = "X-Cart-Owner";

        public Task<(Guid? userId, string? sessionId)> GetOwnerAsync(CancellationToken cancellationToken = default)
        {
            var httpContext = contextAccessor.HttpContext!;
            var user = httpContext.User;

            var header = contextAccessor.HttpContext!.Request.Headers[CartOwnerHeader].ToString();
            if (header is not null)
            {
                return header.StartsWith("user:")
                    // users:
                    ? Task.FromResult<(Guid?, string?)>((Guid.Parse(header[5..]), null))
                    // session:
                    : Task.FromResult<(Guid?, string?)>((null, header[8..]));
            }
            return Task.FromResult<(Guid?, string?)>((null, null));
        }
    }
}
