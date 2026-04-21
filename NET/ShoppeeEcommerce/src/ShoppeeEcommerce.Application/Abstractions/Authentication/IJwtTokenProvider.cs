using ErrorOr;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Application.Abstractions.Authentication
{
    public interface IJwtTokenProvider
    {
        string GenerateAccessToken(User user, IList<string> roles);
        string GenerateRefreshToken(User user);
        ErrorOr<string> GetUserIdFromRefreshToken(string refreshToken);
    }
}
