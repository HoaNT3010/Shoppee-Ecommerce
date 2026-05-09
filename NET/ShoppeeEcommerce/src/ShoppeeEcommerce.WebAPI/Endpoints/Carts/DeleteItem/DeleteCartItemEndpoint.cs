using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Carts.DeleteItem;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Carts.DeleteItem
{
    public class DeleteCartItemEndpoint
        : BaseEndpoint<PathGuidIdRequest, Deleted>
    {
        public DeleteCartItemEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpDelete("api/v{version:apiVersion}/cart/items/{id}")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Remove an item from the cart. The path parameter 'ID' is for product ID.",
            Tags = new[] { EndpointTags.Cart })]
        public override async Task<ActionResult<Deleted>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new DeleteCartItemCommand(Guid.Parse(request.Id));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
