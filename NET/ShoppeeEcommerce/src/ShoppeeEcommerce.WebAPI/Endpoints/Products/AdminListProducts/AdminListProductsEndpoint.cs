using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.AdminListProducts;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminListProducts;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.AdminListProducts
{
    public class AdminListProductsEndpoint
        : BaseEndpoint<AdminListProductsRequest, PagedList<AdminListProductsResponse>>
    {
        public AdminListProductsEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/admin/products")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get products with filter, sorting and pagination support. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public async override Task<ActionResult<PagedList<AdminListProductsResponse>>> HandleAsync(
            [FromQuery] AdminListProductsRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new AdminListProductsQuery
            {
                SearchTerm = request.SearchTerm,
                Status = request.Status,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                IncludeDeleted = request.IncludeDeleted,
                FromCreatedDate = request.FromCreatedDate,
                ToCreatedDate = request.ToCreatedDate,
                SortBy = request.SortBy,
                SortDesc = request.SortDesc,
                // Default page index 1
                PageIndex = request.PageIndex ?? 1,
                // Default page size 10
                PageSize = request.PageSize ?? 10
            };
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
