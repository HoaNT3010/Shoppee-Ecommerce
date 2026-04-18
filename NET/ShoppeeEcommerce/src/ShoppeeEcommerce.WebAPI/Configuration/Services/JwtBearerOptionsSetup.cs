using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShoppeeEcommerce.Infrastructure.Authentication;
using System.Text;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public class JwtBearerOptionsSetup(IOptions<JwtOptions> options) : IConfigureNamedOptions<JwtBearerOptions>
    {
        const string JwtOptionsErrorMsg = "JWT options cannot be null.";
        readonly JwtOptions _jwtOptions = options.Value ?? throw new ArgumentNullException(nameof(options), JwtOptionsErrorMsg);

        public void Configure(string? name, JwtBearerOptions options)
        {
            // Disable the default claim mapping to prevent issues
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "roles",
            };
        }

        public void Configure(JwtBearerOptions options)
        {
            // Disable the default claim mapping to prevent issues
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "roles",
            };
        }
    }
}
