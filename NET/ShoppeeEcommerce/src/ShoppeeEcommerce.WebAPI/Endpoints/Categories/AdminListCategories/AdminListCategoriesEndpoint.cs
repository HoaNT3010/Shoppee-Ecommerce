using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.AdminListCategories;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.AdminListCategories;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.AdminListCategories
{
    public class AdminListCategoriesEndpoint
        : BaseEndpoint<
            AdminListCategoriesRequest,
            PagedList<AdminListCategoryResponse>>
    {
        public AdminListCategoriesEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/admin/categories")]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get categories with filter, sorting and pagination support. Only allow Admin users.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<PagedList<AdminListCategoryResponse>>> HandleAsync(
            [FromQuery] AdminListCategoriesRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new AdminListCategoriesQuery
            {
                SearchTerm = request.SearchTerm,
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
