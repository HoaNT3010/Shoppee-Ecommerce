using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.UseCases.Authentication.GetAccountInfo;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Authentication
{
    public class GetAccountInfoQueryHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;

        public GetAccountInfoQueryHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private GetAccountInfoQueryHandler CreateHandler(
            Mock<ILogger<GetAccountInfoQueryHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? new Mock<ILogger<GetAccountInfoQueryHandler>>();
            return new GetAccountInfoQueryHandler(_userManagerMock.Object, logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetAccountInfoQuery(userId);

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.User.NotFoundWithId(userId.ToString()), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnAccountInfo_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "johndoe",
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe"
            };
            var roles = new List<string> { "Customer", "Premium" };
            var query = new GetAccountInfoQuery(user.Id);

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(user.Id, result.Value.Id);
            Assert.Equal(user.UserName, result.Value.Username);
            Assert.Equal(user.Email, result.Value.Email);
            Assert.Equal(roles.Count, result.Value.Roles.Length);
            Assert.Contains("Customer", result.Value.Roles);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureErrorAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetAccountInfoQuery(userId);
            var loggerMock = new Mock<ILogger<GetAccountInfoQueryHandler>>();

            // Setup to trigger catch block
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new Exception("Database connection timed out"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.User.GetUserInfoFailed(userId.ToString()), result.FirstError);

            // Verifying the log using standard Moq verification for Logger
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(userId.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
