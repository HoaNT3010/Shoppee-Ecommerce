using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Carts.AddItem;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.AddItem;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Carts.AddItem
{
    public class AddCartItemEndpoint
        : BaseEndpoint<AddCartItemRequest, Created>
    {
        public AddCartItemEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPost("api/v{version:apiVersion}/cart/items")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Add a product to cart.",
            Tags = new[] { EndpointTags.Cart })]
        public override async Task<ActionResult<Created>> HandleAsync(
            [FromBody] AddCartItemRequest request, CancellationToken cancellationToken = default)
        {
            var command = new AddCartItemCommand(request.ProductId, request.Quantity);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
