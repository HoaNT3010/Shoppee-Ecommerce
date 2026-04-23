using Ardalis.ApiEndpoints;
using MediatR;

namespace ShoppeeEcommerce.WebAPI.Endpoints
{
    public abstract class BaseEndpoint<TRequest, TResponse>
        : EndpointBaseAsync
        .WithRequest<TRequest>
        .WithActionResult<TResponse>
    {
        protected readonly ISender sender;
        protected BaseEndpoint(ISender sender)
        {
            this.sender = sender;
        }
    }
}
