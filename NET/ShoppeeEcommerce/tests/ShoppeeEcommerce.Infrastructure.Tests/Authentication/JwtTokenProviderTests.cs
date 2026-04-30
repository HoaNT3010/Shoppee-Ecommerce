using Microsoft.Extensions.Options;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.Infrastructure.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace ShoppeeEcommerce.Infrastructure.Tests.Authentication
{
    public class JwtTokenProviderTests
    {
        private readonly JwtOptions _options;
        private readonly IOptions<JwtOptions> _optionsWrapper;

        public JwtTokenProviderTests()
        {
            _options = new JwtOptions
            {
                SecretKey = "a_very_long_and_secure_secret_key_for_testing_purposes",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpiryInMilliseconds = 3600000, // 1 hour
                RefreshTokenExpiryInMilliseconds = 86400000 // 1 day
            };
            _optionsWrapper = Options.Create(_options);
        }

        private JwtTokenProvider CreateProvider(JwtOptions? overrideOptions = null)
        {
            return new JwtTokenProvider(overrideOptions != null
                ? Options.Create(overrideOptions)
                : _optionsWrapper);
        }

        [Fact]
        public void GenerateAccessToken_ShouldContainCorrectClaims()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), UserName = "johndoe", Email = "john@test.com" };
            var roles = new List<string> { "Admin", "User" };
            var provider = CreateProvider();

            // Act
            var tokenString = provider.GenerateAccessToken(user, roles);

            // Decoding to verify content
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenString);

            // Assert
            Assert.Equal(user.Id.ToString(), jwt.Subject);
            Assert.Equal(user.Email, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(2, jwt.Claims.Count(c => c.Type == "roles")); // Verify roles collection
            Assert.Equal(_options.Issuer, jwt.Issuer);
        }

        [Fact]
        public void IsTokenValid_ShouldReturnFalse_WhenTokenIsExpired()
        {
            // Arrange
            // Create a provider with a 0ms expiry
            var shortLivedOptions = new JwtOptions
            {
                SecretKey = _options.SecretKey,
                AccessTokenExpiryInMilliseconds = -1000 // Already expired
            };
            var provider = CreateProvider(shortLivedOptions);
            var user = new User { Id = Guid.NewGuid() };

            var token = provider.GenerateAccessToken(user, new List<string>());

            // Act
            var isValid = provider.IsTokenValid(token);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void GetUserIdFromRefreshToken_ShouldReturnUserId_WhenTokenIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var provider = CreateProvider();
            var token = provider.GenerateRefreshToken(user);

            // Act
            var result = provider.GetUserIdFromRefreshToken(token);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(userId.ToString(), result.Value);
        }

        [Fact]
        public void GetUserIdFromRefreshToken_ShouldReturnError_WhenSignatureIsTampered()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var provider = CreateProvider();
            var validToken = provider.GenerateRefreshToken(user);
            var tamperedToken = validToken + "tamper"; // Append noise to break signature

            // Act
            var result = provider.GetUserIdFromRefreshToken(tamperedToken);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.InvalidRefreshToken(), result.FirstError);
        }
    }
}
