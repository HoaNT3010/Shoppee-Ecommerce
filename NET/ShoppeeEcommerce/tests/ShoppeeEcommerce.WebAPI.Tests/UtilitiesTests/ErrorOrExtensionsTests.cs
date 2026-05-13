using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.WebAPI.Utilities;

namespace ShoppeeEcommerce.WebAPI.Tests.UtilitiesTests
{
    public class ErrorOrExtensionsTests
    {
        [Fact]
        public void ToActionResult_ReturnsConflictObjectResult_WhenConflictError()
        {
            // Arrange
            var error = Error.Conflict(description: "Conflict occurred");
            ErrorOr<string> result = error;

            // Act
            var actionResult = result.ToActionResult();

            // Assert
            var objectResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status409Conflict, objectResult.StatusCode);
        }

        [Fact]
        public void ToActionResult_ReturnsNotFoundObjectResult_WhenNotFoundError()
        {
            // Arrange
            var error = Error.NotFound(description: "Not found");
            ErrorOr<string> result = error;

            // Act
            var actionResult = result.ToActionResult();

            // Assert
            var objectResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public void ToActionResult_Returns201Created_WhenCreatedSuccess()
        {
            // Arrange
            ErrorOr<Created> result = Result.Created;

            // Act
            var actionResult = result.ToActionResult();

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status201Created, statusCodeResult.StatusCode);
        }

        [Fact]
        public void ToActionResult_ReturnsOkObjectResult_WhenValueIsData()
        {
            // Arrange
            var data = new { Name = "Test Item" };
            ErrorOr<object> result = data;

            // Act
            var actionResult = result.ToActionResult();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(data, okObjectResult.Value);
        }

        [Theory]
        [InlineData(ErrorType.Validation, typeof(BadRequestObjectResult))]
        [InlineData(ErrorType.Unauthorized, typeof(UnauthorizedObjectResult))]
        [InlineData(ErrorType.Unexpected, typeof(ObjectResult))] // Maps to 500
        public void ToActionResult_MapsCorrectErrorTypes(ErrorType errorType, Type expectedLayoutType)
        {
            // Arrange
            var error = Error.Custom((int)errorType, "code", "description");
            ErrorOr<string> result = error;

            // Act
            var actionResult = result.ToActionResult();

            // Assert
            Assert.IsType(expectedLayoutType, actionResult.Result);
        }
    }
}
