using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.GetAll;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.GetAll
{
    public class GetAllCategoriesEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<List<BaseCategoryResponse>>
    {
        [HttpGet("api/categories")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Retrieve list of all active categories, excluding soft-deleted ones.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<List<BaseCategoryResponse>>> HandleAsync(
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllCategoriesQuery();
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
