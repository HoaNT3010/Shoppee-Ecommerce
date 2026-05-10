using Refit;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface IOrdersApi
    {
        [Post("/orders/place")]
        public Task PlaceOrder();
    }
}
