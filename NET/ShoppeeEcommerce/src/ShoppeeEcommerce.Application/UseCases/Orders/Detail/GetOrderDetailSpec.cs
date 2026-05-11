using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Detail
{
    internal class GetOrderDetailSpec
        : Specification<Order, OrderDetailResponse>
    {
        public GetOrderDetailSpec(Guid orderId)
        {
            Query.Where(o => o.Id == orderId)
                .AsNoTracking()
                .Select(o => new OrderDetailResponse
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    Status = o.Status.ToString(),
                    TotalPrice = o.TotalPrice,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    Items = o.Items.Select(i => new OrderItemDetailResponse
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        ProductSKU = i.ProductSKU,
                        ProductImgUrl = i.ProductImgUrl,
                        Price = i.PriceSnapshot,
                        Quantity = i.Quantity,
                    }).ToList()
                });
        }
    }
}
