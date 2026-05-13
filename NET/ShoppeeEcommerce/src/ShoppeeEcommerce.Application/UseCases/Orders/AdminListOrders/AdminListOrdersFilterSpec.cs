using Ardalis.Specification;
using ShoppeeEcommerce.Application.Utilities;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminListOrders;

namespace ShoppeeEcommerce.Application.UseCases.Orders.AdminListOrders
{
    internal class AdminListOrdersFilterSpec
        : Specification<Order, AdminListOrdersResponse>
    {
        public AdminListOrdersFilterSpec(AdminListOrdersQuery request)
        {
            if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse(request.Status, out OrderStatus status))
                Query.Where(o => o.Status == status);
            // Date range
            if (request.FromCreatedDate.HasValue)
                Query.Where(x =>
                    x.CreatedDate >= request.FromCreatedDate.Value.StartOfDay());
            if (request.ToCreatedDate.HasValue)
                Query.Where(x =>
                    x.CreatedDate <= request.ToCreatedDate.Value.Date.EndOfDay());
            // Price Range
            if (request.MinPrice.HasValue)
                Query.Where(x =>
                    x.TotalPrice >= request.MinPrice);
            if (request.MaxPrice.HasValue)
                Query.Where(x =>
                    x.TotalPrice <= request.MaxPrice);
            // Projection
            Query
                .AsNoTracking()
                .AsSplitQuery()
                .Select(o => new AdminListOrdersResponse
                (
                    o.Id,
                    o.UserId,
                    o.Status.ToString(),
                    o.TotalPrice,
                    o.CreatedDate,
                    o.UpdatedDate,
                    o.Items.Count
                ));
        }
    }
}
