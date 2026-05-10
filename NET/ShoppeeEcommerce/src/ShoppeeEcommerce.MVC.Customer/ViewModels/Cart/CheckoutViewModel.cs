using ShoppeeEcommerce.SharedViewModels.Models.Carts.View;

namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Cart
{
    public class CheckoutViewModel
    {
        public ViewCartResponse Cart { get; set; } = new ViewCartResponse();
        public CheckoutAddressVM Address { get; set; } = new CheckoutAddressVM();
        public CheckoutPaymentMethodVM Payment { get; set; } = new CheckoutPaymentMethodVM();
    }

    public class CheckoutAddressVM
    {
        public string ReceiverName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
    }

    public class CheckoutPaymentMethodVM
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
