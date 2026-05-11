using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;

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
    }
}
