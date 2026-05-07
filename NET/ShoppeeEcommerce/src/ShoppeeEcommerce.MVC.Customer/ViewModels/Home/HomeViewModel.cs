using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<BaseProductSummaryResponse> FeaturedProducts { get; set; } = [];
        public List<BaseProductSummaryResponse> NewestProducts { get; set; } = [];
    }
}
