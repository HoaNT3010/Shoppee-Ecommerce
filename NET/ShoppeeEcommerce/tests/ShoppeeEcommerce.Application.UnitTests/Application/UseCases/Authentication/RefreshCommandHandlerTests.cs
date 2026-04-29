using Microsoft.AspNetCore.Identity;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Authentication;
using ShoppeeEcommerce.Application.UseCases.Authentication.Refresh;
using ShoppeeEcommerce.Domain.Constants;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Authentication
{
    public class RefreshCommandHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IJwtTokenProvider> _jwtProviderMock;

        public RefreshCommandHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _jwtProviderMock = new Mock<IJwtTokenProvider>();
        }

        private RefreshCommandHandler CreateHandler()
        {
            return new RefreshCommandHandler(_userManagerMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenUserIdCannotBeExtractedFromToken()
        {
            // Arrange
            var command = new RefreshCommand("invalid-token");
            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(Errors.Authentication.InvalidRefreshToken());

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.InvalidRefreshToken(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var command = new RefreshCommand("valid-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(userId);
            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.User.NotFoundWithId(userId), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenNoTokenStoredInDatabase()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var command = new RefreshCommand("valid-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName))
                .ReturnsAsync((string?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.Unauthorized(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidRefreshToken_WhenTokenDoesNotMatchStoredToken()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var command = new RefreshCommand("old-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName))
                .ReturnsAsync("newer-token-in-db");

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.InvalidRefreshToken(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidRefreshToken_WhenTokenExpired()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var command = new RefreshCommand("expired-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(command.RefreshToken);

            // Final lifetime validation fails
            _jwtProviderMock.Setup(x => x.IsTokenValid(command.RefreshToken)).Returns(false);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.InvalidRefreshToken(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldRotateTokensAndReturnResponse_WhenValid()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var roles = new List<string> { "User" };
            var command = new RefreshCommand("valid-current-token");
            var newRefreshToken = "newly-generated-token";
            var newAccessToken = "new-access-token";

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(command.RefreshToken);
            _jwtProviderMock.Setup(x => x.IsTokenValid(command.RefreshToken)).Returns(true);

            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _jwtProviderMock.Setup(x => x.GenerateRefreshToken(user)).Returns(newRefreshToken);
            _jwtProviderMock.Setup(x => x.GenerateAccessToken(user, roles)).Returns(newAccessToken);

            _userManagerMock.Setup(x => x.SetAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<string>(), newRefreshToken))
                .ReturnsAsync(IdentityResult.Success);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(newAccessToken, result.Value.AccessToken);
            Assert.Equal(newRefreshToken, result.Value.RefreshToken);

            // Verify rotation occurred in database
            _userManagerMock.Verify(x => x.SetAuthenticationTokenAsync(
                user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName,
                newRefreshToken), Times.Once);
        }
    }
}
