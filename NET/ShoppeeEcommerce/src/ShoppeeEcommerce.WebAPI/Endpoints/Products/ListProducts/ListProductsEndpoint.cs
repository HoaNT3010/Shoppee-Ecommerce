using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.ListProducts;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ListProducts;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.ListProducts
{
    public class ListProductsEndpoint
        : BaseEndpoint<ListProductsRequest, PagedList<ListProductResponse>>
    {
        public ListProductsEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/products")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Query products with support for filter, sorting and paging.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<PagedList<ListProductResponse>>> HandleAsync(
            [FromQuery] ListProductsRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new ListProductsQuery(request.SearchTerm,
                request.MinPrice,
                request.MaxPrice,
                request.CategoryIds?.Select(Guid.Parse).ToList(),
                request.IsFeatured,
                request.SortBy,
                request.SortDesc,
                // Default page index 1
                request.PageIndex ?? 1,
                // Default page size 10
                request.PageSize ?? 10);
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
