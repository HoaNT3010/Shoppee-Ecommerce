using Ardalis.ApiEndpoints;
using MediatR;
using Moq;
using ShoppeeEcommerce.WebAPI.Endpoints;

namespace ShoppeeEcommerce.WebAPI.Tests.Common
{
    public class EndpointTestBase
    {
        protected readonly Mock<ISender> _senderMock;

        protected EndpointTestBase()
        {
            _senderMock = new Mock<ISender>();
        }
    }
}
