using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;

namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Orders
{
    public class UserOrdersViewModel
    {
        public PagedList<UserOrderSummary> Orders { get; set; }
        public ListUserOrdersRequest Request { get; set; } = new();
        public List<string> AvailableStatuses = ["Pending", "Paid", "Completed", "Cancelled", "Refunded"];
    }
}
