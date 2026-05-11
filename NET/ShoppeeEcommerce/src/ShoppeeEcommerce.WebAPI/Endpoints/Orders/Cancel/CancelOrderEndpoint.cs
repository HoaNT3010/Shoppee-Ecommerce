using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Orders.Cancel;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.Cancel
{
    public class CancelOrderEndpoint
        : BaseEndpoint<PathGuidIdRequest, Updated>
    {
        public CancelOrderEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/orders/{id}/cancel")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
        [SwaggerOperation(
            Summary = "Cancel an order. Only PENDING order can be cancelled. For AUTHENTICATED users only.",
            Tags = new[] { EndpointTags.Orders })]
        public override async Task<ActionResult<Updated>> HandleAsync(PathGuidIdRequest request, CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var command = new CancelOrderCommand(Guid.Parse(request.Id), userId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
