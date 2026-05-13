using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Create;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Create;
using ShoppeeEcommerce.WebAPI.Endpoints.Orders.CreatePayment;
using ShoppeeEcommerce.WebAPI.Tests.Common;

namespace ShoppeeEcommerce.WebAPI.Tests.Endpoints.Orders
{
    public class CreateOrderStripePaymentEndpointTests
        : AuthenticatedEndpointTestBase
    {
        readonly CreateOrderStripePaymentEndpoint _endpoint;
        public CreateOrderStripePaymentEndpointTests()
        {
            _endpoint = new CreateOrderStripePaymentEndpoint(_senderMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _claimsPrincipal }
                }
            };
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnOk_WhenCommandSucceeds()
        {
            // Arrange
            var request = new PathGuidIdRequest(Guid.NewGuid().ToString());
            var expectedResponse = new CreateStripePaymentResponse
            {
                Amount = 100,
                Currency = "usd",
                StripeClientSecret = "client-secret",
                StripePaymentIntentId = "intent-id",
            };
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripePaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var actionResult = await _endpoint.HandleAsync(request);

            // Assert
            var result = Assert.IsType<ActionResult<CreateStripePaymentResponse>>(actionResult);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnNotFound_WhenOrderNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var expectedError = Errors.OrderErrors.NotFoundWithId(orderId);
            var request = new PathGuidIdRequest(orderId);
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripePaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync(request);

            // Assert
            var result = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenOrderIsNotPayable()
        {
            // Arrange
            var expectedError = Errors.PaymentErrors.Order_NotPayable();
            var request = new PathGuidIdRequest(Guid.NewGuid().ToString());
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripePaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync(request);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnConflict_WhenOrderAlreadyHasPayment()
        {
            // Arrange
            var expectedError = Errors.PaymentErrors.Order_HasExistingPayment();
            var request = new PathGuidIdRequest(Guid.NewGuid().ToString());
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripePaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync(request);

            // Assert
            var result = Assert.IsType<ConflictObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurred()
        {
            // Arrange
            var expectedError = Errors.PaymentErrors.Stripe_CreatePaymentIntentFailed();
            var request = new PathGuidIdRequest(Guid.NewGuid().ToString());
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripePaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync(request);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }
    }
}
