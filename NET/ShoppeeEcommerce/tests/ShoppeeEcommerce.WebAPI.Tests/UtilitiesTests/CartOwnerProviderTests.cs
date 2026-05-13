using Microsoft.AspNetCore.Http;
using Moq;
using ShoppeeEcommerce.WebAPI.Utilities;

namespace ShoppeeEcommerce.WebAPI.Tests.UtilitiesTests
{
    public class CartOwnerProviderTests
    {
        readonly Mock<IHttpContextAccessor> _mockContextAccessor;
        readonly CartOwnerProvider _provider;

        public CartOwnerProviderTests()
        {
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _provider = new CartOwnerProvider(_mockContextAccessor.Object);
        }

        private void SetupHttpContextWithHeaders(IHeaderDictionary headers)
        {
            //var mockResponse = new Mock<HttpResponse>();
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Headers).Returns(headers);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Request).Returns(mockRequest.Object);

            _mockContextAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);
        }

        [Fact]
        public async Task GetOwnerAsync_ReturnsBothValues_WhenBothHeadersExist()
        {
            // Arrange
            var expectedUserId = Guid.NewGuid();
            var expectedSessionId = "session-123";

            var headers = new HeaderDictionary
            {
                { "X-Cart-Customer", expectedUserId.ToString() },
                { "X-Cart-Session", expectedSessionId }
            };

            SetupHttpContextWithHeaders(headers);

            // Act
            var (userId, sessionId) = await _provider.GetOwnerAsync();

            // Assert
            Assert.Equal(expectedUserId, userId);
            Assert.Equal(expectedSessionId, sessionId);
        }

        [Fact]
        public async Task GetOwnerAsync_ReturnsNulls_WhenHeadersAreMissing()
        {
            // Arrange
            var headers = new HeaderDictionary(); // Empty headers
            SetupHttpContextWithHeaders(headers);

            // Act
            var (userId, sessionId) = await _provider.GetOwnerAsync();

            // Assert
            Assert.Null(userId);
            Assert.Null(sessionId);
        }

        [Fact]
        public async Task GetOwnerAsync_ReturnsNulls_WhenHeadersAreEmptyStrings()
        {
            // Arrange
            var headers = new HeaderDictionary
            {
                { "X-Cart-Customer", "" },
                { "X-Cart-Session", " " }
            };
            SetupHttpContextWithHeaders(headers);

            // Act
            var (userId, sessionId) = await _provider.GetOwnerAsync();

            // Assert
            Assert.Null(userId);
            Assert.Null(sessionId);
        }

        [Fact]
        public async Task GetOwnerAsync_ReturnsUserId_WhenOnlyValidCustomerHeaderExist()
        {
            // Arrange
            var expectedUserId = Guid.NewGuid();
            var headers = new HeaderDictionary
            {
                { "X-Cart-Customer", expectedUserId.ToString() },
            };
            SetupHttpContextWithHeaders(headers);

            // Act
            var (userId, sessionId) = await _provider.GetOwnerAsync();

            // Assert
            Assert.Equal(expectedUserId, userId);
            Assert.Null(sessionId);
        }

        [Fact]
        public async Task GetOwnerAsync_ReturnsSessionId_WhenOnlyValidSessionHeaderExist()
        {
            // Arrange
            var expectedSessionId = "session-123";
            var headers = new HeaderDictionary
            {
                { "X-Cart-Session", expectedSessionId },
            };
            SetupHttpContextWithHeaders(headers);

            // Act
            var (userId, sessionId) = await _provider.GetOwnerAsync();

            // Assert
            Assert.Equal(expectedSessionId, sessionId);
            Assert.Null(userId);
        }
    }
}
