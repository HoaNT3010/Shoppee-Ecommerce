using ErrorOr;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShoppeeEcommerce.Application.Abstractions.Authentication;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoppeeEcommerce.Infrastructure.Authentication
{
    internal class JwtTokenProvider(IOptions<JwtOptions> options)
        : IJwtTokenProvider
    {
        const string JwtOptionsErrorMsg = "JWT options cannot be null.";
        const string JwtFirstNameClaimType = "firstName";
        const string JwtLastNameClaimType = "lastName";
        const string JwtRolesClaimType = "roles";
        readonly JwtOptions _jwtOptions = options.Value ?? throw new ArgumentNullException(nameof(options), JwtOptionsErrorMsg);

        public string GenerateAccessToken(User user, IList<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(roles);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtFirstNameClaimType, user.FirstName ?? string.Empty),
                new Claim(JwtLastNameClaimType, user.LastName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtRolesClaimType, role));
            }
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMilliseconds(_jwtOptions.AccessTokenExpiryInMilliseconds),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMilliseconds(_jwtOptions.RefreshTokenExpiryInMilliseconds),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ErrorOr<string> GetUserIdFromRefreshToken(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
            };
            try
            {
                var principal = tokenHandler.ValidateToken(
                    refreshToken,
                    validationParameters,
                    out SecurityToken validatedToken);

                // Extra safety: ensure it's actually JWT with expected alg
                if (validatedToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
                {
                    return Errors.Authentication.InvalidRefreshToken();
                }

                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(userId)) return Errors.Authentication.RefreshTokenMissingClaims();

                return userId;
            }
            catch
            {
                return Errors.Authentication.InvalidRefreshToken();
            }
        }
    }
}
