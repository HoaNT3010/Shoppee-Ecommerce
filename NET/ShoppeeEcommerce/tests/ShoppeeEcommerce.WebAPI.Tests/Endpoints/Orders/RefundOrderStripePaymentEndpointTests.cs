using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Refund;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Refund;
using ShoppeeEcommerce.WebAPI.Endpoints.Orders.Refund;
using ShoppeeEcommerce.WebAPI.Tests.Common;
using ShoppeeEcommerce.WebAPI.Utilities;

namespace ShoppeeEcommerce.WebAPI.Tests.Endpoints.Orders
{
    public class RefundOrderStripePaymentEndpointTests
        : AuthenticatedEndpointTestBase
    {
        readonly RefundOrderStripePaymentEndpoint _endpoint;

        public RefundOrderStripePaymentEndpointTests()
        {
            _endpoint = new RefundOrderStripePaymentEndpoint(_senderMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _claimsPrincipal }
                }
            };
        }

        [Fact]
        public async Task HandleAsync_ReturnsCreated_WhenCommandSucceeds()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var userId = _claimsPrincipal.GetUserId();
            var refundAmount = 5000L; // e.g., $50.00 in cents

            var request = new RefundStripePaymentRequest
            {
                OrderId = orderId.ToString(),
                PaymentId = paymentId.ToString(),
                Body = new RefundAmountRequest { Amount = refundAmount }
            };

            _senderMock.Setup(x => x.Send(
                    It.Is<CreateStripeRefundCommand>(c =>
                        c.OrderId == orderId &&
                        c.PaymentId == paymentId &&
                        c.Amount == refundAmount &&
                        c.UserId == userId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Created);

            // Act
            var actionResult = await _endpoint.HandleAsync(request);

            // Assert
            var result = Assert.IsType<ActionResult<Created>>(actionResult);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(StatusCodes.Status201Created, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ReturnsNotFound_WhenOrdersPaymentNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid().ToString();
            var expectedError = Errors.PaymentErrors.NotFoundWithId(paymentId);
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripeRefundCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync(new RefundStripePaymentRequest
            {
                OrderId = Guid.NewGuid().ToString(),
                PaymentId = paymentId,
                Body = new RefundAmountRequest { Amount = 100 }
            });

            // Assert
            var result = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ReturnsBadRequest_WhenOrdersPaymentNotRefundable()
        {
            // Arrange
            var paymentId = Guid.NewGuid().ToString();
            var expectedError = Errors.PaymentErrors.NotRefundable("Pending");
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripeRefundCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync(new RefundStripePaymentRequest
            {
                OrderId = Guid.NewGuid().ToString(),
                PaymentId = paymentId,
                Body = new RefundAmountRequest { Amount = 100 }
            });

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }

        [Fact]
        public async Task HandleAsync_ReturnsInternalServerError_WhenUnexpectedErrorOccurred()
        {
            // Arrange
            var expectedError = Errors.PaymentErrors.Stripe_RefundedPaymentFailed();
            _senderMock.Setup(x => x.Send(It.IsAny<CreateStripeRefundCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var actionResult = await _endpoint.HandleAsync(new RefundStripePaymentRequest
            {
                OrderId = Guid.NewGuid().ToString(),
                PaymentId = Guid.NewGuid().ToString(),
                Body = new RefundAmountRequest { Amount = 100 }
            });

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var errors = Assert.IsType<List<Error>>(result.Value);
            Assert.Single(errors);
            Assert.Equal(expectedError, errors[0]);
        }
    }
}
