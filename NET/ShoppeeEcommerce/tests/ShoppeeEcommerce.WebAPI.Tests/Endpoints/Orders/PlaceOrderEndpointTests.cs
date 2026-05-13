using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppeeEcommerce.Application.UseCases.Orders.Place;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.WebAPI.Endpoints.Orders.Place;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace ShoppeeEcommerce.WebAPI.Tests.Endpoints.Orders
{
    public class PlaceOrderEndpointTests
    {
        readonly Mock<ISender> _senderMock;
        readonly PlaceOrderEndpoint _endpoint;

        public PlaceOrderEndpointTests()
        {
            _senderMock = new Mock<ISender>();
            _endpoint = new PlaceOrderEndpoint(_senderMock.Object);

            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnCreated_WhenCommandSucceeds()
        {
            // Arrange
            _senderMock.Setup(x => x.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Created);

            // Act
            var actionResult = await _endpoint.HandleAsync();

            // Assert
            var result = Assert.IsType<StatusCodeResult>(actionResult.Result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            _senderMock.Verify(x => x.Send(
                It.Is<PlaceOrderCommand>(c => c.UserId != Guid.Empty),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnNotFound_WhenCartDoesNotExist()
        {
            // Arrange
            var expectedError = Errors.OrderErrors.NoCartInfo();
            _senderMock.Setup(x => x.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync();

            // Assert
            var result = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);

            var error = errors.First();
            Assert.Equal("Order.NoCartInfo", error.Code);
            Assert.Equal("No shopping cart associated with user found.", error.Description);
            Assert.Equal(ErrorType.NotFound, error.Type);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenCheckoutFromGuestCart()
        {
            // Arrange
            var expectedError = Errors.OrderErrors.GuestCartNotAllowed();
            _senderMock.Setup(x => x.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync();

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenCheckoutFromEmptyCart()
        {

            // Arrange
            var expectedError = Errors.OrderErrors.EmptyCart();
            _senderMock.Setup(x => x.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync();

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenCartItemCountInvalid()
        {

            // Arrange
            var expectedError = Errors.OrderErrors.ItemCountNotMatch();
            _senderMock.Setup(x => x.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync();

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenCartContainsInvalidProduct()
        {

            // Arrange
            var expectedError = Errors.OrderErrors.ContainsInvalidProducts();
            _senderMock.Setup(x => x.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync();

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurred()
        {

            // Arrange
            var expectedError = Errors.OrderErrors.CreateOrderFailed();
            _senderMock.Setup(x => x.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync();

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }
    }
}
