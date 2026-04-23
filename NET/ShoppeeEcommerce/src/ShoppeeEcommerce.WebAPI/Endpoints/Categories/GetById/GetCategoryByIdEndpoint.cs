using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.GetById;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.GetById
{
    public class GetCategoryByIdEndpoint
        : BaseEndpoint<GetCategoryByIdRequest, DetailedCategoryResponse>
    {
        public GetCategoryByIdEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/categories/{id}")]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get detailed information of a category by ID. Only allow Admin user.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<DetailedCategoryResponse>> HandleAsync(
            [FromRoute] GetCategoryByIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCategoryByIdQuery(Guid.Parse(request.Id));
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }

    public class GetCategoryByIdRequest
    {
        public string Id { get; set; } = string.Empty;
    }
}
