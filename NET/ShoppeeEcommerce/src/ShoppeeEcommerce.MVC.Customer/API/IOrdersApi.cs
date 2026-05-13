using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Create;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Refund;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface IOrdersApi
    {
        [Post("/orders/place")]
        public Task PlaceOrder();
        [Get("/orders")]
        public Task<PagedList<UserOrderSummary>> ListUserOrders([Query] ListUserOrdersRequest request);
        [Get("/orders/{request.id}")]
        public Task<OrderDetailResponse> ViewOrderDetail(PathGuidIdRequest request);
        [Patch("/orders/{request.id}/cancel")]
        public Task CancelOrder(PathGuidIdRequest request);
        [Post("/orders/{request.id}/payment/stripe")]
        public Task<CreateStripePaymentResponse> CreatePayment(PathGuidIdRequest request);
        [Post("/orders/{orderId}/payment/stripe/{paymentId}/refund")]
        public Task RefundOrder([AliasAs("orderId")] Guid orderId,
            [AliasAs("paymentId")] Guid paymentId,
            [Body] RefundAmountRequest request);
    }
}
