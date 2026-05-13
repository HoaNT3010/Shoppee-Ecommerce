using ShoppeeEcommerce.WebAPI.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppeeEcommerce.WebAPI.Tests.UtilitiesTests
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void GetUserId_ReturnsGuid_WhenSubClaimExists()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, expectedGuid.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Act
            var result = user.GetUserId();

            // Assert
            Assert.Equal(expectedGuid, result);
        }

        [Fact]
        public void GetUserId_ThrowsUnauthorizedAccessException_WhenSubClaimIsMissing()
        {
            // Arrange
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Act

            // Assert
            var exception = Assert.Throws<UnauthorizedAccessException>(() => user.GetUserId());
            Assert.Equal("User id claim not found.", exception.Message);
        }

        [Fact]
        public void GetUserId_ThrowsFormatException_WhenSubClaimIsNotValidGuid()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, "not-a-guid")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Act
            var act = () => user.GetUserId();

            // Assert
            Assert.Throws<FormatException>(() => user.GetUserId());
        }
    }
}
