using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Moq;
using ShoppeeEcommerce.Application.UseCases.Authentication.CustomerRegister;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Authentication
{
    public class CustomerRegisterCommandHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private const string CustomerRoleName = "Customer";

        public CustomerRegisterCommandHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private CustomerRegisterCommandHandler CreateHandler()
        {
            return new CustomerRegisterCommandHandler(_userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserNameExisted_WhenUserNameIsTaken()
        {
            // Arrange
            var command = CreateCommand();
            _userManagerMock.Setup(x => x.FindByNameAsync(command.UserName))
                .ReturnsAsync(new User());

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.User.UserNameExisted(command.UserName), result.FirstError);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmailExisted_WhenEmailIsTaken()
        {
            // Arrange
            var command = CreateCommand();
            _userManagerMock.Setup(x => x.FindByNameAsync(command.UserName))
                .ReturnsAsync((User?)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email))
                .ReturnsAsync(new User());

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.User.EmailExisted(command.Email), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnRegisterFailed_WhenIdentityCreateFails()
        {
            // Arrange
            var command = CreateCommand();
            _userManagerMock.Setup(x => x.FindByNameAsync(command.UserName)).ReturnsAsync((User?)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((User?)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), command.Password))
                .ReturnsAsync(IdentityResult.Failed());

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Authentication.CustomerRegisterFailed(), result.FirstError);
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreateUserAndAssignRole_WhenRequestIsValid()
        {
            // Arrange
            var command = CreateCommand();
            _userManagerMock.Setup(x => x.FindByNameAsync(command.UserName)).ReturnsAsync((User?)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((User?)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), command.Password))
                .ReturnsAsync(IdentityResult.Success);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Created, result.Value);

            // Verify User properties mapping
            _userManagerMock.Verify(x => x.CreateAsync(
                It.Is<User>(u =>
                    u.UserName == command.UserName &&
                    u.Email == command.Email &&
                    u.FirstName == command.FirstName &&
                    u.LastName == command.LastName),
                command.Password),
                Times.Once);

            // Verify Role assignment
            _userManagerMock.Verify(x => x.AddToRoleAsync(
                It.Is<User>(u => u.UserName == command.UserName),
                CustomerRoleName),
                Times.Once);
        }

        private static CustomerRegisterCommand CreateCommand() =>
            new(
                "johndoe",
                "john@example.com",
                "SecurePass123!",
                "SecurePass123!",
                "John",
                "Doe");
    }
}
