using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Authentication;
using ShoppeeEcommerce.Application.UseCases.Authentication.Login;
using ShoppeeEcommerce.Domain.Constants;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Authentication
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IJwtTokenProvider> _jwtProviderMock;

        public LoginCommandHandlerTests()
        {
            // Identity managers require complex constructors; mocking them this way is standard for Moq
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null!, null!, null!, null!);

            _jwtProviderMock = new Mock<IJwtTokenProvider>();
        }

        private LoginCommandHandler CreateHandler()
        {
            return new LoginCommandHandler(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _jwtProviderMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidCredentials_WhenUserDoesNotExist()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "Password123!");
            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email))
                .ReturnsAsync((User?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.InvalidCredentials(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnLockedOut_WhenIdentityResultIsLockedOut()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var command = new LoginCommand(user.Email, "Password123!");

            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.Password, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.User.LockedOut(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidCredentials_WhenPasswordIsWrong()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var command = new LoginCommand(user.Email, "WrongPassword");

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.Password, true))
                .ReturnsAsync(SignInResult.Failed);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.InvalidCredentials(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnGenerateRefreshTokenFailed_WhenTokenCannotBeStored()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var command = new LoginCommand(user.Email, "Password123!");

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.Password, true))
                .ReturnsAsync(SignInResult.Success);

            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            _jwtProviderMock.Setup(x => x.GenerateRefreshToken(user)).Returns("refresh-token");

            // Simulate Identity failing to save the token
            _userManagerMock.Setup(x => x.SetAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.GenerateRefreshTokenFailed(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var roles = new List<string> { "Admin" };
            var command = new LoginCommand(user.Email, "Password123!");
            var expectedAccessToken = "access-token-123";
            var expectedRefreshToken = "refresh-token-456";

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.Password, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _userManagerMock.Setup(x => x.SetAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _jwtProviderMock.Setup(x => x.GenerateRefreshToken(user)).Returns(expectedRefreshToken);
            _jwtProviderMock.Setup(x => x.GenerateAccessToken(user, roles)).Returns(expectedAccessToken);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(expectedAccessToken, result.Value.AccessToken);
            Assert.Equal(expectedRefreshToken, result.Value.RefreshToken);

            _userManagerMock.Verify(x => x.SetAuthenticationTokenAsync(
                user,
                ApplicationToken.ApplicationLoginProvider,
                ApplicationToken.ApplicationRefreshTokenName,
                expectedRefreshToken), Times.Once);
        }
    }
}
