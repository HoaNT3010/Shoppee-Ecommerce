using ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail;
using System.Net.NetworkInformation;

namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Orders
{
    public class OrderDetailViewModel
    {
        public OrderDetailResponse Order { get; set; }
        public bool CanAcceptPayment =>
            (Order.Status == "Pending" || Order.Status == "AwaitingPayment")
            && Order.Payment?.ClientSecret != null
            && Order.Payment?.Status == "Pending";

        public bool CanRefundPayment =>
            (Order.Status == "Paid")
            && Order.Payment?.Status == "Succeeded";
    }
}
