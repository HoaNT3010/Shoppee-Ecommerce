using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface IOrdersApi
    {
        [Post("/orders/place")]
        public Task PlaceOrder();
        [Get("/user/orders")]
        public Task<PagedList<UserOrderSummary>> ListUserOrders([Query] ListUserOrdersRequest request);
    }
}
