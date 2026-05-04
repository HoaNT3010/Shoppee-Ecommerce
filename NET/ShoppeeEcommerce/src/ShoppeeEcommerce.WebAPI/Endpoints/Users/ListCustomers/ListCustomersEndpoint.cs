using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Users.ListCustomers;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Users.ListCustomers;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Users.ListCustomers
{
    public class ListCustomersEndpoint
        : BaseEndpoint<ListCustomersRequest, PagedList<ListCustomersResponse>>
    {
        public ListCustomersEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/admin/users/customers")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get all customer users in the system. For ADMIN users only.",
            Tags = new[] { EndpointTags.Users })]
        public override async Task<ActionResult<PagedList<ListCustomersResponse>>> HandleAsync(
            [FromQuery] ListCustomersRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new ListCustomersQuery
            {
                SearchTerm = request.SearchTerm,
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
