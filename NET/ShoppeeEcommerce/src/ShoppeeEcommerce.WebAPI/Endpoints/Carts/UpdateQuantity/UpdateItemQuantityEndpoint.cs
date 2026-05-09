using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Carts.UpdateQuantity;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.UpdateQuantity;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Carts.UpdateQuantity
{
    public class UpdateItemQuantityEndpoint
        : BaseEndpoint<UpdateItemQuantityRequest, Updated>
    {
        public UpdateItemQuantityEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/cart/items/{id}")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Update quantity of an item in cart.",
            Tags = new[] { EndpointTags.Cart })]
        public override async Task<ActionResult<Updated>> HandleAsync(UpdateItemQuantityRequest request, CancellationToken cancellationToken = default)
        {
            var command = new UpdateItemQuantityCommand(request.Id, request.Quantity.Quantity);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
