using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Authentication;
using ShoppeeEcommerce.Application.UseCases.Authentication.Logout;
using ShoppeeEcommerce.Domain.Constants;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Authentication
{
    public class LogoutCommandHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IJwtTokenProvider> _jwtProviderMock;

        public LogoutCommandHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _jwtProviderMock = new Mock<IJwtTokenProvider>();
        }

        private LogoutCommandHandler CreateHandler()
        {
            return new LogoutCommandHandler(_userManagerMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var command = new LogoutCommand("invalid-token");
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
        public async Task Handle_ShouldReturnUserNotFound_WhenUserFromTokenDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var command = new LogoutCommand("valid-token");

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
        public async Task Handle_ShouldReturnDeleted_WhenNoTokenIsStoredInDatabase()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var command = new LogoutCommand("valid-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            // Simulate token not found in user tokens table
            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user,
                    ApplicationToken.ApplicationLoginProvider,
                    ApplicationToken.ApplicationRefreshTokenName))
                .ReturnsAsync((string?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Result.Deleted, result.Value);
            _userManagerMock.Verify(x => x.RemoveAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidRefreshToken_WhenTokenDoesNotMatchStoredToken()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var command = new LogoutCommand("mismatching-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user,
                    ApplicationToken.ApplicationLoginProvider,
                    ApplicationToken.ApplicationRefreshTokenName))
                .ReturnsAsync("different-token-in-db");

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.InvalidRefreshToken(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldRemoveTokenAndReturnDeleted_WhenTokenIsValid()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var command = new LogoutCommand("correct-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user,
                    ApplicationToken.ApplicationLoginProvider,
                    ApplicationToken.ApplicationRefreshTokenName))
                .ReturnsAsync(command.RefreshToken);

            _userManagerMock.Setup(x => x.RemoveAuthenticationTokenAsync(user,
                    ApplicationToken.ApplicationLoginProvider,
                    ApplicationToken.ApplicationRefreshTokenName))
                .ReturnsAsync(IdentityResult.Success);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Result.Deleted, result.Value);
            _userManagerMock.Verify(x => x.RemoveAuthenticationTokenAsync(user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnRevokeFailed_WhenIdentityFailsToRemoveToken()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var command = new LogoutCommand("valid-token");

            _jwtProviderMock.Setup(x => x.GetUserIdFromRefreshToken(command.RefreshToken))
                .Returns(user.Id.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(command.RefreshToken);

            _userManagerMock.Setup(x => x.RemoveAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.RevokeRefreshTokenFailed(), result.FirstError);
        }
    }
}
