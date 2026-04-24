using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.GetById;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.GetById
{
    public class GetCategoryByIdEndpoint
        : BaseEndpoint<PathGuidIdRequest, DetailedCategoryResponse>
    {
        public GetCategoryByIdEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/admin/categories/{id}")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get detailed information of a category by ID. For ADMIN users only.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<DetailedCategoryResponse>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCategoryByIdQuery(Guid.Parse(request.Id));
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }

    //public class GetCategoryByIdRequest
    //{
    //    public string Id { get; set; } = string.Empty;
    //}
}
