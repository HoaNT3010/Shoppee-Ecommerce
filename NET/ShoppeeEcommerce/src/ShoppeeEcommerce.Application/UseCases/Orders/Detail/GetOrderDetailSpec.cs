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
                    }).ToList(),
                    Payment = o.Payment == null ? null : new OrderPaymentResponse
                    {
                        Id = o.Payment.Id,
                        Amount = o.Payment.Amount,
                        Status = o.Payment.Status.ToString(),
                        Method = o.Payment.Method.ToString(),
                        CreatedDate = o.Payment.CreatedDate,
                        UpdatedDate = o.Payment.UpdatedDate,
                        PaidDate = o.Payment.PaidDate,
                        RefundedDate = o.Payment.RefundedDate,
                        FailedDate = o.Payment.FailedDate,
                        ClientSecret = o.Payment.StripeClientSecret
                    }
                });
        }
    }
}
