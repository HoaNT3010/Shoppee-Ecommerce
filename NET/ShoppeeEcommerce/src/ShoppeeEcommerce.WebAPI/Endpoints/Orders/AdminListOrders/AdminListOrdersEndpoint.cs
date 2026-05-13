using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Orders.AdminListOrders;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminListOrders;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.AdminListOrders
{
    public class AdminListOrdersEndpoint
        : BaseEndpoint<AdminListOrdersRequest, PagedList<AdminListOrdersResponse>>
    {
        public AdminListOrdersEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/admin/orders")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get orders with filter, sorting and pagination support. For ADMIN users only.",
            Tags = new[] { EndpointTags.OrdersAdmin })]
        public override async Task<ActionResult<PagedList<AdminListOrdersResponse>>> HandleAsync(
            [FromQuery] AdminListOrdersRequest request, CancellationToken cancellationToken = default)
        {
            var query = new AdminListOrdersQuery(request.Status,
                request.MinPrice,
                request.MaxPrice,
                request.FromCreatedDate,
                request.ToCreatedDate,
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
